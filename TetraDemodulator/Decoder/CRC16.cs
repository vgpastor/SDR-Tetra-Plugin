using System;

namespace SDRSharp.Tetra
{
    unsafe class CRC16
    {
        private const int GenPoly = 0x1021;
        private const int GoodCRC = 0x1d0f;

        public int CalcBuffer(byte* buffer, int length)
        {
            int crc = 0xffff;
            UInt16 bit;

            for (int i = 0; i < length; i++)
            {
                bit = (UInt16)(buffer[i] & 0x1);

                crc ^= bit << 15;

                if ((crc & 0x8000) != 0)
                {
                    crc <<= 1;
                    crc ^= GenPoly;
                }
                else
                {
                    crc <<= 1;
                }
            }

            return crc & 0xffff;
        }

        public bool Process(byte* source, byte* dest, int sourceLength)
        {
            var crc = CalcBuffer(source, sourceLength);

            sourceLength -= 16;

            for (int i = 0; i < sourceLength; i++)
            {
                dest[i] = source[i];
            }

            return (crc == GoodCRC);
        }
    }
}
