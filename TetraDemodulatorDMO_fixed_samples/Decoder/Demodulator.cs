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

        private static readonly byte[] NormalTrainingSequence1 = new byte[22] { 1, 1, 0, 1, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 1, 0, 0 };
        private static readonly byte[] NormalTrainingSequence2 = new byte[22] { 0, 1, 1, 1, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1, 1, 0 };
        private static readonly byte[] SynchronizationTrainingSequence = new byte[38] { 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1 };

        private static readonly float[] freqCorrectionFeild = new float[40] {  -Pi34, -Pi34, -Pi34, -Pi34,
                                                                               Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14,
                                                                               Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14,
                                                                               Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14,
                                                                               Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14, Pi14,
                                                                               -Pi34, -Pi34, -Pi34, -Pi34};
        private const int NtsSquenceOffsetTMO = 122;
        private const int NtsSquenceOffsetDMO = 115;
        private const int StsSquenceOffset = 107;

        private const int SmallTrainingWindow = 6;
        private const int BuffersOverlap = 2;

        private const int SyncLostValue = 8;

        private const double SymbolRate = 18000.0;

        private UnsafeBuffer _buffer;
        private Complex* _bufferPtr;

        private UnsafeBuffer _tempBuffer;
        private float* _tempBufferPtr;

        private int _filterLength;
        private IQFirFilter _matchedFilter;
        private double _samplerateIn;
        private double _samplerate;
        private int _length;
        private int _interpolation;
        private UnsafeBuffer _nts1Buffer;
        private unsafe float* _nts1BufferPtr;
        private UnsafeBuffer _nts2Buffer;
        private unsafe float* _nts2BufferPtr;
        private UnsafeBuffer _stsBuffer;
        private unsafe float* _stsBufferPtr;
        private int _writeAddress;
        private double _symbolLength;
        private int _windowLength;
        private int _offset;
        private FirFilter _fsFilter;
        private int _tailBufferLength;
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

        private float _sample;
        private float _training;

        public void ProcessBuffer(Burst burst, Complex* iqBuffer, double iqSamplerate, int iqBufferLength, float* digitalBuffer)
        {
            burst.Type = BurstType.WaitBurst;
            burst.Length = 0;

            #region Buffers and filters

            if (_buffer == null || iqSamplerate != _samplerateIn)
            {
                _samplerateIn = iqSamplerate;
                _samplerate = _samplerateIn;

                _interpolation = 1;
                while (_samplerateIn * _interpolation < SymbolRate * 5)
                {
                    _interpolation += 1;
                }

                _samplerate = _samplerateIn * _interpolation;
                _length = iqBufferLength * _interpolation;

                _symbolLength = (_samplerate / SymbolRate);
                _windowLength = (int)Math.Round(_symbolLength * 255);
                _writeAddress = 0;

                _tailBufferLength = (int)Math.Round(_symbolLength);

                _syncCounter = 0;

                _buffer = UnsafeBuffer.Create(_length + _tailBufferLength, sizeof(Complex));
                _bufferPtr = (Complex*)_buffer;

                _tempBuffer = UnsafeBuffer.Create((int)_samplerate, sizeof(float));
                _tempBufferPtr = (float*)_tempBuffer;

                _filterLength = Math.Max((int)((_samplerate * 6) / SymbolRate) | 1, 5);
                var coeff = FilterBuilder.MakeSinc(_samplerate, SymbolRate, _filterLength);
                _matchedFilter = new IQFirFilter(coeff);
                coeff = FilterBuilder.MakeLowPassKernel(_samplerate,_filterLength, 25000, WindowType.BlackmanHarris4);
                _fsFilter = new FirFilter(coeff);

                CreateFrameSynchronization();
            }

            #endregion

            #region Interpolator and Matched filter

            if (_interpolation > 1)
            {
                //interpolator
                var index = 0;
                var nullComplex = new Complex(0, 0);

                for (int i = 0; i < _length; i++)
                {
                    _bufferPtr[i + _tailBufferLength] = ((i % _interpolation) == 0) ? iqBuffer[index++] : nullComplex;
                }
            }
            else
            {
                Utils.Memcpy(_bufferPtr + _tailBufferLength, iqBuffer, _length * sizeof(Complex));
            }

            _matchedFilter.Process(_bufferPtr + _tailBufferLength, _length);

            #endregion

            #region Syncronization
            //Demodulate buffer
            if (_writeAddress + _length < _tempBuffer.Length)
            {
                for (int i = 0; i < _length; i++)
                {
                    _tempBufferPtr[_writeAddress] = (_bufferPtr[i + _tailBufferLength] * _bufferPtr[i].Conjugate()).ArgumentFast();
                    _writeAddress++;
                }
            }
            else
            {
                Debug.WriteLine(" Dropped_buffer");
            }

            //Copy tail symbols
            for (int i = 0; i < _tailBufferLength; i++)
            {
                _bufferPtr[i] = _bufferPtr[_length + i];
            }

            if (_writeAddress < _windowLength * 2)
                return;

            _ntsOffset = (int)(burst.Mode == Mode.TMO ? Math.Round(NtsSquenceOffsetTMO * _symbolLength) : Math.Round(NtsSquenceOffsetDMO * _symbolLength));
            _stsOffset = (int)Math.Round(StsSquenceOffset * _symbolLength);

            _ndb1minSum = float.MinValue;
            _ndb1Index = 0;
            _ndb2minSum = float.MinValue;
            _ndb2Index = 0;
            _stsMinSum = float.MinValue;
            _stsIndex = 0;

            //Find training squence
            _trainingWindow = (_syncCounter > 0) ? (SmallTrainingWindow * _tailBufferLength) : _windowLength;
            if (_syncCounter > 0) _syncCounter--;

            for (int i = 0; i < _trainingWindow; i++)
            {
                _ndb1sum = 0; _ndb2sum = 0; _stsSum = 0;

                for (int j = 0; j < _nts1Buffer.Length; j++)
                {
                    _sample = _tempBufferPtr[i + j + _ntsOffset];
                    _training = _nts1BufferPtr[j];
                    _ndb1sum += (_training * _sample);// * (_training - _sample);
                }
                if (_ndb1sum > _ndb1minSum)
                {
                    _ndb1minSum = _ndb1sum;
                    _ndb1Index = i;
                }

                for (int j = 0; j < _nts2Buffer.Length; j++)
                {
                    _sample = _tempBufferPtr[i + j + _ntsOffset];
                    _training = _nts2BufferPtr[j];
                    _ndb2sum += (_training * _sample);// * (_training - _sample);
                }
                if (_ndb2sum > _ndb2minSum)
                {
                    _ndb2minSum = _ndb2sum;
                    _ndb2Index = i;
                }

                for (int j = 0; j < _stsBuffer.Length; j++)
                {
                    _sample = _tempBufferPtr[i + j + _stsOffset];
                    _training = _stsBufferPtr[j];
                    _stsSum += (_training * _sample);// * (_training - _sample);
                }
                if (_stsSum > _stsMinSum)
                {
                    _stsMinSum = _stsSum;
                    _stsIndex = i;
                }
            }

            _ndb1minSum /= NormalTrainingSequence1.Length;
            _ndb2minSum /= NormalTrainingSequence2.Length;
            _stsMinSum /= SynchronizationTrainingSequence.Length;

            //if (_ndb1minSum > 1.0 || _ndb2minSum > 1.0 || _stsMinSum > 1.0)
            //{
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
            //}
            //else
            //{
             //   burst.Type = BurstType.None;
             //   _offset = _tailBufferLength * BuffersOverlap;
            //}

            Debug.WriteLine(String.Format(" Offset-{0} ndb1-{1} ndb2-{2} sts-{3}", _offset, _ndb1minSum, _ndb2minSum, _stsMinSum));

            #endregion

            #region resampler

            int ind = 0;
            int shift = 0;

            while (ind < 256)
            {
                shift = (int)Math.Round(ind * _symbolLength);
                digitalBuffer[ind] = _tempBufferPtr[_offset + shift];
                ind++;
            }

            _offset += shift;
            _offset -= _tailBufferLength * BuffersOverlap;
            if (_offset < 0) _offset = 0;

            _writeAddress -= _offset;
            if (_writeAddress < 0) _writeAddress = 0;

            Utils.Memcpy(_tempBufferPtr, _tempBufferPtr + _offset, _writeAddress * sizeof(float));

            #endregion

            AngleToSymbol(burst.Ptr, digitalBuffer, 255);
            burst.Length = 510;
        }


        private void CreateFrameSynchronization()
        {
            var symbolLength = (_samplerate / SymbolRate);


            CreateFsBuffers(symbolLength);

            var currentSymbol = 0;

            for (int i = 0; i < _nts1Buffer.Length; i++)
            {
                if ((i + symbolLength * 0.5) % symbolLength < 1.0f)
                {
                    _nts1BufferPtr[i] = SymbolToAngel(NormalTrainingSequence1, currentSymbol);
                    currentSymbol++;
                }
                else
                {
                    _nts1BufferPtr[i] = 0.0f;
                }
            }

            currentSymbol = 0;

            for (int i = 0; i < _nts2Buffer.Length; i++)
            {
                if ((i + symbolLength * 0.5) % symbolLength < 1.0f)
                {
                    _nts2BufferPtr[i] = SymbolToAngel(NormalTrainingSequence2, currentSymbol);
                    currentSymbol++;
                }
                else
                {
                    _nts2BufferPtr[i] = 0.0f;
                }
            }

            currentSymbol = 0;

            for (int i = 0; i < _stsBuffer.Length; i++)
            {
                if ((i + symbolLength * 0.5) % symbolLength < 1.0f)
                {
                    _stsBufferPtr[i] = SymbolToAngel(SynchronizationTrainingSequence, currentSymbol);
                    currentSymbol++;
                }
                else
                {
                    _stsBufferPtr[i] = 0.0f;
                }
            }

            _fsFilter.Process(_nts1BufferPtr, _nts1Buffer.Length);
            _fsFilter.Process(_nts2BufferPtr, _nts2Buffer.Length);
            _fsFilter.Process(_stsBufferPtr, _stsBuffer.Length);

            var testBuffer = new float[_stsBuffer.Length];
            for (int i = 0; i < _stsBuffer.Length; i++)
            {
                testBuffer[i] = _stsBufferPtr[i];
            }

            return;
        }

        private void CreateFsBuffers(double symbolLength)
        {
            _nts1Buffer?.Dispose();
            _nts2Buffer?.Dispose();
            _stsBuffer?.Dispose();

            var ntsLength = (int)(symbolLength * (NormalTrainingSequence1.Length * 0.5)) | 1;
            var stsLength = (int)(symbolLength * (SynchronizationTrainingSequence.Length * 0.5)) | 1;

            _nts1Buffer = UnsafeBuffer.Create(ntsLength, sizeof(float));
            _nts1BufferPtr = (float*)_nts1Buffer;
            _nts2Buffer = UnsafeBuffer.Create(ntsLength, sizeof(float));
            _nts2BufferPtr = (float*)_nts2Buffer;
            _stsBuffer = UnsafeBuffer.Create(stsLength, sizeof(float));
            _stsBufferPtr = (float*)_stsBuffer;
        }

        private float SymbolToAngel(byte[] trainingSquence, int symbolIndex)
        {
            float result = 0.0f;

            result = trainingSquence[(symbolIndex * 2) + 1] == 1 ? Pi34 : Pi14;
            result = trainingSquence[symbolIndex * 2] == 1 ? -result : result;

            return result;
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

        public float FrequencyError { get; set; }
    }
}

