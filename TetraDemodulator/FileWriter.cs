using SDRSharp.Radio;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

// Original code by BzztPloink
// Hacked by MM6DOS
// крякед бу Васили_ру
namespace SDRSharp.Tetra
{
    public unsafe class FileWriter
    {
        private const long MaxStreamLength = 2147483648;

        private readonly string _filename;
        private BinaryWriter _outputStream;
        private long _length;
        private bool _isStreamFull;
        
        #region Public Propertites

        public long Length
        {
            get { return _length; }
        }

        public bool IsStreamFull
        {
            get { return _isStreamFull; }
        }

        #endregion

        public FileWriter(string filename)
        {
            _filename = filename;
        }

        #region Public Methods

        public void Open()
        {
            if (_outputStream == null)
            {
                _outputStream = new BinaryWriter(File.Create(_filename));
                return;
            }
            throw new InvalidOperationException("Stream already open");
        }

        public void Close()
        {
            if (_outputStream != null)
            {
                _outputStream.Flush();
                _outputStream.Close();
                _outputStream = null;
                return;
            }
            throw new InvalidOperationException("Stream not open");
        }

        public void Write(byte[] data, int length)
        {
            if (_outputStream != null)
            {
                WriteStream(data);
                return;
            }
            throw new InvalidOperationException("Stream not open");
        }

        #endregion

        #region Private Methods

        private void WriteStream(byte[] data)
        {
            if (_outputStream != null)
            {
                var toWrite = (int)Math.Min(MaxStreamLength - _outputStream.BaseStream.Length, data.Length);

                _outputStream.Write(data, 0, toWrite);
                
                _length += toWrite;
                _isStreamFull = _outputStream.BaseStream.Length >= MaxStreamLength;
            }
        }

        #endregion
    }
}
