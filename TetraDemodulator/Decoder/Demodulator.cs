using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SDRSharp.Tetra
{
    unsafe class Demodulator
    {
        private const double SymbolRate = 18000.0;
 
        private IirFilter _syncFilter = new IirFilter();
        
        private UnsafeBuffer _buffer;
        private Complex* _bufferPtr;

        private int _filterLength;
        private IQFirFilter _matchedFilter;
        private double _samplerateIn;
        private double _samplerate;
        private int _length;
        private int _interpolation;
        private Complex _lastSymbol;
        private Complex _lastData;
        private float _lastOsc;
        private float _lastSyncValue;

        public int ProcessBuffer(Complex* iqBuffer, double iqSamplerate, int iqBufferLength, float* digitalBuffer)
        {
            var indexOut = 0;
 
            #region Buffers and filters

            if (_buffer == null || iqSamplerate != _samplerateIn)
            {
                _samplerateIn = iqSamplerate;
                _samplerate = _samplerateIn;

                _interpolation = 1;
                while (_samplerateIn * _interpolation < SymbolRate * 4)
                {
                    _interpolation *= 2;
                }

                _samplerate = _samplerateIn * _interpolation;
                _length = iqBufferLength * _interpolation;
                
                _buffer = UnsafeBuffer.Create(_length, sizeof(Complex));
                _bufferPtr = (Complex*)_buffer;

                _filterLength = Math.Max((int)(_samplerate / SymbolRate) | 1, 5);
                var coeff = FilterBuilder.MakeSinc(_samplerate, SymbolRate, _filterLength);
                _matchedFilter = new IQFirFilter(coeff, 1);
                
                _syncFilter.Init(IirFilterType.BandPass, SymbolRate, _samplerate, 2000.0);
            }

            #endregion

            #region Interpolator and Matched filter

            if (_interpolation > 1)
            {
                //interpolator
                var index = 0;
                var nullComplex = new Complex(0, 0);

                for (int i = 0; i < _length; i++ )
                {
                    _bufferPtr[i] = ((i % _interpolation) == 0) ? iqBuffer[index++] : nullComplex;
                }
            }
            else
            {
                Utils.Memcpy(_bufferPtr, iqBuffer, _length * sizeof(Complex));
            }

            _matchedFilter.Process(_bufferPtr, _length);
            
            #endregion

            #region Syncronization and Resampler

            float result;
            float syncValue;
            Complex data;
            bool signalOverZero;
            bool oscOverZero;
            float osc;

            for (int i = 0; i < _length; i++)
            {
                data = _bufferPtr[i];

                syncValue = (data * _lastData).ArgumentFast();
                _lastData = data.Conjugate();

                signalOverZero = ((syncValue > 0.0f) && (_lastSyncValue < 0.0f)) || ((syncValue < 0.0f) && (_lastSyncValue > 0.0f));   
                _lastSyncValue = syncValue;
                
                osc = _syncFilter.Process(signalOverZero ? 1.0f : 0.0f);
                
                oscOverZero = ((osc > 0) && (_lastOsc < 0));
                _lastOsc = osc;
                
                if (oscOverZero)
                {
                    result = (data * _lastSymbol).ArgumentFast();
                    _lastSymbol = data.Conjugate();

                    digitalBuffer[indexOut++] = result;
                }
            }

            #endregion

            return indexOut;
        }
    }
}

