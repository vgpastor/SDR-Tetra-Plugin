using SDRSharp.Radio;
using System;

namespace SDRSharp.Tetra
{
    unsafe class Scrambler
    {
        public const uint DefaultScramblerInit = 3;
        private const uint ScramblerPoly = 0xDB710641;

        private UnsafeBuffer _onesCount;
        private byte* _onesCountPtr;

        public Scrambler()
        {
            _onesCount = UnsafeBuffer.Create(256, sizeof(byte));
            _onesCountPtr = (byte*)_onesCount;

            for (uint j = 0; j < 256; j++)
            {
                var predBits = j;
                var counter = (uint)0;

                while (predBits != 0)
                {
                    counter += predBits & 1;
                    predBits >>= 1;
                }

                _onesCountPtr[j] = (byte)counter;
            }

        }
        /* Descramble buffer */
        public void Process(byte* buffer, int length, uint scrambSequence)
        {
            uint bit = 0;
            uint key = 0;

            for (int i = 0; i < length; i++)
            {
                key = scrambSequence & ScramblerPoly;

                bit = (uint)(_onesCountPtr[(key >> 24) & 0xff] + _onesCountPtr[(key >> 16) & 0xff]
                    + _onesCountPtr[(key >> 8) & 0xff] + _onesCountPtr[key & 0xff]) & 0x1;

                scrambSequence = (scrambSequence >> 1) | (bit << 31);
                buffer[i] ^= (byte)bit;                
            }
        }
    }
}
