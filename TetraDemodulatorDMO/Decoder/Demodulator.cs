using SDRSharp.Radio;
using System;
using System.Diagnostics;

namespace SDRSharp.Tetra
{
    unsafe class Demodulator
    {
        private const float Pi34 = (float)(Math.PI * 0.75f);
        private const float Pi14 = (float)(Math.PI * 0.25f);
        private const float Pi12 = (float)(Math.PI * 0.5f);
        private const float Pi2 = (float)(Math.PI * 2.0f);

        private static readonly float[] NormalTrainingSequence1 = new float[11] { -Pi34, Pi34, Pi14, Pi14, -Pi34, -Pi14, -Pi14, Pi14, -Pi34, Pi34, Pi14 };
        private static readonly float[] NormalTrainingSequence2 = new float[11] { Pi34, -Pi34, -Pi14, -Pi14, Pi34, Pi14, Pi14, -Pi34, Pi34, -Pi34, -Pi14 };
        private static readonly float[] SynchronizationTrainingSequence = new float[19] { -Pi34, Pi14, Pi14, Pi34, -Pi14, Pi34, -Pi34, Pi14, -Pi34, -Pi14, -Pi14, Pi34, -Pi34, Pi14, Pi14, Pi34, -Pi14, Pi34, -Pi34 };

        private static readonly float[] freqCorrectionFeild = new float[40] {  -Pi34, -Pi34, -Pi34, -Pi34,
                                                                               Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14,
                                                                               Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14,
                                                                               Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14,
                                                                               Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14,
                                                                               -Pi34, -Pi34, -Pi34, -Pi34};

        private static readonly float[] RRC = new float[33]
        {
            0.000510757983647636f, 0.003330067428578331f, 0.002393109708464180f, -0.002387287975579965f,
            -0.006363400592817924f, -0.003693818745887256f, 0.006403754994607879f, 0.016336093984136823f,
            0.014279885199357522f, -0.005518131891221855f, -0.033790811445214785f, -0.047158305460470728f,
            -0.021172494292841292f, 0.051717837981414049f, 0.151943364355247467f, 0.239281710120849311f,
            0.273901430540718138f,
            0.239281710120849311f, 0.151943364355247467f, 0.051717837981414049f, -0.021172494292841292f,
            -0.047158305460470728f, -0.033790811445214785f, -0.005518131891221855f, 0.014279885199357522f,
            0.016336093984136823f, 0.006403754994607879f, -0.003693818745887256f, -0.006363400592817924f,
            -0.002387287975579965f, 0.002393109708464180f, 0.003330067428578331f, 0.000510757983647636f
        };
        private const int NtsSquenceOffsetTMO = 122;
        private const int NtsSquenceOffsetDMO = 115;
        private const int StsSquenceOffset = 107;

        private const int SmallTrainingWindow = 6;

        private const int SyncLostValue = 8;

        private const int SamplesPerSymbol = 4;
        private const double SymbolRate = 18000.0;
        private const double Samplerate = SymbolRate * SamplesPerSymbol;
        private const int SamplesPerBurst = 255 * SamplesPerSymbol;

        private UnsafeBuffer _tempBuffer;
        private Complex* _tempBufferPtr;
        private IQFirFilter _rrcFilter;
        private UnsafeBuffer _nts1Buffer;
        private unsafe Complex* _nts1BufferPtr;
        private UnsafeBuffer _nts2Buffer;
        private unsafe Complex* _nts2BufferPtr;
        private UnsafeBuffer _stsBuffer;
        private unsafe Complex* _stsBufferPtr;
        private int _writeAddress;
        private int _offset;
        private IQFirFilter _fsFilter;
        private int _syncCounter;

        private int _ntsOffset;
        private int _stsOffset;

        private float _ndb1minSum;
        private int _ndb1Index;
        private float _ndb2minSum;
        private int _ndb2Index;
        private float _stsMinSum;
        private int _stsIndex;
        private int _trainingWindow;
        private float _ndb1sum;
        private float _ndb2sum;
        private float _stsSum;

        private Complex _sample;
        private Complex _training;
        private Complex _lastData;
        private float _gain = 1.0f;

