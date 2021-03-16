// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.IFProcessor
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using SDRSharp.Radio;

namespace SDRSharp.Tetra
{
    public class IFProcessor : IIQProcessor, IStreamProcessor, IBaseProcessor
    {
        private double _sampleRate;
        private bool _enabled;

        public event IFProcessor.IQReadyDelegate IQReady;

        public double SampleRate
        {
            get => this._sampleRate;
            set => this._sampleRate = value;
        }

        public bool Enabled
        {
            get => this._enabled;
            set => this._enabled = value;
        }

        public unsafe void Process(Complex* buffer, int length)
        {
            IFProcessor.IQReadyDelegate iqReady = this.IQReady;
            if (iqReady == null)
                return;
            iqReady(buffer, this._sampleRate, length);
        }

        public unsafe delegate void IQReadyDelegate(Complex* buffer, double samplerate, int length);
    }
}
