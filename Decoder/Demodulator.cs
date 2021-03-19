// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.Demodulator
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using SDRSharp.Radio;
using System;

namespace SDRSharp.Tetra
{
    internal class Demodulator
    {
        private const float Pi34 = 2.356194f;
        private const float Pi14 = 0.7853982f;
        private const float Pi12 = 1.570796f;
        private const float Pi2 = 6.283185f;
        private static readonly byte[] NormalTrainingSequence1 = new byte[22]
        {
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0
        };
        private static readonly byte[] NormalTrainingSequence2 = new byte[22]
        {
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0
        };
        private static readonly byte[] synchronizationTrainingSequence = new byte[38]
        {
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 1
        };
        private static readonly float[] freqCorrectionFeild = new float[40]
        {
      -2.356194f,
      -2.356194f,
      -2.356194f,
      -2.356194f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      0.7853982f,
      -2.356194f,
      -2.356194f,
      -2.356194f,
      -2.356194f
        };
        private const int NtsSquenceOffsetTMO = 122;
        private const int NtsSquenceOffsetDMO = 115;
        private const int StsSquenceOffset = 107;
        private const int SmallTrainingWindow = 6;
        private const int BuffersOverlap = 2;
        private const int SyncLostValue = 8;
        private const double SymbolRate = 18000.0;
        private UnsafeBuffer _buffer;
        private unsafe Complex* _bufferPtr;
        private UnsafeBuffer _tempBuffer;
        private unsafe float* _tempBufferPtr;
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

