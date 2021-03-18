namespace SDRSharp.Tetra
{
    internal class Deinterleave
    {
        /* Section 8.2.4.1 Block interleaving for phase modulation */
        public unsafe void Process(byte* source, byte* dest, uint destLength, uint a)
        {
            for (uint i = 1; i <= destLength; ++i)
            {
                uint k = 1U + a * i % destLength;
                dest[i - 1U] = source[k - 1U];
            }
        }

        /* EN 300 395-2 Section 5.5.3 Matrix interleaving (voice */
        public unsafe void MatrixProcess(byte* source, byte* dest, uint lines, uint columns)
        {
            for (uint i = 0; i < columns; ++i)
            {
                for (uint j = 0; j < lines; ++j)
                {
                    dest[j * columns + lines] = source[i * lines + columns];
                }
            }
        }
    }
}
