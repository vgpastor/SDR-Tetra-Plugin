using SDRSharp.Radio;
using System;

namespace SDRSharp.Tetra
{
    public enum BurstType
    {
        None,
        NDB1,
        NDB2,
        SYNC,
        WaitBurst,
    }
    public enum Mode
    {
        TMO,
        DMO,
    }
    public class Burst
    {
        public Mode Mode;
        public BurstType Type;
        public unsafe byte* Ptr;
        public int Length;
    }
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
        public const int bb_Length = bb1_Length + bb2_Length;
        public const int cb_Length = 84;
        public const int freqCorrection_Length = 80;
        public const int sb_Length = 120;

        public const int PossibleError = 2;

        private const float Pi = (float)Math.PI;
        private const float PiDivTwo = (float)(Math.PI / 2.0);
        private const float PiDivFor = (float)(Math.PI / 4.0);

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
            this._tempBuffer = UnsafeBuffer.Create(BurstLength * 2, sizeof(byte));
            this._tempBufferPtr = (byte*)(void*)this._tempBuffer;
            this._outBuffer = UnsafeBuffer.Create(BurstLength, sizeof(byte));
            this._outBufferPtr = (byte*)(void*)this._outBuffer;
        }

        public unsafe Burst ParseTrainingSequence(float* inBuffer, int length)
        {
            Burst burst = new Burst()
            {
                Type = BurstType.None,
                Length = BurstLength,
                Ptr = this._outBufferPtr
            };
            this.ConvertAngleToDiBits(burst.Ptr, inBuffer, length);
            return burst;
        }

        private unsafe void ConvertAngleToDiBits(byte* bitsBuffer, float* angles, int sourceLength)
        {
            /*
            delta > PiDivTwo    0 1
            delta > 0           0 0
            delta < 0           1 0
            delta < -PiDivTwo   1 1
             */
            while (sourceLength-- > 0)
            {
                float delta = *angles++;
                *bitsBuffer++ = (double)delta < 0.0 ? (byte)1 : (byte)0;
                *bitsBuffer++ = (double)Math.Abs(delta) > PiDivTwo ? (byte)1 : (byte)0;
            }
        }

        public unsafe void ExtractPhyChannels(
          Mode mode,
          Burst burst,
          byte* bbBuffer,
          byte* bkn1Buffer,
          byte* bkn2Buffer)
        {
            int offset = 2;
            switch (mode)
            {
                case Mode.TMO:
                    switch (burst.Type)
                    {
                        case BurstType.NDB1:
                        case BurstType.NDB2:
                            offset += nts3_pre_Length + phaseAdjust_Length;
                            BlockCopy(burst.Ptr, offset, bkn1Buffer, 0, bkn_Length);
                            offset += bkn_Length;
                            BlockCopy(burst.Ptr, offset, bbBuffer, 0, bb1_Length);
                            offset += bb1_Length + nts_Length;
                            BlockCopy(burst.Ptr, offset, bbBuffer, bb1_Length, bb2_Length);
                            offset += bb2_Length;
                            BlockCopy(burst.Ptr, offset, bkn2Buffer, 0, bkn_Length);
                            break;

                        case BurstType.SYNC:
                            offset += nts3_pre_Length + phaseAdjust_Length + freqCorrection_Length;
                            offset += sts_Length + sb_Length;
                            BlockCopy(burst.Ptr, offset, bbBuffer, 0, bb_Length);
                            offset += bb_Length;
                            BlockCopy(burst.Ptr, offset, bkn2Buffer, 0, bkn_Length);
                            break;

                        default:
                            break;
                    }
                    break;

                case Mode.DMO:
                    switch (burst.Type)
                    {
                        case BurstType.NDB1:
                        case BurstType.NDB2:
                            offset += nts3_pre_Length + phaseAdjust_Length;
                            BlockCopy(burst.Ptr, offset, bkn1Buffer, 0, bkn_Length);
                            offset += bkn_Length;
                            offset += nts_Length;
                            BlockCopy(burst.Ptr, offset, bkn2Buffer, 0, bkn_Length);
                            break;

                        case BurstType.SYNC:
                            offset += nts3_pre_Length + phaseAdjust_Length + freqCorrection_Length;
                            offset += sb_Length;
                            offset += sts_Length;
                            BlockCopy(burst.Ptr, offset, bkn2Buffer, 0, bkn_Length);
                            break;

                        default:
                            break;
                    }
                    break;
            }
        }

        public unsafe void ExtractSBChannels(Burst burst, byte* sb1Buffer)
        {
            var offset = 2;

            offset += nts3_pre_Length + phaseAdjust_Length + freqCorrection_Length;
            BlockCopy(burst.Ptr, offset, sb1Buffer, 0, sb_Length);
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
            {
                *dest++ = *source++;
            }
        }

        public void Dispose()
        {
        }
    }
}
