using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDRSharp.Tetra
{
    public enum BurstType
    {
        None = 0,
        NDB1,
        NDB2,
        SYNC
    }

    public unsafe class Burst
    {
        public BurstType Type;
        public byte* Ptr;
        public int Length;
    }

    public unsafe class PhyLevel : IDisposable
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

        private static readonly byte[] NormalTrainingSequence1 = new byte[22] { 1, 1, 0, 1, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 1, 0, 0 };
        private static readonly byte[] NormalTrainingSequence2 = new byte[22] { 0, 1, 1, 1, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1, 1, 0 };
        private static readonly byte[] normalTrainingSequence3 = new byte[22] { 1, 0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1 };
        private static readonly byte[] synchronizationTrainingSequence = new byte[38] { 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1 };
        
        private static readonly byte[] extendedTrainingSequence = new byte[30] { 1, 0, 0, 1, 1, 1, 0, 1, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 1, 0, 0, 0, 0, 1, 1 };
        private static readonly byte[] tailBits = new byte[4] { 1, 1, 0, 0 };
        
        private UnsafeBuffer _tempBuffer;
        private byte* _tempBufferPtr;

        private UnsafeBuffer _outBuffer;
        private byte* _outBufferPtr;
        private int _nextWindow;

        public PhyLevel()
        {
            _tempBuffer = UnsafeBuffer.Create(BurstLength * 2, sizeof(byte));
            _tempBufferPtr = (byte*)_tempBuffer;

            _outBuffer = UnsafeBuffer.Create(BurstLength, sizeof(byte));
            _outBufferPtr = (byte*) _outBuffer;

            _nextWindow = 0;
        }

        public Burst ParseTrainingSequence(float* inBuffer, int length)
        {
            var result = new Burst
            {
                Type = BurstType.None,
                Length = BurstLength,
                Ptr = _outBufferPtr
            };

            var burstWindowStartIndex = 0;
            var burstWindowEndIndex = burstWindowStartIndex + BurstLength;
            
            var nts1Index = 0;
            var nts2Index = 0;
            var stsIndex = 0;

            var ntsOffset = 0;
            var stsOffset = 0;

            var ntsSquenceOffset = nts3_pre_Length + phaseAdjust_Length + bkn_Length + bb1_Length;
            var stsSquenceOffset = nts3_pre_Length + phaseAdjust_Length + freqCorrection_Length + sb_Length;

            var trainingSquenceFound = false;

            Utils.Memcpy(_tempBufferPtr, _tempBufferPtr + BurstLength, BurstLength);

            ConvertAngleToDiBits(_tempBufferPtr + BurstLength, inBuffer, length);

            var currentIndex = _nextWindow;
            var xorSum = 0;

            while (currentIndex < BurstLength)
            {
                ntsOffset = currentIndex + ntsSquenceOffset;
                stsOffset = currentIndex + stsSquenceOffset;
                
                nts1Index = 0;
                nts2Index = 0;
                stsIndex = 0;

                trainingSquenceFound = false;
                
                if (!trainingSquenceFound)
                {
                    xorSum = 0;
                    result.Type = BurstType.NDB1;

                    while (nts1Index < nts_Length && xorSum <= PossibleError)
                    {
                        xorSum += _tempBufferPtr[ntsOffset + nts1Index] ^ NormalTrainingSequence1[nts1Index];
                        nts1Index++;
                    }
                    trainingSquenceFound = xorSum <= PossibleError;
                }

                if (!trainingSquenceFound)
                {
                    xorSum = 0;
                    result.Type = BurstType.NDB2;

                    while (nts2Index < nts_Length && xorSum <= PossibleError)
                    {
                        xorSum += _tempBufferPtr[ntsOffset + nts2Index] ^ NormalTrainingSequence2[nts2Index];
                        nts2Index++;
                    }
                    trainingSquenceFound = xorSum <= PossibleError;
                }

                if (!trainingSquenceFound)
                {
                    xorSum = 0;
                    result.Type = BurstType.SYNC;

                    while (stsIndex < sts_Length && xorSum <= PossibleError)
                    {
                        xorSum += _tempBufferPtr[stsOffset + stsIndex] ^ synchronizationTrainingSequence[stsIndex];
                        stsIndex++;
                    }
                    trainingSquenceFound = xorSum <= PossibleError;
                }

                if (!trainingSquenceFound)
                {
                    currentIndex += 2;
                    continue;
                }
                else
                {
                    burstWindowStartIndex = currentIndex;
                    burstWindowEndIndex = currentIndex + BurstLength;
                    break;
                }
            }

            Utils.Memcpy(result.Ptr, _tempBufferPtr + burstWindowStartIndex, BurstLength);

            if (!trainingSquenceFound)
            {
                result.Type = BurstType.None;
                _nextWindow = 0;
            }
            else
            {
                _nextWindow = Math.Max( 0, burstWindowStartIndex - 4);
            }

            return result;
        }

        private void ConvertAngleToDiBits(byte* bitsBuffer, float* angles, int sourceLength)
        {
            float delta;

            /*
            delta > PiDivTwo    0 1
            delta > 0           0 0
            delta < 0           1 0
            delta < -PiDivTwo   1 1
             */

            while (sourceLength-- > 0)
            {
                delta = *angles++;

                *bitsBuffer++ = delta < 0 ? (byte)1 : (byte)0;
                *bitsBuffer++ = Math.Abs(delta) > PiDivTwo ? (byte)1 : (byte)0;
            }
        }

        public void ExtractPhyChannels(Burst burst, byte* bbBuffer, byte* bkn1Buffer, byte* bkn2Buffer, byte* sb1Buffer)
        {
            var offset = 0;

            switch (burst.Type)
            {
                case BurstType.NDB1:
                case BurstType.NDB2:
                    offset = nts3_pre_Length + phaseAdjust_Length;
                    BlockCopy(burst.Ptr, offset, bkn1Buffer, 0, bkn_Length);
                    offset += bkn_Length;
                    BlockCopy(burst.Ptr, offset, bbBuffer, 0, bb1_Length);
                    offset += bb1_Length + nts_Length;
                    BlockCopy(burst.Ptr, offset, bbBuffer, bb1_Length, bb2_Length);
                    offset += bb2_Length;
                    BlockCopy(burst.Ptr, offset, bkn2Buffer, 0, bkn_Length);
                    break;

                case BurstType.SYNC:
                    offset = nts3_pre_Length + phaseAdjust_Length + freqCorrection_Length;
                    BlockCopy(burst.Ptr, offset, sb1Buffer, 0, sb_Length);
                    offset += sts_Length + sb_Length;
                    BlockCopy(burst.Ptr, offset, bbBuffer, 0, bb_Length);
                    offset += bb_Length;
                    BlockCopy(burst.Ptr, offset, bkn2Buffer, 0, bkn_Length);
                    break;

                default:
                    break;
            }
        }

        private void BlockCopy(byte* source, int sourceOffset, byte* dest, int destOffset, int length)
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
