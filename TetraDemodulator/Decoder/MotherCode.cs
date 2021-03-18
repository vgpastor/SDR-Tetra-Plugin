using SDRSharp.Radio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDRSharp.Tetra
{
    unsafe class MotherCode
    {
        private const int MotherCodeLength = 4;
        private const int CodedDataLength = 5;

        private const float BerAlphaCoeff = 0.99f;
        private const float BerBetaCoeff = 1 - BerAlphaCoeff;
        
        private UnsafeBuffer _hamingLengthResult = UnsafeBuffer.Create(512 * 32, sizeof(byte));
        private byte* _hamingLengthPtr;
        private UnsafeBuffer _tempBuffer = UnsafeBuffer.Create(512 * 16, sizeof(byte));
        private byte* _tempBufferPtr;

        private int[] _prevSum = new int[32];
        private int[] _prevSource = new int[32];
        private int[] _currentSource = new int[16];
        private int[] _currentLowerSums = new int[16];

        private float _ber;

        //Не используется, только для справки.
        private readonly byte[,] _nextState = new byte[32, 2]
        {
            { 0, 16 }, { 0, 16 }, { 1, 17 }, { 1, 17 }, { 2, 18 }, { 2, 18 }, { 3, 19 }, { 3, 19 },
            { 4, 20 }, { 4, 20 }, { 5, 21 }, { 5, 21 }, { 6, 22 }, { 6, 22 }, { 7, 23 }, { 7, 23 },
            { 8, 24 }, { 8, 24 }, { 9, 25 }, { 9, 25 }, { 10, 26 }, { 10, 26 }, { 11, 27 }, { 11, 27 },
            { 12, 28 }, { 12, 28 }, { 13, 29 }, { 13, 29 }, { 14, 30 }, { 14, 30 }, { 15, 31 }, { 15, 31 },
        };

        private readonly sbyte[] _lutBitsG1 = new sbyte[] { -1, 1, -1, 1, -1, 1, -1, 1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, -1, 1, -1, 1, -1, 1, -1, 1 };
        private readonly sbyte[] _lutBitsG2 = new sbyte[] { -1, 1, 1, -1, 1, -1, -1, 1, -1, 1, 1, -1, 1, -1, -1, 1, 1, -1, -1, 1, -1, 1, 1, -1, 1, -1, -1, 1, -1, 1, 1, -1 };
        private readonly sbyte[] _lutBitsG3 = new sbyte[] { -1, 1, -1, 1, 1, -1, 1, -1, 1, -1, 1, -1, -1, 1, -1, 1, 1, -1, 1, -1, -1, 1, -1, 1, -1, 1, -1, 1, 1, -1, 1, -1 };
        private readonly sbyte[] _lutBitsG4 = new sbyte[] { -1, 1, 1, -1, -1, 1, 1, -1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, -1, 1, 1, -1, -1, 1, 1, -1 };
        private readonly sbyte[] _absLut = new sbyte[] { 2, 1, 0, 1, 2 };
        
        //это таблица кодера K=5, N=4, G1=11001, G2=10111, G3=11101, G4=11011 где:
        //второй индекс - состояние кодера (т.е. значение 5 сдвиговых регистров)
        //первый индекс - номер выходного бита (для этой скорости их 4)
        //значения массива - значения выходных бит для каждого состояния.
        //Здесь заточено под мягкие решения, поэтому значения выходных бит -1 = 0 и +1 = 1,
        //

        /* Mother code according to Section 8.2.3.1.1 */
        public void DecoderInit()
        {
            _hamingLengthPtr = (byte*)_hamingLengthResult;
            _tempBufferPtr = (byte*)_tempBuffer;
        }

        //Витерби декодер.
        public float BufferDecode(sbyte* source, byte* dest, int sourceLength)
        {

            var offset = 0;
            var destLength = (sourceLength / 4) - 4;

            var g1 = 0;
            var g2 = 0;
            var g3 = 0;
            var g4 = 0;

            var ham = 0;
            
            var minSum = int.MaxValue;
            var minSumIndex = 0;

            var iIndex32 = 0;
            var iIndex16 = 0;

            var zeroCounter = 0;

            fixed (sbyte* lutG1Ptr = _lutBitsG1, lutG2Ptr = _lutBitsG2, lutG3Ptr = _lutBitsG3, lutG4Ptr = _lutBitsG4, absLutPtr = _absLut)
            {
                for (int i = 0; i < destLength; i++)
                {
                    iIndex32 = i * 32;

                    g1 = source[offset++];
                    g2 = source[offset++];
                    g3 = source[offset++];
                    g4 = source[offset++];

                    if (g1 == 0) zeroCounter++;
                    if (g2 == 0) zeroCounter++;
                    if (g3 == 0) zeroCounter++;
                    if (g4 == 0) zeroCounter++;

                    for (int j = 0; j < 32; j++)
                    {
                        ham = absLutPtr[(lutG1Ptr[j] - g1) + 2];
                        ham += absLutPtr[(lutG2Ptr[j] - g2) + 2];
                        ham += absLutPtr[(lutG3Ptr[j] - g3) + 2];
                        ham += absLutPtr[(lutG4Ptr[j] - g4) + 2];

                        _hamingLengthPtr[iIndex32 + j] = (byte)(ham);
                    }
                }
            }

            fixed (int* prevSumPtr = _prevSum, prevSourcePtr = _prevSource, currentSumPtr = _currentLowerSums, currentSourcePtr = _currentSource)
            {
                //Инициализируем массивы перед использованием
                for (int j = 0; j < 32; j++)
                {
                    prevSumPtr[j] = 0;
                    prevSourcePtr[j] = j;
                    if (j < 16)
                    {
                        currentSourcePtr[j] = 0;
                        currentSumPtr[j] = 0;
                    }
                }

                var index1 = 0;
                var index2 = 0;
                var len1 = (byte)0;
                var len2 = (byte)0;

                for (int i = 0; i < destLength; i++)
                {
                    minSum = int.MaxValue;
                    minSumIndex = 0;
                    iIndex32 = i * 32;
                    iIndex16 = i * 16;

                    for (int j = 0; j < 16; j++)
                    {
                        index1 = j * 2;
                        index2 = index1 + 1;

                        len1 = _hamingLengthPtr[iIndex32 + index1];
                        len2 = _hamingLengthPtr[iIndex32 + index2];

                        if (len1 > len2)
                        {
                            index1 = index2;
                            len1 = len2;
                        }

                        currentSourcePtr[j] = prevSourcePtr[index1];
                        currentSumPtr[j] = prevSumPtr[index1] + len1;

                        _tempBufferPtr[iIndex16 + j] = (byte)currentSourcePtr[j];

                        if (currentSumPtr[j] < minSum)
                        {
                            minSum = currentSumPtr[j];
                            minSumIndex = j;
                        }
                    }

                    for (int j = 0; j < 16; j++)
                    {
                        prevSourcePtr[j] = prevSourcePtr[j + 16] = currentSourcePtr[j];
                        prevSumPtr[j] = prevSumPtr[j + 16] = currentSumPtr[j];
                    }
                }

                var answer = currentSourcePtr[minSumIndex];

                for (int i = 0; i < destLength; i++)
                {
                    iIndex16 = i * 16;

                    for (int j = 0; j < 16; j++)
                    {
                        if (_tempBufferPtr[iIndex16 + j] == answer)
                        {
                            dest[i] = (byte)(j < 8 ? 0 : 1);
                            break;
                        }
                    }
                }
            }

            _ber = ((float)(minSum - zeroCounter) / (sourceLength - zeroCounter)) * 100.0f;
            
            return _ber;
        }   
    }
}
