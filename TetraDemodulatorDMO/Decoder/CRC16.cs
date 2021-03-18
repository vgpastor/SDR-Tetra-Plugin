using System;

namespace SDRSharp.Tetra
{
    unsafe class CRC16
    {
        private const int GenPoly = 0x8408;
        private const int GoodCRC = 0xF0B8;
        
        public int CalcBuffer(byte* buffer, int length)
        {
            int crc = 0xffff;
            UInt16 bit;

            for (int i = 0; i < length; i++)
            {
                bit = (UInt16)(buffer[i] ^ (crc & 0x1)) ;

                crc >>= 1;
                if (bit != 0) crc ^= GenPoly;
            }

            return crc;
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
