using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SDRSharp.Tetra
{
    public class TcpServer : IDisposable
    {
        private const int DefaultPortNumber = 47806;

        private TcpListener _listener;
        private int _port = DefaultPortNumber;
        private bool _serverRunning;

        private readonly List<TcpClient> _tcpClients = new List<TcpClient>();
        private readonly List<TcpClient> _deadTcpClients = new List<TcpClient>();

        public int ConnectedClients
        {
            get
            {
                return _tcpClients.Count;
            }
        }

        ~TcpServer()
        {
            Dispose();
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
        
        #region Public Methods

        public void FrameReady(byte[] frame, int actualLength)
        {
            if (_tcpClients.Count == 0 || !_serverRunning) return;

            lock (_tcpClients)
            {
                foreach (TcpClient client in _tcpClients)
                {
                    try
                    {
                        if (client.Connected)
                        {
                            var stream = client.GetStream();
                            stream.Write(frame, 0, frame.Length);
                            stream.Flush();
                        }
                        else
                        {
                            _deadTcpClients.Add(client);
                        }
                    }
                    catch
                    {
                        _deadTcpClients.Add(client);
                    }
                }
            }
            if (_deadTcpClients.Count > 0)
            {
                CloseDead();
            }
        }

        public void Start(int port)
        {
            _port = port;

            #region Listen / Async Accept

            try
            {
                _listener = new TcpListener(IPAddress.Any, _port);
                _listener.Start();
                _listener.BeginAcceptTcpClient(TcpClientConnectCallback, _listener);
                Console.WriteLine("Listening on {0}", _listener.LocalEndpoint);
                _serverRunning = true;
            }
            catch
            {
                if (_listener != null)
                {
                    _listener.Stop();
                    _listener = null;
                }
                throw;
            }

            #endregion

        }

        public void Stop()
        {
            _serverRunning = false;
            CloseAll();
            
            try
            {
                _listener.Stop();
            }
            catch
            {
            }
        }

        #endregion

        #region Private Methods

        private void CloseDead()
        {
            lock (_tcpClients)
            {
                foreach (TcpClient client in _deadTcpClients)
                {
                    try
                    {
                        Console.WriteLine("Removing client from {0}", client.Client.RemoteEndPoint);
                        if (client.Connected)
                        {
                            var stream = client.GetStream();
                            stream.Close();
                        }
                        client.Close();
                    }
                    catch
                    {
                    }
                    _tcpClients.Remove(client);
                }
            }
            _deadTcpClients.Clear();
        }

        private void CloseAll()
        {
            lock (_tcpClients)
            {
                foreach (TcpClient client in _tcpClients)
                {
                    try
                    {
                        if (client.Connected)
                        {
                            var stream = client.GetStream();
                            stream.Close();
                        }
                        client.Close();
                    }
                    catch
                    {
                    }
                }
                _tcpClients.Clear();
            }
        }

        #endregion

        #region Async Callback

        private void TcpClientConnectCallback(IAsyncResult result)
        {
            if (!_serverRunning) return;

            var listener = (TcpListener)result.AsyncState;
            var client = listener.EndAcceptTcpClient(result);
            lock (_tcpClients)
            {
                _tcpClients.Add(client);
            }

            Console.WriteLine("New client from {0}. {1} clients now connected.", client.Client.RemoteEndPoint, _tcpClients.Count);

            try
            {
                _listener.BeginAcceptTcpClient(TcpClientConnectCallback, _listener);
            }
            catch
            {
                //Console.WriteLine("Terminating TCP Server");
                Stop();
            }

        }

        #endregion
    }
}