        public unsafe void ProcessBuffer(
          Burst burst,
          Complex* iqBuffer,
          double iqSamplerate,
          int iqBufferLength,
          float* digitalBuffer)
        {
            burst.Type = BurstType.WaitBurst;
            burst.Length = 0;
            if (this._buffer == null || iqSamplerate != this._samplerateIn)
            {
                this._samplerateIn = iqSamplerate;
                this._samplerate = this._samplerateIn;
                this._interpolation = 1;
                while (this._samplerateIn * (double)this._interpolation < 90000.0)
                    ++this._interpolation;
                this._samplerate = this._samplerateIn * (double)this._interpolation;
                this._length = iqBufferLength * this._interpolation;
                this._symbolLength = this._samplerate / 18000.0;
                this._windowLength = (int)Math.Round(this._symbolLength * (double)byte.MaxValue);
                this._writeAddress = 0;
                this._tailBufferLength = (int)Math.Round(this._symbolLength);
                this._syncCounter = 0;
                this._buffer = UnsafeBuffer.Create(this._length + this._tailBufferLength, sizeof(Complex));
                this._bufferPtr = (Complex*)(void*)this._buffer;
                this._tempBuffer = UnsafeBuffer.Create((int)this._samplerate, 4);
                this._tempBufferPtr = (float*)(void*)this._tempBuffer;
                this._filterLength = Math.Max((int)(this._samplerate / 18000.0) | 1, 5);
                float[] coefficients = FilterBuilder.MakeSinc(this._samplerate, 13500.0, this._filterLength);
                this._matchedFilter = new IQFirFilter(coefficients);
                this._fsFilter = new FirFilter(coefficients);
                this.CreateFrameSynchronization();
            }
            if (this._interpolation > 1)
            {
                int num = 0;
                Complex complex = new Complex(0.0f, 0.0f);
                for (int index = 0; index < this._length; ++index)
                    this._bufferPtr[index + this._tailBufferLength] = index % this._interpolation == 0 ? iqBuffer[num++] : complex;
            }
            else
                Radio.Utils.Memcpy((void*)(this._bufferPtr + this._tailBufferLength), (void*)iqBuffer, this._length * sizeof(Complex));
            this._matchedFilter.Process(this._bufferPtr + this._tailBufferLength, this._length);
            if (this._writeAddress + this._length < this._tempBuffer.Length)
            {
                for (int index = 0; index < this._length; ++index)
                {
                    this._tempBufferPtr[this._writeAddress] = (this._bufferPtr[index + this._tailBufferLength] * this._bufferPtr[index].Conjugate()).ArgumentFast();
                    ++this._writeAddress;
                }
            }
            for (int index = 0; index < this._tailBufferLength; ++index)
                this._bufferPtr[index] = this._bufferPtr[this._length + index];
            if (this._writeAddress < this._windowLength * 2)
                return;
            this._ntsOffset = burst.Mode == Mode.TMO ? (int)Math.Round(122.0 * this._symbolLength) : (int)Math.Round(115.0 * this._symbolLength);
            this._stsOffset = (int)Math.Round(107.0 * this._symbolLength);
            this._ndb1minSum = float.MaxValue;
            this._ndb1Index = 0;
            this._ndb2minSum = float.MaxValue;
            this._ndb2Index = 0;
            this._stsMinSum = float.MaxValue;
            this._stsIndex = 0;
            this._trainingWindow = this._syncCounter > 0 ? 6 * this._tailBufferLength : this._windowLength;
            if (this._syncCounter > 0)
                --this._syncCounter;
            for (int index1 = 0; index1 < this._trainingWindow; ++index1)
            {
                this._ndb1sum = 0.0f;
                this._ndb2sum = 0.0f;
                this._stsSum = 0.0f;
                for (int index2 = 0; index2 < this._nts1Buffer.Length; ++index2)
                {
                    this._sample = this._tempBufferPtr[index1 + index2 + this._ntsOffset];
                    this._training = this._nts1BufferPtr[index2];
                    this._ndb1sum += (float)(((double)this._training - (double)this._sample) * ((double)this._training - (double)this._sample));
                }
                if ((double)this._ndb1sum < (double)this._ndb1minSum)
                {
                    this._ndb1minSum = this._ndb1sum;
                    this._ndb1Index = index1;
                }
                for (int index2 = 0; index2 < this._nts2Buffer.Length; ++index2)
                {
                    this._sample = this._tempBufferPtr[index1 + index2 + this._ntsOffset];
                    this._training = this._nts2BufferPtr[index2];
                    this._ndb2sum += (float)(((double)this._training - (double)this._sample) * ((double)this._training - (double)this._sample));
                }
                if ((double)this._ndb2sum < (double)this._ndb2minSum)
                {
                    this._ndb2minSum = this._ndb2sum;
                    this._ndb2Index = index1;
                }
                for (int index2 = 0; index2 < this._stsBuffer.Length; ++index2)
                {
                    this._sample = this._tempBufferPtr[index1 + index2 + this._stsOffset];
                    this._training = this._stsBufferPtr[index2];
                    this._stsSum += (float)(((double)this._training - (double)this._sample) * ((double)this._training - (double)this._sample));
                }
                if ((double)this._stsSum < (double)this._stsMinSum)
                {
                    this._stsMinSum = this._stsSum;
                    this._stsIndex = index1;
                }
            }
            this._ndb1minSum /= (float)this._nts1Buffer.Length;
            this._ndb2minSum /= (float)this._nts2Buffer.Length;
            this._stsMinSum /= (float)this._stsBuffer.Length;
            if ((double)this._ndb1minSum < 1.0 || (double)this._ndb2minSum < 1.0 || (double)this._stsMinSum < 1.0)
            {
                if ((double)this._ndb1minSum < (double)this._ndb2minSum && (double)this._ndb1minSum < (double)this._stsMinSum)
                {
                    this._offset = this._ndb1Index;
                    burst.Type = BurstType.NDB1;
                }
                else if ((double)this._ndb2minSum < (double)this._ndb1minSum && (double)this._ndb2minSum < (double)this._stsMinSum)
                {
                    this._offset = this._ndb2Index;
                    burst.Type = BurstType.NDB2;
                }
                else
                {
                    this._offset = this._stsIndex;
                    burst.Type = BurstType.SYNC;
                }
                this._syncCounter = 8;
            }
            else
            {
                burst.Type = BurstType.None;
                this._offset = this._tailBufferLength * 2;
            }
            int index3 = 0;
            int num1 = 0;
            for (; index3 < 256; ++index3)
            {
                num1 = (int)Math.Round((double)index3 * this._symbolLength);
                digitalBuffer[index3] = this._tempBufferPtr[this._offset + num1];
            }
            this._offset += num1;
            this._offset -= this._tailBufferLength * 2;
            if (this._offset < 0)
                this._offset = 0;
            this._writeAddress -= this._offset;
            if (this._writeAddress < 0)
                this._writeAddress = 0;
            Radio.Utils.Memcpy((void*)this._tempBufferPtr, (void*)(this._tempBufferPtr + this._offset), this._writeAddress * 4);
            this.AngleToSymbol(burst.Ptr, digitalBuffer, (int)byte.MaxValue);
            burst.Length = 510;
        }

