using SDRSharp.Radio;
using System;

namespace SDRSharp.Tetra
{
    internal class Rm3014
    {
        private const int k = 14;
        private const int n = 30;
        private const int nk = 16;
        private const uint msb = 536870912;
        private UnsafeBuffer _onesCounter;
        private unsafe byte* _onesCounterPtr;
        private UnsafeBuffer _syndromesDecoder;
        private unsafe uint* _syndromesDecoderPtr;
        private UnsafeBuffer _hMatrixUint;
        private unsafe uint* _hMatrixUintPtr;
        private static readonly byte[,] gMatrix = new byte[14, 30]
        {
      {
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1
      }
        };
        private static readonly byte[,] hMatrix = new byte[30, 16]
        {
      {
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0
      },
      {
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 1
      },
      {
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 1,
        (byte) 1
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 1
      },
      {
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0
      },
      {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1
      }
        };

        // This code is Reed Mahler's decoder for RM (30,14)
        // The code is not universal, optimized for tetra
        // Recovers 3 bits out of 30 transmitted.
        public unsafe void Test()
        {
            var rnd = new Random();

            var inTestBuffer = UnsafeBuffer.Create(32, sizeof(byte));
            var inTestBufferPtr = (byte*)inTestBuffer;

            var outTestBuffer = UnsafeBuffer.Create(14, sizeof(byte));
            var outTestBufferPtr = (byte*)outTestBuffer;

            var result = (uint)0;
            var checkBit = (uint)0;

            var error = 0;

            // All possible options, 16K x 30 bits
            for (int j = 1; j < 16384; j++)
            {
                result = 0;

                for (int column = 0; column < 30; column++)
                {
                    checkBit = 0;

                    for (int row = 0; row < 14; row++)
                    {
                        checkBit ^= (uint)((j >> (13 - row)) & 1) & gMatrix[row, column];
                    }

                    result <<= 1;
                    result |= (uint)checkBit;
                }

                var data = result;

                for (int n = 0; n < 3; n++)
                {
                    var bitNum = rnd.Next(0, 29);
                    data ^= (1u << bitNum);

                }

                TetraUtils.UIntToBits(data, inTestBufferPtr, 2);

                Process(inTestBufferPtr, outTestBufferPtr);

                data = TetraUtils.BitsToUInt32(outTestBufferPtr, 0, 14);

                if ((result >> 16) != data)
                    error++;
            }
        }

        public unsafe void Init()
        {
            _syndromesDecoder = UnsafeBuffer.Create((1 << (nk)), sizeof(UInt32));
            _syndromesDecoderPtr = (UInt32*)_syndromesDecoder;

            _hMatrixUint = UnsafeBuffer.Create(nk, sizeof(UInt32));
            _hMatrixUintPtr = (UInt32*)_hMatrixUint;

            _onesCounter = UnsafeBuffer.Create(256, sizeof(byte));
            _onesCounterPtr = (byte*)_onesCounter;

            for (int i = 0; i < _syndromesDecoder.Length; i++) _syndromesDecoderPtr[i] = 0x0;

            int syndromeI = 0;

            for (int i1 = 0; i1 < n; i1++)
            {
                for (int i2 = i1 + 1; i2 < n; i2++)
                {
                    for (int i3 = i2 + 1; i3 < n; i3++)
                    {
                        // Ошибка в 3 битах
                        syndromeI = 0;

                        for (int ir = 0; ir < nk; ir++)
                        {
                            syndromeI <<= 1;
                            syndromeI += hMatrix[i1, ir] ^ hMatrix[i2, ir] ^ hMatrix[i3, ir];
                        }

                        _syndromesDecoderPtr[syndromeI] = (UInt32)((msb >> i1) | (msb >> i2) | (msb >> i3));
                    }

                    // Ошибка в 2 битах
                    syndromeI = 0;

                    for (int ir = 0; ir < nk; ir++)
                    {
                        syndromeI <<= 1;
                        syndromeI += hMatrix[i1, ir] ^ hMatrix[i2, ir];
                    }

                    _syndromesDecoderPtr[syndromeI] = (UInt32)((msb >> i1) | (msb >> i2));
                }

                // Ошибка в 1 бите
                syndromeI = 0;

                for (int ir = 0; ir < nk; ir++)
                {
                    syndromeI <<= 1;
                    syndromeI += hMatrix[i1, ir];
                }

                _syndromesDecoderPtr[syndromeI] = (UInt32)(msb >> i1);
            }

            //Таблица определения хэменигово расстояния
            //для байта
            for (uint j = 0; j < 256; j++)
            {
                var predBits = j;
                var counter = (uint)0;

                while (predBits != 0)
                {
                    counter += predBits & 1;
                    predBits >>= 1;
                }

                _onesCounterPtr[j] = (byte)counter;
            }

            //Перенос матрицы в UINT
            for (int j = 0; j < nk; j++)
            {
                _hMatrixUintPtr[j] = 0;

                for (int i = 0; i < n; i++)
                {
                    _hMatrixUintPtr[j] <<= 1;
                    _hMatrixUintPtr[j] |= (UInt32)hMatrix[i, j];
                }
            }

            //Test();
        }


        public unsafe bool Process(byte* inBuffer, byte* outBuffer)
        {
            uint alphaValue = 0;
            int counter = 0;

            //Принятое кодовое слово
            var vector = TetraUtils.BitsToUInt32(inBuffer, 0, n);

            int syndromeI = 0;

            for (int i = 0; i < nk; i++)
            {
                alphaValue = vector & _hMatrixUintPtr[i];

                counter = _onesCounterPtr[alphaValue & 0xff]
                    + _onesCounterPtr[(alphaValue >> 8) & 0xff]
                    + _onesCounterPtr[(alphaValue >> 16) & 0xff]
                    + _onesCounterPtr[(alphaValue >> 24) & 0xff];

                syndromeI <<= 1;
                syndromeI |= (counter & 1);
            }

            var noErrors = (syndromeI == 0);

            if (!noErrors)
            {
                var bitMask = _syndromesDecoderPtr[syndromeI];
                vector ^= bitMask;
                noErrors = bitMask != 0;
            }

            for (int i = 0; i < k; i++)
            {
                outBuffer[i] = (byte)((vector & msb) == 0 ? 0 : 1);
                vector <<= 1;
            }

            return noErrors;
        }
    }
}