        public void ProcessBuffer(Burst burst, Complex* iqBuffer, float* digitalBuffer)
        {
            burst.Type = BurstType.None;
            burst.Length = 0;

            #region Buffers and filters

            if (_tempBuffer == null)
            {
                _writeAddress = 0;

                _syncCounter = 0;

                _tempBuffer = UnsafeBuffer.Create((int)Samplerate, sizeof(Complex));
                _tempBufferPtr = (Complex*)_tempBuffer;

                //var coeff = FilterBuilder.MakeSinc(Samplerate, SymbolRate, 5);
                _rrcFilter = new IQFirFilter(RRC);
                _fsFilter = new IQFirFilter(RRC);

                CreateFrameSynchronization();
            }

            #endregion

            #region Interpolator and Matched filter
       
            _rrcFilter.Process(iqBuffer, SamplesPerBurst);

            #endregion

            if (_writeAddress + SamplesPerBurst < _tempBuffer.Length)
            {
                for (int i = 0; i < SamplesPerBurst; i++)
                {
                    _tempBufferPtr[_writeAddress] = iqBuffer[i];// * _gain;
                    _gain = _gain * 0.99f + (0.27f / iqBuffer[i].Real) * 0.01f;

                    _writeAddress++;
                }
            }
            else
            {
                Debug.WriteLine(" Dropped_buffer");
            }

            if (_writeAddress < SamplesPerBurst * 2)
            {
                burst.Type = BurstType.WaitBurst;
                return;
            }

            _ntsOffset = burst.Mode == Mode.TMO ? (NtsSquenceOffsetTMO * SamplesPerSymbol) : (NtsSquenceOffsetDMO * SamplesPerSymbol);
            _stsOffset = StsSquenceOffset * SamplesPerSymbol;

            _ndb1minSum = float.MinValue;
            _ndb1Index = 0;
            _ndb2minSum = float.MinValue;
            _ndb2Index = 0;
            _stsMinSum = float.MinValue;
            _stsIndex = 0;

            int ntsLength = NormalTrainingSequence1.Length * SamplesPerSymbol;
            int stsLength = SynchronizationTrainingSequence.Length * SamplesPerSymbol;

            //Find training squence
            _trainingWindow = (_syncCounter > 0) ? (SmallTrainingWindow * SamplesPerSymbol) : SamplesPerBurst;
            if (_syncCounter > 0) _syncCounter--;

            for (int i = 0; i < _trainingWindow; i++)
            {
                _ndb1sum = 0; _ndb2sum = 0; _stsSum = 0;
                int ntsWindow = i + _ntsOffset;
                int stsWindow = i + _stsOffset;

                for (int j = 0; j < ntsLength; j++)
                {
                    _sample = _tempBufferPtr[ntsWindow + j];
                    _training = _nts1BufferPtr[j].Conjugate();
                    _ndb1sum += (_training * _sample).ModulusSquared();
                }
                if (_ndb1sum > _ndb1minSum)
                {
                    _ndb1minSum = _ndb1sum;
                    _ndb1Index = i;
                }

                for (int j = 0; j < ntsLength; j++)
                {
                    _sample = _tempBufferPtr[ntsWindow + j];
                    _training = _nts2BufferPtr[j].Conjugate();
                    _ndb2sum += (_training * _sample).ModulusSquared();
                }
                if (_ndb2sum > _ndb2minSum)
                {
                    _ndb2minSum = _ndb2sum;
                    _ndb2Index = i;
                }

                for (int j = 0; j < stsLength; j++)
                {
                    _sample = _tempBufferPtr[stsWindow + j];
                    _training = _stsBufferPtr[j].Conjugate();
                    _stsSum += (_training * _sample).ModulusSquared();
                }
                if (_stsSum > _stsMinSum)
                {
                    _stsMinSum = _stsSum;
                    _stsIndex = i;
                }
            }

            _ndb1minSum /= ntsLength;
            _ndb2minSum /= ntsLength;
            _stsMinSum /= stsLength;

            if (_ndb1minSum > 0|| _ndb2minSum > 0 || _stsMinSum > 0)
            {
                if (_ndb1minSum > _ndb2minSum && _ndb1minSum > _stsMinSum)
                {
                    _offset = _ndb1Index;
                    burst.Type = BurstType.NDB1;
                }
                else if (_ndb2minSum > _ndb1minSum && _ndb2minSum > _stsMinSum)
                {
                    _offset = _ndb2Index;
                    burst.Type = BurstType.NDB2;
                }
                else
                {
                    _offset = _stsIndex;
                    burst.Type = BurstType.SYNC;
                }
                _syncCounter = SyncLostValue;
            }
            else
            {
                burst.Type = BurstType.None;
                _offset = SamplesPerSymbol;
            }

            Debug.WriteLine(String.Format(" Offset-{0} ndb1-{1} ndb2-{2} sts-{3}", _offset, _ndb1minSum, _ndb2minSum, _stsMinSum));

            #region Syncronization

            #endregion

            #region resampler

            int ind = 0;

            while (ind < 256)
            {
                digitalBuffer[ind] =  (_tempBufferPtr[_offset] * _lastData.Conjugate()).ArgumentFast();
                _lastData = _tempBufferPtr[_offset];
                _offset += SamplesPerSymbol;
                ind++;
            }

            _offset -= SamplesPerSymbol;
            
            if (_offset < 0) _offset = 0;

            _writeAddress -= _offset;
            if (_writeAddress < 0) _writeAddress = 0;

            Utils.Memcpy(_tempBufferPtr, _tempBufferPtr + _offset, _writeAddress * sizeof(Complex));

            #endregion

            AngleToSymbol(burst.Ptr, digitalBuffer, 255);
            burst.Length = 510;
        }


