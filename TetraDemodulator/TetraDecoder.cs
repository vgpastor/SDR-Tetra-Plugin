using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace SDRSharp.Tetra
{
    unsafe class TetraDecoder
    {
        public delegate void DataReadyDelegate(List<Dictionary<GlobalNames, int>> data);
        public event DataReadyDelegate DataReady;

        public delegate void SyncInfoReadyDelegate(Dictionary<GlobalNames, int> syncInfo);
        public event SyncInfoReadyDelegate SyncInfoReady;

        private PhyLevel _phyLevel = new PhyLevel();
        private LowerMacLevel _lowerMac = new LowerMacLevel();
        private MacLevel _parse = new MacLevel();
        
        private UnsafeBuffer _bbBuffer;
        private byte* _bbBufferPtr;
        private UnsafeBuffer _bkn1Buffer;
        private byte* _bkn1BufferPtr;
        private UnsafeBuffer _bkn2Buffer;
        private byte* _bkn2BufferPtr;
        private UnsafeBuffer _sb1Buffer;
        private byte* _sb1BufferPtr;

        private LogicChannel _logicChannel = new LogicChannel();
        private NetworkTime _networkTime = new NetworkTime();

        private Dictionary<GlobalNames, int> _syncInfo = new Dictionary<GlobalNames, int>();
        private List<Dictionary<GlobalNames, int>> _data = new List<Dictionary<GlobalNames, int>>();
        private int _timeCounter;
        private float _badBurstCounter;
        private float _averageBer;
        private Control _owner;
        
        public int NetworkTimeTN{ get; internal set; }
        public int NetworkTimeFN{ get; internal set; }
        public int NetworkTimeMN{ get; internal set; }

        public long DownFrequency { get; private set; }
        public bool BurstReceived { get; internal set; }
        public float Ber { get; internal set; }

        public TetraDecoder(Control owner)
        {
            _bbBuffer = UnsafeBuffer.Create(30);
            _bbBufferPtr = (byte*)_bbBuffer;
            _bkn1Buffer = UnsafeBuffer.Create(216);
            _bkn1BufferPtr = (byte*)_bkn1Buffer;
            _bkn2Buffer = UnsafeBuffer.Create(216);
            _bkn2BufferPtr = (byte*)_bkn2Buffer;
            _sb1Buffer = UnsafeBuffer.Create(120);
            _sb1BufferPtr = (byte*)_sb1Buffer;

            _owner = owner;
        }

        public void Dispose()
        {

        }

        public int Process(float* inBuffer, int length, byte* audioOut)
        {
            var trafficChannel = 0;
            var dataReceived = false;  
            var burst = _phyLevel.ParseTrainingSequence(inBuffer, length);

            BurstReceived = (burst.Type != BurstType.None);

            _timeCounter++;
            Ber = _averageBer;
            if (_timeCounter > 100)
            {
                Mer = (_badBurstCounter / _timeCounter) * 100.0f;
                
                _timeCounter = 0;
                _badBurstCounter = 0;
            }

            _networkTime.AddTimeSlot();

            Debug.WriteLine("");
            Debug.Write(string.Format("ts:{0:0} fr:{1:00}", _networkTime.TimeSlot, _networkTime.Frame ));
            Debug.Write(" burst:" + burst.Type.ToString());
            
            UpdatePublicProp();
            
            if (burst.Type == BurstType.None)
            {
                _badBurstCounter += 1.0f;
                return trafficChannel;
            }

            _phyLevel.ExtractPhyChannels(burst, _bbBufferPtr, _bkn1BufferPtr, _bkn2BufferPtr, _sb1BufferPtr);

            _parse.ResetAACH();

            _data.Clear();
            
            if (burst.Type == BurstType.SYNC)
            {
                _logicChannel = _lowerMac.ExtractLogicChannelFromSB(_sb1BufferPtr, _sb1Buffer.Length);
                _badBurstCounter += _logicChannel.CrcIsOk ? 0.0f : 0.5f;
                _averageBer = _averageBer * 0.5f + _lowerMac.Ber * 0.5f;
                if (_logicChannel.CrcIsOk)
                {
                    dataReceived = true;
                    _syncInfo.Clear();
                    _parse.SyncPDU(_logicChannel, _syncInfo);
                    _lowerMac.ScramblerCode = TetraUtils.CreateScramblerCode(_syncInfo[GlobalNames.MCC], _syncInfo[GlobalNames.MNC], _syncInfo[GlobalNames.ColorCode]);
                    _networkTime.Synchronize(_syncInfo[GlobalNames.TimeSlot], _syncInfo[GlobalNames.Frame], _syncInfo[GlobalNames.MultiFrame]);

                    UpdateSyncInfo(_syncInfo);
                }
            }

            _lowerMac.TS = _networkTime.TimeSlot;
            _lowerMac.FR = _networkTime.Frame;

            _logicChannel = _lowerMac.ExtractLogicChannelFromBB(_bbBufferPtr, _bbBuffer.Length);
            _badBurstCounter += _logicChannel.CrcIsOk ? 0.0f : 1.0f;
            if (_logicChannel.CrcIsOk)
            {
                _parse.AccessAsignPDU(_logicChannel);

                Debug.Write(" " + _parse.DownLinkChannelType.ToString() + string.Format(" {0:00} {1:00}", _parse.Field1, _parse.Field2));

                Debug.WriteLine("");
            
                switch (burst.Type)
                {
                    case BurstType.NDB1:

                        if (_parse.DownLinkChannelType == ChannelType.Traffic)
                        {
                            Debug.Write(" slot 12:voice");

                            trafficChannel = _networkTime.TimeSlot;
                            _logicChannel = _lowerMac.ExtractVoiceDataFromBKN1BKN2(_bkn1BufferPtr, _bkn2BufferPtr, _bkn1Buffer.Length);

                            Utils.Memcpy(audioOut, _logicChannel.Ptr, _logicChannel.Length);

                            break;
                        }
                        else
                        {
                            _logicChannel = _lowerMac.ExtractLogicChannelFromBKN1BKN2(_bkn1BufferPtr, _bkn2BufferPtr, _bkn1Buffer.Length);
                            _badBurstCounter += _logicChannel.CrcIsOk ? 0.0f : 1.0f;
                            _averageBer = _averageBer * 0.5f + _lowerMac.Ber * 0.5f;
                            Debug.Write(" slot 12:" + (_logicChannel.CrcIsOk ? " OK" : " Err"));
                            if (_logicChannel.CrcIsOk)
                            {
                                Debug.Write(" slot 12:");

                                dataReceived = true;
                                _parse.ParseMacPDU(_logicChannel, _data);
                            }
                        }
                        break;

                    case BurstType.NDB2:

                        _logicChannel = _lowerMac.ExtractLogicChannelFromBKN(_bkn1BufferPtr, _bkn1Buffer.Length);
                        _badBurstCounter += _logicChannel.CrcIsOk ? 0.0f : 0.5f;
                        _averageBer = _averageBer * 0.5f + _lowerMac.Ber * 0.5f;
                        Debug.Write(" slot 1:" + (_logicChannel.CrcIsOk ? " OK" : " Err"));
                        if (_logicChannel.CrcIsOk)
                        {
                            
                            dataReceived = true;
                            _parse.ParseMacPDU(_logicChannel, _data);
                        }

                        Debug.WriteLine("");
                       
                        if (_parse.DownLinkChannelType == ChannelType.Traffic && !_parse.HalfSlotStolen)
                        {
                            Debug.Write(" slot 2:voice");
            
                            trafficChannel = _networkTime.TimeSlot;
                            _logicChannel = _lowerMac.ExtractVoiceDataFromBKN2(_bkn2BufferPtr, _bkn2Buffer.Length);

                            Utils.Memcpy(audioOut, _logicChannel.Ptr, _logicChannel.Length);

                            break;
                        }
                        else
                        {
                            _logicChannel = _lowerMac.ExtractLogicChannelFromBKN(_bkn2BufferPtr, _bkn2Buffer.Length);
                            _badBurstCounter += _logicChannel.CrcIsOk ? 0.0f : 0.5f;
                            _averageBer = _averageBer * 0.5f + _lowerMac.Ber * 0.5f;
                            Debug.Write(" slot 2:" + (_logicChannel.CrcIsOk ? " OK" : " Err"));
                            if (_logicChannel.CrcIsOk)
                            {
                                dataReceived = true;
                                _parse.ParseMacPDU(_logicChannel, _data);
                            }
                        }
                        break;

                    case BurstType.SYNC:

                        Debug.Write(" slot 1:" + (dataReceived ? " OK" : " Err") + " Sync PDU");
                        Debug.WriteLine("");
                        
                        _logicChannel = _lowerMac.ExtractLogicChannelFromBKN(_bkn2BufferPtr, _bkn2Buffer.Length);
                        Debug.Write(" slot 2:" + (_logicChannel.CrcIsOk ? " OK" : " Err"));
                        _badBurstCounter += _logicChannel.CrcIsOk ? 0.0f : 0.5f;
                        _averageBer = _averageBer * 0.5f + _lowerMac.Ber * 0.5f;
                        if (_logicChannel.CrcIsOk)
                        {
                            dataReceived = true;
                            _parse.ParseMacPDU(_logicChannel, _data);
                        }
                        break;

                    default:
                        break;
                }
            }

            UpdateData(_data);

            return trafficChannel;
        }

        private void UpdateSyncInfo(Dictionary<GlobalNames, int> syncInfo)
        {
            if (SyncInfoReady != null)
            {
                SyncInfoReady(syncInfo);
            }
        }
        

        public void UpdateData(List<Dictionary<GlobalNames, int>> data)
        {
            if (DataReady != null)
            {
                DataReady(data);
            }
        }

        private void UpdatePublicProp()
        {
            if (_networkTime != null)
            {
                NetworkTimeFN = _networkTime.Frame;
                NetworkTimeTN = _networkTime.TimeSlot;
                NetworkTimeMN = _networkTime.SuperFrame;
            }
        }

        public float Mer { get; set; }
    }
}
