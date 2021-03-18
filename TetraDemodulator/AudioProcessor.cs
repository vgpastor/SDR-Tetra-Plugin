using SDRSharp.Radio;


namespace SDRSharp.Tetra
{
    public unsafe class AudioProcessor : IRealProcessor
    {
        public delegate void AudioReadyDelegate(float* buffer, double samplerate, int length);
        public event AudioReadyDelegate AudioReady;

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

        public void Process(float* buffer, int length)
        {
            if (AudioReady != null)
            {
                AudioReady(buffer, _sampleRate, length);
            }
        }
    }
}