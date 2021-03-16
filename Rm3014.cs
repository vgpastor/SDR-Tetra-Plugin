// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.Rm3014
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

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

    public unsafe void Test()
    {
      Random random = new Random();
      byte* numPtr1 = (byte*) (void*) UnsafeBuffer.Create(32, 1);
      byte* numPtr2 = (byte*) (void*) UnsafeBuffer.Create(14, 1);
      int num1 = 0;
      for (int index1 = 1; index1 < 16384; ++index1)
      {
        uint num2 = 0;
        for (int index2 = 0; index2 < 30; ++index2)
        {
          uint num3 = 0;
          for (int index3 = 0; index3 < 14; ++index3)
            num3 ^= (uint) (index1 >> 13 - index3 & 1) & (uint) Rm3014.gMatrix[index3, index2];
          num2 = num2 << 1 | num3;
        }
        uint data = num2;
        for (int index2 = 0; index2 < 3; ++index2)
        {
          int num3 = random.Next(0, 29);
          data ^= (uint) (1 << num3);
        }
        TetraUtils.UIntToBits(data, numPtr1, 2);
        this.Process(numPtr1, numPtr2);
        uint uint32 = TetraUtils.BitsToUInt32(numPtr2, 0, 14);
        if ((int) (num2 >> 16) != (int) uint32)
          ++num1;
      }
    }

    public unsafe void Init()
    {
      this._syndromesDecoder = UnsafeBuffer.Create(65536, 4);
      this._syndromesDecoderPtr = (uint*) (void*) this._syndromesDecoder;
      this._hMatrixUint = UnsafeBuffer.Create(16, 4);
      this._hMatrixUintPtr = (uint*) (void*) this._hMatrixUint;
      this._onesCounter = UnsafeBuffer.Create(256, 1);
      this._onesCounterPtr = (byte*) (void*) this._onesCounter;
      for (int index = 0; index < this._syndromesDecoder.Length; ++index)
        this._syndromesDecoderPtr[index] = 0U;
      for (int index1 = 0; index1 < 30; ++index1)
      {
        for (int index2 = index1 + 1; index2 < 30; ++index2)
        {
          for (int index3 = index2 + 1; index3 < 30; ++index3)
          {
            int index4 = 0;
            for (int index5 = 0; index5 < 16; ++index5)
              index4 = (index4 << 1) + ((int) Rm3014.hMatrix[index1, index5] ^ (int) Rm3014.hMatrix[index2, index5] ^ (int) Rm3014.hMatrix[index3, index5]);
            this._syndromesDecoderPtr[index4] = 536870912U >> index1 | 536870912U >> index2 | 536870912U >> index3;
          }
          int index6 = 0;
          for (int index3 = 0; index3 < 16; ++index3)
            index6 = (index6 << 1) + ((int) Rm3014.hMatrix[index1, index3] ^ (int) Rm3014.hMatrix[index2, index3]);
          this._syndromesDecoderPtr[index6] = 536870912U >> index1 | 536870912U >> index2;
        }
        int index7 = 0;
        for (int index2 = 0; index2 < 16; ++index2)
          index7 = (index7 << 1) + (int) Rm3014.hMatrix[index1, index2];
        this._syndromesDecoderPtr[index7] = 536870912U >> index1;
      }
      for (uint index = 0; index < 256U; ++index)
      {
        uint num1 = index;
        uint num2 = 0;
        for (; num1 != 0U; num1 >>= 1)
          num2 += num1 & 1U;
        this._onesCounterPtr[index] = (byte) num2;
      }
      for (int index1 = 0; index1 < 16; ++index1)
      {
        this._hMatrixUintPtr[index1] = 0U;
        for (int index2 = 0; index2 < 30; ++index2)
        {
          uint* numPtr1 = this._hMatrixUintPtr + index1;
          *numPtr1 = *numPtr1 << 1;
          uint* numPtr2 = this._hMatrixUintPtr + index1;
          *numPtr2 = *numPtr2 | (uint) Rm3014.hMatrix[index2, index1];
        }
      }
    }

    public unsafe bool Process(byte* inBuffer, byte* outBuffer)
    {
      uint uint32 = TetraUtils.BitsToUInt32(inBuffer, 0, 30);
      int index1 = 0;
      for (int index2 = 0; index2 < 16; ++index2)
      {
        uint num1 = uint32 & this._hMatrixUintPtr[index2];
        int num2 = (int) this._onesCounterPtr[num1 & (uint) byte.MaxValue] + (int) this._onesCounterPtr[num1 >> 8 & (uint) byte.MaxValue] + (int) this._onesCounterPtr[num1 >> 16 & (uint) byte.MaxValue] + (int) this._onesCounterPtr[num1 >> 24 & (uint) byte.MaxValue];
        index1 = index1 << 1 | num2 & 1;
      }
      bool flag = index1 == 0;
      if (!flag)
      {
        uint num = this._syndromesDecoderPtr[index1];
        uint32 ^= num;
        flag = num > 0U;
      }
      for (int index2 = 0; index2 < 14; ++index2)
      {
        outBuffer[index2] = ((int) uint32 & 536870912) == 0 ? (byte) 0 : (byte) 1;
        uint32 <<= 1;
      }
      return flag;
    }
  }
}