        private unsafe void CreateFrameSynchronization()
        {
            double symbolLength = this._samplerate / 18000.0;
            this.CreateFsBuffers(symbolLength);
            int symbolIndex1 = 0;
            for (int index = 0; index < this._nts1Buffer.Length; ++index)
            {
                if (((double)index + symbolLength * 0.5) % symbolLength < 1.0)
                {
                    this._nts1BufferPtr[index] = this.SymbolToAngel(Demodulator.NormalTrainingSequence1, symbolIndex1);
                    ++symbolIndex1;
                }
                else
                    this._nts1BufferPtr[index] = 0.0f;
            }
            int symbolIndex2 = 0;
            for (int index = 0; index < this._nts2Buffer.Length; ++index)
            {
                if (((double)index + symbolLength * 0.5) % symbolLength < 1.0)
                {
                    this._nts2BufferPtr[index] = this.SymbolToAngel(Demodulator.NormalTrainingSequence2, symbolIndex2);
                    ++symbolIndex2;
                }
                else
                    this._nts2BufferPtr[index] = 0.0f;
            }
            int symbolIndex3 = 0;
            for (int index = 0; index < this._stsBuffer.Length; ++index)
            {
                if (((double)index + symbolLength * 0.5) % symbolLength < 1.0)
                {
                    this._stsBufferPtr[index] = this.SymbolToAngel(Demodulator.synchronizationTrainingSequence, symbolIndex3);
                    ++symbolIndex3;
                }
                else
                    this._stsBufferPtr[index] = 0.0f;
            }
            this._fsFilter.Process(this._nts1BufferPtr, this._nts1Buffer.Length);
            this._fsFilter.Process(this._nts2BufferPtr, this._nts2Buffer.Length);
            this._fsFilter.Process(this._stsBufferPtr, this._stsBuffer.Length);
        }

        private unsafe void CreateFsBuffers(double symbolLength)
        {
            this._nts1Buffer?.Dispose();
            this._nts2Buffer?.Dispose();
            this._stsBuffer?.Dispose();
            int length1 = (int)(symbolLength * ((double)Demodulator.NormalTrainingSequence1.Length * 0.5)) | 1;
            int length2 = (int)(symbolLength * ((double)Demodulator.synchronizationTrainingSequence.Length * 0.5)) | 1;
            this._nts1Buffer = UnsafeBuffer.Create(length1, 4);
            this._nts1BufferPtr = (float*)(void*)this._nts1Buffer;
            this._nts2Buffer = UnsafeBuffer.Create(length1, 4);
            this._nts2BufferPtr = (float*)(void*)this._nts2Buffer;
            this._stsBuffer = UnsafeBuffer.Create(length2, 4);
            this._stsBufferPtr = (float*)(void*)this._stsBuffer;
        }

        private float SymbolToAngel(byte[] trainingSquence, int symbolIndex)
        {
            float num = trainingSquence[symbolIndex * 2 + 1] == (byte)1 ? 2.356194f : 0.7853982f;
            return trainingSquence[symbolIndex * 2] == (byte)1 ? -num : num;
        }

        private unsafe void AngleToSymbol(byte* bitsBuffer, float* angles, int sourceLength)
        {
            while (sourceLength-- > 0)
            {
                float num = *angles++;
                *bitsBuffer++ = (double)num < 0.0 ? (byte)1 : (byte)0;
                *bitsBuffer++ = (double)Math.Abs(num) > 1.57079637050629 ? (byte)1 : (byte)0;
            }
        }

        public float FrequencyError { get; set; }
    }
}
