namespace SDRSharp.Tetra
{
    internal class CRC16
    {
        private const int GenPoly = 33800;
        private const int GoodCRC = 61624;

        public unsafe int CalcBuffer(byte* buffer, int length)
        {
            int crc = (int)ushort.MaxValue;
            for (int index = 0; index < length; ++index)
            {
                int num = (int)(ushort)((uint)buffer[index] ^ (uint)(crc & 1));
                crc >>= 1;
                if (num != 0)
                    crc ^= 33800;
            }
            return crc;
        }

        public unsafe bool Process(byte* source, byte* dest, int sourceLength)
        {
            int crc = this.CalcBuffer(source, sourceLength);
            sourceLength -= 16;
            for (int i = 0; i < sourceLength; ++i)
            {
                dest[i] = source[i];
            }
            return crc == GoodCRC;
        }
    }
}
