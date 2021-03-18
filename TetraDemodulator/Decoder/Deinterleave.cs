using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDRSharp.Tetra
{
    unsafe class Deinterleave
    {
        /* Section 8.2.4.1 Block interleaving for phase modulation */
        public void Process(byte* source, byte* dest, uint K, uint a)
        {
            uint k;

            for (uint i = 1; i <= K; i++)
            {
                k = 1 + ((a * i) % K);
                dest[i - 1] = source[k - 1];
            }
        }

        /* EN 300 395-2 Section 5.5.3 Matrix interleaving (voice */
        public void MatrixProcess(byte* source, byte* dest, uint lines, uint columns)
        {
            uint i, j;

            for (i = 0; i < columns; i++)
            {
                for (j = 0; j < lines; j++)
                {
                    dest[j * columns + lines] = source[i * lines + columns];
                }
            }
        }
    }
}
