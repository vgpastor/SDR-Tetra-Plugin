using SDRSharp.Radio;

namespace SDRSharp.Tetra
{
    unsafe class MotherCode
    {

        private const int MotherCodeLength = 4;
        private const int TailLength = 4;
        private const int RegisterLength = 5;
        private const int RegisterStates = 1 << RegisterLength;
        private const int RegisterStatesMask = RegisterStates - 1;

        private const float BerAlphaCoeff = 0.99f;
        private const float BerBetaCoeff = 1 - BerAlphaCoeff;

        private UnsafeBuffer _metric = UnsafeBuffer.Create((512 + TailLength + 1) * RegisterStates, sizeof(float));
        private float* _metricPtr;
        private UnsafeBuffer _prev = UnsafeBuffer.Create((512 + TailLength + 1) * RegisterStates, sizeof(int));
        private int* _prevPtr;

        private float _ber;

        private sbyte[] _lutG0 = new sbyte[RegisterStates];// = new sbyte[] { -1, 1, -1, 1, -1, 1, -1, 1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, -1, 1, -1, 1, -1, 1, -1, 1 };
        private sbyte[] _lutG1 = new sbyte[RegisterStates];// = new sbyte[] { -1, 1, 1, -1, 1, -1, -1, 1, -1, 1, 1, -1, 1, -1, -1, 1, 1, -1, -1, 1, -1, 1, 1, -1, 1, -1, -1, 1, -1, 1, 1, -1 };
        private sbyte[] _lutG2 = new sbyte[RegisterStates];// = new sbyte[] { -1, 1, -1, 1, 1, -1, 1, -1, 1, -1, 1, -1, -1, 1, -1, 1, 1, -1, 1, -1, -1, 1, -1, 1, -1, 1, -1, 1, 1, -1, 1, -1 };
        private sbyte[] _lutG3 = new sbyte[RegisterStates];// = new sbyte[] { -1, 1, 1, -1, -1, 1, 1, -1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, -1, 1, 1, -1, -1, 1, 1, -1 };

        private float[] currentSum = new float[RegisterStates];

        private const int G0 = 0x19;
        private const int G1 = 0x17;
        private const int G2 = 0x1d;
        private const int G3 = 0x1b;
        
        /* Mother code according to Section 8.2.3.1.1 */
        public void DecoderInit()
        {
            _metricPtr = (float*)_metric;
            _prevPtr = (int*)_prev;

            for (int i = 0; i < RegisterStates; i++)
            {
                _lutG0[i] = Parity((i << 1) & G0);
                _lutG1[i] = Parity((i << 1) & G1);
                _lutG2[i] = Parity((i << 1) & G2);
                _lutG3[i] = Parity((i << 1) & G3);
            }
        }

        private sbyte Parity(int data)
        {
            int result = 0;

            while (data > 0)
            {
                result += data & 0x1;
                data >>= 1;
            }

            return (result & 0x1) == 0 ? (sbyte)1 : (sbyte)-1;
        }

        //Витерби декодер.
        public float BufferDecode(sbyte* source, byte* dest, int sourceLength)
        {
            int destLength = (sourceLength / MotherCodeLength) - TailLength;

            float g0;
            float g1;
            float g2;
            float g3;
        
            float ham;
            float metr1;
            float metr2;
            float maxMetric;

            int stateIndex;
            int prev = 0;

            FillMetrics();

            fixed (sbyte* lutG0Ptr = _lutG0, lutG1Ptr = _lutG1, lutG2Ptr = _lutG2, lutG3Ptr = _lutG3)
            {
                int sourceIndex = 0;

                for (int i = 0; i < destLength + TailLength; i++)
                {
                    stateIndex = i * RegisterStates;
                    
                    g0 = source[sourceIndex++];
                    g1 = source[sourceIndex++];
                    g2 = source[sourceIndex++];
                    g3 = source[sourceIndex++];

                    for (int j = 0; j < RegisterStates; j++)
                    {
                        ham = (lutG0Ptr[j] * g0);
                        ham += (lutG1Ptr[j] * g1);
                        ham += (lutG2Ptr[j] * g2);
                        ham += (lutG3Ptr[j] * g3);

                        prev = (j << 1) & RegisterStatesMask;

                        metr1 = _metricPtr[stateIndex + prev] + ham;
                        metr2 = _metricPtr[stateIndex + prev + 1] - ham;
                        _metricPtr[stateIndex + RegisterStates + j] = metr1 > metr2 ? metr1 : metr2;
                        _prevPtr[stateIndex + RegisterStates + j] = metr1 > metr2 ? prev : prev + 1;
                    }
                }
            }

            maxMetric = float.MinValue;

            stateIndex = (destLength + TailLength + 1) * RegisterStates;

            for (int i = 0; i < RegisterStates; i++)
            {
                currentSum[i] = _metricPtr[stateIndex + i];
                if (_metricPtr[stateIndex + i] > maxMetric)
                {
                    maxMetric = _metricPtr[stateIndex + i];
                    prev = _prevPtr[stateIndex + i];
                }
            }

            stateIndex = (destLength + TailLength + 1) * RegisterStates;

            for (int i = destLength - 1; i >= 0; i--)
            {
                dest[i] = (byte)(prev & 0x01);
                stateIndex -= RegisterStates;
                prev = _prevPtr[stateIndex + prev];
            }

            _ber = 100.0f - (maxMetric / (sourceLength * 0.375f)) * 100.0f;

            return _ber;
        }

        void FillMetrics()
        {
            for (int i = 0; i < RegisterStates; i++)
            {
                _metricPtr[i] = float.MinValue;
            }
            _metricPtr[0] = 0;
        }
    }
}
