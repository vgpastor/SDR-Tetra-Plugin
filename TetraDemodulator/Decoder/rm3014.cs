using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDRSharp.Tetra
{
    unsafe class rm3014
    {
        private UnsafeBuffer _possibleVectors;
        private uint* _possibleVectorsPtr;

        private UnsafeBuffer _possibleErrors;
        private uint* _possibleErrorsPtr;

        private UnsafeBuffer _onesCounter;
        private byte* _onesCounterPtr;

        //private UnsafeBuffer _inTestBuffer;
        //private byte* _inTestBufferPtr;

        //private UnsafeBuffer _outTestBuffer;
        //private byte* _outTestBufferPtr;

        private static byte[,] genMatrix = new byte[14, 30]
        {
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0 },
            { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0 },
            { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0 },
            { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 1, 0, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1 }
        };

        //Данный код - декодер рида малера для RM(30,14)
        //Код не универсальный, оптимизирован для тетры
        //Восстанавливает 3 бита из 30 переданных.
 
        public void Init()
        {
            _possibleVectors = UnsafeBuffer.Create(16384, sizeof(uint));
            _possibleVectorsPtr = (uint*)_possibleVectors;

            _possibleErrors = UnsafeBuffer.Create(470, sizeof(uint));
            _possibleErrorsPtr = (uint*)_possibleErrors;

            _onesCounter = UnsafeBuffer.Create(256, sizeof(byte));
            _onesCounterPtr = (byte*)_onesCounter;

            var result = (uint)0;
            var checkBit = (uint)0;

            //Таблица правильных векторов
            //Все возможные варианты, 16К х 30 бит
            for (int j = 0; j < 16384; j++)
            {
                result = 0;

                for (int column = 0; column < 30; column++)
                {
                    checkBit = 0;

                    for (int row = 0; row < 14; row++)
                    {
                        checkBit ^= (uint)((j >> (13 - row)) & 1) & genMatrix[row, column];
                    }

                    result |= (uint)(checkBit << (29 - column));
                }

                _possibleVectorsPtr[j] = result;
            }

            //Таблица определения хэменигового расстояния
            //для байта
            for (uint j = 0; j < 256; j++)
            {
                var predBits = j;
                var counter = (uint) 0;

                while (predBits != 0)
                {
                    counter += predBits & 1;
                    predBits >>= 1;
                }

                _onesCounterPtr[j] = (byte) counter;
            }

            //Таблица возможных ошибок от 1 до 3 бит
            //Все возможные варианты, 470 х 14 бит
            uint index = 0;
            _possibleErrorsPtr[index++] = 0;
            for (int i = 1; i < 16384; i++)
            {
                if ((_onesCounterPtr[(i >> 8) & 0xFF] + _onesCounterPtr[(i & 0xff)]) == 1)
                {
                    _possibleErrorsPtr[index++] = (uint)i;
                }
            }
            for (int i = 1; i < 16384; i++)
            {
                if ((_onesCounterPtr[(i >> 8) & 0xFF] + _onesCounterPtr[(i & 0xff)]) == 2)
                {
                    _possibleErrorsPtr[index++] = (uint)i;
                }
            }
            for (int i = 1; i < 16384; i++)
            {
                if ((_onesCounterPtr[(i >> 8) & 0xFF] + _onesCounterPtr[(i & 0xff)]) == 3)
                {
                    _possibleErrorsPtr[index++] = (uint)i;
                }
            }
            //test();
        }

        /*public void test()
        {
            var rnd = new Random();

            _inTestBuffer = UnsafeBuffer.Create(32, sizeof(byte));
            _inTestBufferPtr = (byte*)_inTestBuffer;

            _outTestBuffer = UnsafeBuffer.Create(14, sizeof(byte));
            _outTestBufferPtr = (byte*)_outTestBuffer;

            var error = 0;
            var errorCount = 0;
            for (int i = 0; i < 16384; i++)
            {
                var data = _possibleVectorsPtr[i];

                for (int n = 0; n < 3; n++)
                {
                    var bitNum = rnd.Next(0, 29);
                    data ^= (1u << bitNum);
                }

                TetraUtils.IntToBits((int)data, _inTestBufferPtr, 0);

                Process(_inTestBufferPtr + 2, _outTestBufferPtr);

                TetraUtils.IntToBits((int)_possibleVectorsPtr[i], _inTestBufferPtr, 0);

                error = 0;
                for (int j = 0; j < 14; j++)
                {
                    if (_inTestBufferPtr[j + 2] != _outTestBufferPtr[j])
                        error++;
                }
                if (error != 0)
                    errorCount++;

            }
        }*/

        public bool Process(byte* inBuffer, byte* outBuffer)
        {
            var res = 0;

            uint xorValue;
            int counter;
            int minCounter = int.MaxValue;
            uint repairVector;

            //Принятое кодовое слово
            var vector = TetraUtils.BitsToUInt32(inBuffer, 0, 30);
            //Исходный вектор
            var vectorData = vector >> 16;
            
            for (int i = 0; i < 470; i++)
            {
                //Предискажение исходного вектора, i == 0 - без предискажений
                //Правильное кодовое слово для данного исходного вектора
                repairVector = _possibleVectorsPtr[vectorData ^ _possibleErrorsPtr[i]];

                //Сравниваем принятое кодовое слово с правильным табличным значением
                xorValue = vector ^ repairVector;

                //Подсчитываем расстояние хэминга по таблицам
                counter = _onesCounterPtr[xorValue & 0xff] + _onesCounterPtr[(xorValue >> 8) & 0xff]
                    + _onesCounterPtr[(xorValue >> 16) & 0xff] + _onesCounterPtr[(xorValue >> 24) & 0xff];

                //Правильное решение будет с минимальным расстоянием
                //Если расстояние меньше 4 - это верное решение для этого кода
                if (counter < minCounter)
                {
                    minCounter = counter;
                    res = (int)(repairVector >> 16);
                    
                    if (counter < 4) break;
                }
            }

            for (int i = 0; i < 14; i++)
            {
                outBuffer[i] = (byte)((res & 0x2000) == 0 ? 0 : 1);
                res <<= 1;
            }

            return minCounter < 4;
        }
    }
}