        private void CreateFrameSynchronization()
        {

            CreateFsBuffers();

            int filterOffset = (int)(RRC.Length * 0.5f + 2);

            float prevSymb = Pi14;
            float angel;

            int index = filterOffset;
            for (int i = 0; i < NormalTrainingSequence1.Length; i++)
            {
                angel = SymbolToAngel(NormalTrainingSequence1[i], prevSymb);
                prevSymb = angel;

                _nts1BufferPtr[index] = Complex.FromAngleFast(angel);
                index += SamplesPerSymbol;
            }

            index = filterOffset;
            prevSymb = Pi14;
            for (int i = 0; i < NormalTrainingSequence1.Length; i++)
            {
                angel = SymbolToAngel(NormalTrainingSequence2[i], prevSymb);
                prevSymb = angel;

                _nts2BufferPtr[index] = Complex.FromAngleFast(angel);
                index += SamplesPerSymbol;
            }

            index = filterOffset;
            prevSymb = Pi14;
            for (int i = 0; i < SynchronizationTrainingSequence.Length; i++)
            {
                angel = SymbolToAngel(SynchronizationTrainingSequence[i], prevSymb);
                prevSymb = angel;

                _stsBufferPtr[index] = Complex.FromAngleFast(angel);
                index += SamplesPerSymbol;
            }

            _fsFilter.Process(_nts1BufferPtr, _nts1Buffer.Length);
            _fsFilter.Process(_nts2BufferPtr, _nts2Buffer.Length);
            _fsFilter.Process(_stsBufferPtr, _stsBuffer.Length);

            Utils.Memcpy(_nts1BufferPtr, _nts1BufferPtr + filterOffset - 2, NormalTrainingSequence1.Length * SamplesPerSymbol * sizeof(Complex));
            Utils.Memcpy(_nts2BufferPtr, _nts2BufferPtr + filterOffset - 2, NormalTrainingSequence2.Length * SamplesPerSymbol * sizeof(Complex));
            Utils.Memcpy(_stsBufferPtr, _stsBufferPtr + filterOffset - 2, SynchronizationTrainingSequence.Length * SamplesPerSymbol * sizeof(Complex));
        }

        private void CreateFsBuffers()
        {
            _nts1Buffer?.Dispose();
            _nts2Buffer?.Dispose();
            _stsBuffer?.Dispose();

            var ntsLength = SamplesPerSymbol * NormalTrainingSequence1.Length + RRC.Length;
            var stsLength = SamplesPerSymbol * SynchronizationTrainingSequence.Length + RRC.Length;

            _nts1Buffer = UnsafeBuffer.Create(ntsLength, sizeof(Complex));
            _nts1BufferPtr = (Complex*)_nts1Buffer;
            _nts2Buffer = UnsafeBuffer.Create(ntsLength, sizeof(Complex));
            _nts2BufferPtr = (Complex*)_nts2Buffer;
            _stsBuffer = UnsafeBuffer.Create(stsLength, sizeof(Complex));
            _stsBufferPtr = (Complex*)_stsBuffer;

            for (int i = 0; i < ntsLength; i++)
            {
                _nts1BufferPtr[i] = 0;
                _nts2BufferPtr[i] = 0;
            }
            for (int i = 0; i < stsLength; i++)
            {
                _stsBufferPtr[i] = 0;
            }
        }

        private float SymbolToAngel(float symbol, float prevSymb)
        {
            float res = (prevSymb + symbol);
            if (res > Pi2) res -= Pi2;
            if (res < 0) res += Pi2;
            return res;
        }

        private void AngleToSymbol(byte* bitsBuffer, float* angles, int sourceLength)
        {
            float delta;

            while (sourceLength-- > 0)
            {
                delta = *angles++;

                *bitsBuffer++ = delta < 0 ? (byte)1 : (byte)0;
                *bitsBuffer++ = Math.Abs(delta) > Pi12 ? (byte)1 : (byte)0;
            }
        }
    }
}

