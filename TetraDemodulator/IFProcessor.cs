using SDRSharp.Radio;


namespace SDRSharp.Tetra
{
    public unsafe class IFProcessor : IIQProcessor
    {
        public delegate void IQReadyDelegate(Complex* buffer, double samplerate, int length);
        public event IQReadyDelegate IQReady;

        private double _sampleRate;
        private bool _enabled;

        public double SampleRate
        {
            get { return _sampleRate; }
            set { _sampleRate = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public void Process(Complex* buffer, int length)
        {
            if (IQReady != null)
            {
                IQReady(buffer, _sampleRate, length);
            }
        }
    }
}