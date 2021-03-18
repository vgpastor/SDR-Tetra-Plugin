// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.MotherCode
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using SDRSharp.Radio;

namespace SDRSharp.Tetra
{
    internal class MotherCode
    {
        private const int MotherCodeLength = 4;
        private const int CodedDataLength = 5;
        private const float BerAlphaCoeff = 0.99f;
        private const float BerBetaCoeff = 0.00999999f;
        private UnsafeBuffer _hamingLengthResult = UnsafeBuffer.Create(16384, 1);
        private unsafe byte* _hamingLengthPtr;
        private UnsafeBuffer _tempBuffer = UnsafeBuffer.Create(8192, 1);
        private unsafe byte* _tempBufferPtr;
        private int[] _prevSum = new int[32];
        private int[] _currentSum = new int[16];
        private float _ber;
        private readonly byte[,] _nextState = new byte[32, 2]
        {
      {
        (byte) 0,
        (byte) 16
      },
      {
        (byte) 0,
        (byte) 16
      },
      {
        (byte) 1,
        (byte) 17
      },
      {
        (byte) 1,
        (byte) 17
      },
      {
        (byte) 2,
        (byte) 18
      },
      {
        (byte) 2,
        (byte) 18
      },
      {
        (byte) 3,
        (byte) 19
      },
      {
        (byte) 3,
        (byte) 19
      },
      {
        (byte) 4,
        (byte) 20
      },
      {
        (byte) 4,
        (byte) 20
      },
      {
        (byte) 5,
        (byte) 21
      },
      {
        (byte) 5,
        (byte) 21
      },
      {
        (byte) 6,
        (byte) 22
      },
      {
        (byte) 6,
        (byte) 22
      },
      {
        (byte) 7,
        (byte) 23
      },
      {
        (byte) 7,
        (byte) 23
      },
      {
        (byte) 8,
        (byte) 24
      },
      {
        (byte) 8,
        (byte) 24
      },
      {
        (byte) 9,
        (byte) 25
      },
      {
        (byte) 9,
        (byte) 25
      },
      {
        (byte) 10,
        (byte) 26
      },
      {
        (byte) 10,
        (byte) 26
      },
      {
        (byte) 11,
        (byte) 27
      },
      {
        (byte) 11,
        (byte) 27
      },
      {
        (byte) 12,
        (byte) 28
      },
      {
        (byte) 12,
        (byte) 28
      },
      {
        (byte) 13,
        (byte) 29
      },
      {
        (byte) 13,
        (byte) 29
      },
      {
        (byte) 14,
        (byte) 30
      },
      {
        (byte) 14,
        (byte) 30
      },
      {
        (byte) 15,
        (byte) 31
      },
      {
        (byte) 15,
        (byte) 31
      }
        };
        private readonly sbyte[] _lutBitsG1 = new sbyte[32]
        {
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1
        };
        private readonly sbyte[] _lutBitsG2 = new sbyte[32]
        {
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1
        };
        private readonly sbyte[] _lutBitsG3 = new sbyte[32]
        {
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1
        };
        private readonly sbyte[] _lutBitsG4 = new sbyte[32]
        {
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 1,
      (sbyte) 1,
      (sbyte) -1
        };
        private readonly sbyte[] _absLut = new sbyte[5]
        {
      (sbyte) 2,
      (sbyte) 1,
      (sbyte) 0,
      (sbyte) 1,
      (sbyte) 2
        };

        public unsafe void DecoderInit()
        {
            this._hamingLengthPtr = (byte*)(void*)this._hamingLengthResult;
            this._tempBufferPtr = (byte*)(void*)this._tempBuffer;
        }

        public unsafe float BufferDecode(sbyte* source, byte* dest, int sourceLength)
        {
            int num1 = 0;
            int num2 = sourceLength / 4 - 4;
            int num3 = 0;
            fixed (sbyte* numPtr1 = this._lutBitsG1)
            fixed (sbyte* numPtr2 = this._lutBitsG2)
            fixed (sbyte* numPtr3 = this._lutBitsG3)
            fixed (sbyte* numPtr4 = this._lutBitsG4)
            fixed (sbyte* numPtr5 = this._absLut)
            {
                for (int index1 = 0; index1 < num2; ++index1)
                {
                    int num4 = index1 * 32;
                    sbyte* numPtr6 = source;
                    int index2 = num1;
                    int num5 = index2 + 1;
                    int num6 = (int)numPtr6[index2];
                    sbyte* numPtr7 = source;
                    int index3 = num5;
                    int num7 = index3 + 1;
                    int num8 = (int)numPtr7[index3];
                    sbyte* numPtr8 = source;
                    int index4 = num7;
                    int num9 = index4 + 1;
                    int num10 = (int)numPtr8[index4];
                    sbyte* numPtr9 = source;
                    int index5 = num9;
                    num1 = index5 + 1;
                    int num11 = (int)numPtr9[index5];
                    if (num6 == 0)
                        ++num3;
                    if (num8 == 0)
                        ++num3;
                    if (num10 == 0)
                        ++num3;
                    if (num11 == 0)
                        ++num3;
                    for (int index6 = 0; index6 < 32; ++index6)
                    {
                        int num12 = (int)numPtr5[(int)numPtr1[index6] - num6 + 2] + (int)numPtr5[(int)numPtr2[index6] - num8 + 2] + (int)numPtr5[(int)numPtr3[index6] - num10 + 2] + (int)numPtr5[(int)numPtr4[index6] - num11 + 2];
                        this._hamingLengthPtr[num4 + index6] = (byte)num12;
                    }
                }
            }
            int num13;
            int num14;
            fixed (int* numPtr1 = this._prevSum)
            fixed (int* numPtr2 = this._currentSum)
            {
                for (int index = 0; index < 32; ++index)
                    numPtr1[index] = 0;
                for (int index = 0; index < 16; ++index)
                    numPtr2[index] = 0;
                for (int index1 = 0; index1 < num2; ++index1)
                {
                    int num4 = index1 * 32;
                    int num5 = index1 * 16;
                    for (int index2 = 0; index2 < 16; ++index2)
                    {
                        int index3 = index2 * 2;
                        int index4 = index3 + 1;
                        int num6 = (int)this._hamingLengthPtr[num4 + index3] + numPtr1[index3];
                        int num7 = (int)this._hamingLengthPtr[num4 + index4] + numPtr1[index4];
                        if (num7 > num6)
                        {
                            numPtr2[index2] = num6;
                            this._tempBufferPtr[num5 + index2] = (byte)index3;
                        }
                        else
                        {
                            numPtr2[index2] = num7;
                            this._tempBufferPtr[num5 + index2] = (byte)index4;
                        }
                    }
                    for (int index2 = 0; index2 < 16; ++index2)
                        numPtr1[index2] = numPtr1[index2 + 16] = numPtr2[index2];
                }
                num13 = int.MaxValue;
                num14 = 0;
                for (int index = 0; index < 16; ++index)
                {
                    if (numPtr2[index] < num13)
                    {
                        num13 = numPtr2[index];
                        num14 = index;
                    }
                }
            }
            for (int index = num2 - 1; index >= 0; --index)
            {
                dest[index] = num14 < 8 ? (byte)0 : (byte)1;
                num14 = (int)this._tempBufferPtr[index * 16 + num14] & 15;
            }
            this._ber = (float)((double)(num13 - num3) / (double)(sourceLength - num3) * 100.0);
            return this._ber;
        }
    }
}
