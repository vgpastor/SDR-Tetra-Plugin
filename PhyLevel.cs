// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.PhyLevel
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using SDRSharp.Radio;
using System;

namespace SDRSharp.Tetra
{
  public class PhyLevel : IDisposable
  {
    public const int BurstLength = 510;
    public const int preGuardPeriod_Length = 34;
    public const int postGuardPeriod_Length = 14;
    public const int tailBits_Length = 4;
    public const int nts_Length = 22;
    public const int nts3_pre_Length = 12;
    public const int nts3_post_Length = 10;
    public const int sts_Length = 38;
    public const int ets_Length = 30;
    public const int phaseAdjust_Length = 2;
    public const int bkn_Length = 216;
    public const int bb1_Length = 14;
    public const int bb2_Length = 16;
    public const int bb_Length = 30;
    public const int cb_Length = 84;
    public const int freqCorrection_Length = 80;
    public const int sb_Length = 120;
    public const int PossibleError = 2;
    private const float Pi = 3.141593f;
    private const float PiDivTwo = 1.570796f;
    private const float PiDivFor = 0.7853982f;
    private static readonly byte[] NormalTrainingSequence1 = new byte[22]
    {
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0
    };
    private static readonly byte[] NormalTrainingSequence2 = new byte[22]
    {
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
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
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0
    };
    private static readonly byte[] synchronizationTrainingSequence = new byte[38]
    {
      (byte) 1,
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
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
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
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1
    };
    private static readonly byte[] normalTrainingSequence3 = new byte[22]
    {
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
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
    };
    private static readonly byte[] extendedTrainingSequence = new byte[30]
    {
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1
    };
    private static readonly byte[] tailBits = new byte[4]
    {
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 0
    };
    private UnsafeBuffer _tempBuffer;
    private unsafe byte* _tempBufferPtr;
    private UnsafeBuffer _outBuffer;
    private unsafe byte* _outBufferPtr;

    public unsafe PhyLevel()
    {
      this._tempBuffer = UnsafeBuffer.Create(1020, 1);
      this._tempBufferPtr = (byte*) (void*) this._tempBuffer;
      this._outBuffer = UnsafeBuffer.Create(510, 1);
      this._outBufferPtr = (byte*) (void*) this._outBuffer;
    }

    public unsafe Burst ParseTrainingSequence(float* inBuffer, int length)
    {
      Burst burst = new Burst()
      {
        Type = BurstType.None,
        Length = 510,
        Ptr = this._outBufferPtr
      };
      this.ConvertAngleToDiBits(burst.Ptr, inBuffer, length);
      return burst;
    }

    private unsafe void ConvertAngleToDiBits(byte* bitsBuffer, float* angles, int sourceLength)
    {
      while (sourceLength-- > 0)
      {
        float num = *angles++;
        *bitsBuffer++ = (double) num < 0.0 ? (byte) 1 : (byte) 0;
        *bitsBuffer++ = (double) Math.Abs(num) > 1.57079637050629 ? (byte) 1 : (byte) 0;
      }
    }

    public unsafe void ExtractPhyChannels(
      Mode mode,
      Burst burst,
      byte* bbBuffer,
      byte* bkn1Buffer,
      byte* bkn2Buffer)
    {
      int num = 2;
      switch (mode)
      {
        case Mode.TMO:
          switch (burst.Type)
          {
            case BurstType.NDB1:
            case BurstType.NDB2:
              int sourceOffset1 = num + 14;
              this.BlockCopy(burst.Ptr, sourceOffset1, bkn1Buffer, 0, 216);
              int sourceOffset2 = sourceOffset1 + 216;
              this.BlockCopy(burst.Ptr, sourceOffset2, bbBuffer, 0, 14);
              int sourceOffset3 = sourceOffset2 + 36;
              this.BlockCopy(burst.Ptr, sourceOffset3, bbBuffer, 14, 16);
              int sourceOffset4 = sourceOffset3 + 16;
              this.BlockCopy(burst.Ptr, sourceOffset4, bkn2Buffer, 0, 216);
              return;
            case BurstType.SYNC:
              int sourceOffset5 = num + 94 + 158;
              this.BlockCopy(burst.Ptr, sourceOffset5, bbBuffer, 0, 30);
              int sourceOffset6 = sourceOffset5 + 30;
              this.BlockCopy(burst.Ptr, sourceOffset6, bkn2Buffer, 0, 216);
              return;
            default:
              return;
          }
        case Mode.DMO:
          switch (burst.Type)
          {
            case BurstType.NDB1:
            case BurstType.NDB2:
              int sourceOffset7 = num + 14;
              this.BlockCopy(burst.Ptr, sourceOffset7, bkn1Buffer, 0, 216);
              int sourceOffset8 = sourceOffset7 + 216 + 22;
              this.BlockCopy(burst.Ptr, sourceOffset8, bkn2Buffer, 0, 216);
              return;
            case BurstType.SYNC:
              int sourceOffset9 = num + 94 + 120 + 38;
              this.BlockCopy(burst.Ptr, sourceOffset9, bkn2Buffer, 0, 216);
              return;
            default:
              return;
          }
      }
    }

    public unsafe void ExtractSBChannels(Burst burst, byte* sb1Buffer)
    {
      int sourceOffset = 2 + 94;
      this.BlockCopy(burst.Ptr, sourceOffset, sb1Buffer, 0, 120);
    }

    private unsafe void BlockCopy(
      byte* source,
      int sourceOffset,
      byte* dest,
      int destOffset,
      int length)
    {
      source += sourceOffset;
      dest += destOffset;
      while (length-- > 0)
        *dest++ = *source++;
    }

    public void Dispose()
    {
    }
  }
}
