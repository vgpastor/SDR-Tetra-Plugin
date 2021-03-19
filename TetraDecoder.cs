using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace SDRSharp.Tetra
{
    unsafe class TetraDecoder
    {
        public delegate void DataReadyDelegate(List<ReceivedData> data);
        public event DataReadyDelegate DataReady;

        public delegate void SyncInfoReadyDelegate(ReceivedData syncInfo);
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

        private ReceivedData _syncInfo = new ReceivedData();
        private List<ReceivedData> _data = new List<ReceivedData>();
        private int _timeCounter;
        private float _badBurstCounter;
        private float _averageBer;
        private Control _owner;
        private int _fpass;
        private unsafe void* _ch1InitStruct;
        private unsafe void* _ch2InitStruct;
        private unsafe void* _ch3InitStruct;
        private unsafe void* _ch4InitStruct;

        short[] _cdc = new short[276];
        short[] _sdc = new short[480];
        private bool _haveErrors;


        public int NetworkTimeTN { get; internal set; }
        public int NetworkTimeFN { get; internal set; }
        public int NetworkTimeMN { get; internal set; }

        public long DownFrequency { get; private set; }
        public bool BurstReceived { get; internal set; }
        public float Ber { get; internal set; }
        public bool HaveErrors { get { return _haveErrors; } }


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

            _fpass = 1;

            _ch1InitStruct = NativeMethods.tetra_decode_init();
            _ch2InitStruct = NativeMethods.tetra_decode_init();
            _ch3InitStruct = NativeMethods.tetra_decode_init();
            _ch4InitStruct = NativeMethods.tetra_decode_init();
        }

        public void Dispose()
        {

        }

        public int Process(Burst burst, float* audioOut)
        {
            var trafficChannel = 0;

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

            if (burst.Type == BurstType.None)
            {
                _haveErrors = true;
                if (TetraMode == Mode.TMO)
                {
                    _badBurstCounter++;
                    //Debug.WriteLine("burst err");
                    //Debug.Write(string.Format("ts:{0:0} fr:{1:00} Burst_Err", _networkTime.TimeSlot, _networkTime.Frame));
                }
                return trafficChannel;
            }

            _haveErrors = false;

            //Debug.WriteLine("");
            //Debug.Write(string.Format("ts:{0:0} fr:{1:00}", _networkTime.TimeSlot, _networkTime.Frame));
            //Debug.Write(" " + burst.Type.ToString());

            _parse.ResetAACH();

            _data.Clear();
            _syncInfo.Clear();

            #region Sync burst

            if (burst.Type == BurstType.SYNC)
            {
                _phyLevel.ExtractSBChannels(burst, _sb1BufferPtr);

                _logicChannel = _lowerMac.ExtractLogicChannelFromSB(_sb1BufferPtr, _sb1Buffer.Length);
                _badBurstCounter += _logicChannel.CrcIsOk ? 0.0f : 0.5f;
                _haveErrors |= !_logicChannel.CrcIsOk;
                _averageBer = _averageBer * 0.5f + _lowerMac.Ber * 0.5f;
                //Debug.Write(" slot 1:" + (_logicChannel.CrcIsOk ? " OK" : " Err"));
                if (_logicChannel.CrcIsOk)
                {
                    _parse.SyncPDU(_logicChannel, _syncInfo);

                    if (_syncInfo.Value(GlobalNames.SystemCode) < 8)
                    {
                        TetraMode = Mode.TMO;
                        _lowerMac.ScramblerCode = TetraUtils.CreateScramblerCode(_syncInfo.Value(GlobalNames.MCC), _syncInfo.Value(GlobalNames.MNC), _syncInfo.Value(GlobalNames.ColorCode));
                        _networkTime.Synchronize(_syncInfo.Value(GlobalNames.TimeSlot), _syncInfo.Value(GlobalNames.Frame), _syncInfo.Value(GlobalNames.MultiFrame));
                    }
                    else
                    {
                        TetraMode = Mode.DMO;

                        if (_syncInfo.Value(GlobalNames.SYNC_PDU_type) == 0)
                        {
                            //Debug.Write(_syncInfo.Value(GlobalNames.Master_slave_link_flag) == 1 ? " Master" : " Slave");

                            if (_syncInfo.Value(GlobalNames.Master_slave_link_flag) == 1 || _syncInfo.Value(GlobalNames.Communication_type) == 0)
                            {
                                _networkTime.SynchronizeMaster(_syncInfo.Value(GlobalNames.TimeSlot), _syncInfo.Value(GlobalNames.Frame));
                            }
                            else
                            {
                                _networkTime.SynchronizeSlave(_syncInfo.Value(GlobalNames.TimeSlot), _syncInfo.Value(GlobalNames.Frame));
                            }
                        }
                    }
                }
                else
                {
                    //Debug.WriteLine("Sync SB crc error");
                }

                _phyLevel.ExtractPhyChannels(TetraMode, burst, _bbBufferPtr, _bkn1BufferPtr, _bkn2BufferPtr);

                if (TetraMode == Mode.TMO)
                {
                    _logicChannel = _lowerMac.ExtractLogicChannelFromBKN(_bkn2BufferPtr, _bkn2Buffer.Length);
                    _logicChannel.TimeSlot = _networkTime.TimeSlot;
                    _logicChannel.Frame = _networkTime.Frame;

                    _badBurstCounter += _logicChannel.CrcIsOk ? 0.0f : 0.5f;
                    _haveErrors |= !_logicChannel.CrcIsOk;
                    _averageBer = _averageBer * 0.5f + _lowerMac.Ber * 0.5f;
                    //Debug.Write(" slot 2:" + (_logicChannel.CrcIsOk ? " OK" : " Err"));
                    if (_logicChannel.CrcIsOk)
                    {
                        _parse.TmoParseMacPDU(_logicChannel, _data);
                    }
                    else
                    {
                        //Debug.WriteLine("Sync BKN2 crc error");
                    }
                }
                else
                {
                    _logicChannel = _lowerMac.ExtractLogicChannelFromBKN2(_bkn2BufferPtr, _bkn2Buffer.Length);
                    _logicChannel.TimeSlot = _networkTime.TimeSlot;
                    _logicChannel.Frame = _networkTime.Frame;

                    _badBurstCounter += _logicChannel.CrcIsOk ? 0.0f : 0.5f;
                    _haveErrors |= !_logicChannel.CrcIsOk;
                    _averageBer = _averageBer * 0.5f + _lowerMac.Ber * 0.5f;
                    //Debug.Write(" slot 2:" + (_logicChannel.CrcIsOk ? " OK" : " Err"));
                    if (_logicChannel.CrcIsOk)
                    {
                        _parse.SyncPDUHalfSlot(_logicChannel, _syncInfo);

                        if (_syncInfo.Value(GlobalNames.Communication_type) == 0)
                        {
                            if (_syncInfo.Contains(GlobalNames.MNC) && _syncInfo.Contains(GlobalNames.Source_address))
                                _lowerMac.ScramblerCode = TetraUtils.CreateScramblerCode(_syncInfo.Value(GlobalNames.MNC), _syncInfo.Value(GlobalNames.Source_address));
                        }
                        else if (_syncInfo.Value(GlobalNames.Communication_type) == 1)
                        {
                            if (_syncInfo.Contains(GlobalNames.Repeater_address) && _syncInfo.Contains(GlobalNames.Source_address))
                                _lowerMac.ScramblerCode = TetraUtils.CreateScramblerCode(_syncInfo.Value(GlobalNames.Repeater_address), _syncInfo.Value(GlobalNames.Source_address));
                        }
                    }
                }

                UpdateSyncInfo(_syncInfo);
            }
            #endregion

            UpdatePublicProp();

            #region Other burst

            if (burst.Type != BurstType.SYNC)
            {
                _phyLevel.ExtractPhyChannels(TetraMode, burst, _bbBufferPtr, _bkn1BufferPtr, _bkn2BufferPtr);

                if (TetraMode == Mode.TMO)
                {
                    _logicChannel = _lowerMac.ExtractLogicChannelFromBB(_bbBufferPtr, _bbBuffer.Length);
                    _logicChannel.TimeSlot = _networkTime.TimeSlot;
                    _logicChannel.Frame = _networkTime.Frame;

                    _badBurstCounter += _logicChannel.CrcIsOk ? 0.0f : 0.2f;
                    _haveErrors |= !_logicChannel.CrcIsOk;
                    //Debug.Write(_logicChannel.CrcIsOk ? " BB OK" : " BB Err");
                    if (_logicChannel.CrcIsOk)
                    {
                        _parse.AccessAsignPDU(_logicChannel);
                    }
                    else
                    {
                        //Debug.WriteLine("BB crc error");
                    }

                }

                switch (burst.Type)
                {
                    case BurstType.NDB1:

                        if (((TetraMode == Mode.TMO) && (_parse.DownLinkChannelType == ChannelType.Traffic))
                            || ((TetraMode == Mode.DMO) && (!_networkTime.Frame18) && (_networkTime.TimeSlot == 1))
                            || ((TetraMode == Mode.DMO) && (!_networkTime.Frame18Slave) && (_networkTime.TimeSlotSlave == 1)))
                        {
                            //Debug.Write(" slot 12:voice");

                            _logicChannel = _lowerMac.ExtractVoiceDataFromBKN1BKN2(_bkn1BufferPtr, _bkn2BufferPtr, _bkn1Buffer.Length);
                            var itsAudio = DecodeAudio(audioOut, _logicChannel.Ptr, _logicChannel.Length, false, _networkTime.TimeSlot);
                            trafficChannel = itsAudio ? _networkTime.TimeSlot : 0;
                            break;
                        }
                        else
                        {
                            _logicChannel = _lowerMac.ExtractLogicChannelFromBKN1BKN2(_bkn1BufferPtr, _bkn2BufferPtr, _bkn1Buffer.Length);
                            _logicChannel.TimeSlot = _networkTime.TimeSlot;
                            _logicChannel.Frame = _networkTime.Frame;

                            _badBurstCounter += _logicChannel.CrcIsOk ? 0.0f : 0.8f;
                            _haveErrors |= !_logicChannel.CrcIsOk;
                            _averageBer = _averageBer * 0.5f + _lowerMac.Ber * 0.5f;
                            //Debug.Write(" slot 12:" + (_logicChannel.CrcIsOk ? " OK" : " Err"));
                            if (_logicChannel.CrcIsOk)
                            {
                                if (TetraMode == Mode.TMO) _parse.TmoParseMacPDU(_logicChannel, _data);
                                else _parse.DmoParseMacPDU(_logicChannel, _data);
                            }
                            else
                            {
                                //Debug.WriteLine("BKN1 BKN2 crc error");
                            }

                        }
                        break;

                    case BurstType.NDB2:

                        _logicChannel = _lowerMac.ExtractLogicChannelFromBKN(_bkn1BufferPtr, _bkn1Buffer.Length);
                        _logicChannel.TimeSlot = _networkTime.TimeSlot;
                        _logicChannel.Frame = _networkTime.Frame;

                        _badBurstCounter += _logicChannel.CrcIsOk ? 0.0f : 0.4f;
                        _haveErrors |= !_logicChannel.CrcIsOk;
                        _averageBer = _averageBer * 0.5f + _lowerMac.Ber * 0.5f;
                        //Debug.Write(" slot 1:" + (_logicChannel.CrcIsOk ? " OK" : " Err"));
                        if (_logicChannel.CrcIsOk)
                        {
                            if (TetraMode == Mode.TMO) _parse.TmoParseMacPDU(_logicChannel, _data);
                            else _parse.DmoParseMacPDU(_logicChannel, _data);
                        }
                        else
                        {
                            //Debug.WriteLine("BKN1 crc error");
                        }

                        if ((_parse.DownLinkChannelType == ChannelType.Traffic && !_parse.HalfSlotStolen)
                            || ((TetraMode == Mode.DMO) && (!_networkTime.Frame18) && (_networkTime.TimeSlot == 1) && (!_parse.HalfSlotStolen))
                            || ((TetraMode == Mode.DMO) && (!_networkTime.Frame18Slave) && (_networkTime.TimeSlotSlave == 1) && (!_parse.HalfSlotStolen)))
                        {
                            //Debug.Write(" slot 2:voice");
                            _logicChannel = _lowerMac.ExtractVoiceDataFromBKN2(_bkn2BufferPtr, _bkn2Buffer.Length);
                            var itsAudio = DecodeAudio(audioOut, _logicChannel.Ptr, _logicChannel.Length, true, _networkTime.TimeSlot);
                            trafficChannel = itsAudio ? _networkTime.TimeSlot : 0;
                            break;
                        }
                        else
                        {
                            _logicChannel = _lowerMac.ExtractLogicChannelFromBKN(_bkn2BufferPtr, _bkn2Buffer.Length);
                            _logicChannel.TimeSlot = _networkTime.TimeSlot;
                            _logicChannel.Frame = _networkTime.Frame;

                            _badBurstCounter += _logicChannel.CrcIsOk ? 0.0f : 0.4f;
                            _haveErrors |= !_logicChannel.CrcIsOk;
                            _averageBer = _averageBer * 0.5f + _lowerMac.Ber * 0.5f;
                            //Debug.Write(" slot 2:" + (_logicChannel.CrcIsOk ? " OK" : " Err"));
                            if (_logicChannel.CrcIsOk)
                            {
                                if (TetraMode == Mode.TMO) _parse.TmoParseMacPDU(_logicChannel, _data);
                                else _parse.DmoParseMacPDU(_logicChannel, _data);
                            }
                            else
                            {
                                //Debug.WriteLine("BKN2 crc error");
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            if (_data.Count > 0) UpdateData(_data);

            #endregion

            return trafficChannel;
        }

        private void UpdateSyncInfo(ReceivedData syncInfo)
        {
            if (SyncInfoReady != null)
            {
                _owner.BeginInvoke(SyncInfoReady, syncInfo);
            }
        }


        public void UpdateData(List<ReceivedData> data)
        {
            if (DataReady != null)
            {
                _owner.BeginInvoke(DataReady, data.ToList());
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

        private bool DecodeAudio(float* audioBuffer, byte* buf, int length, bool stolten, int ch)
        {
            var noErrors = true;

            fixed (short* cdcPtr = _cdc, sdcPtr = _sdc)
            {
                NativeMethods.tetra_cdec(_fpass, buf, cdcPtr, stolten ? 1 : 0);
                _fpass = 0;

                noErrors = (cdcPtr[0] == 0) || (cdcPtr[138] == 0);

                _badBurstCounter += (cdcPtr[0] == 0 || stolten) ? 0 : 0.4f;
                _badBurstCounter += cdcPtr[138] == 0 ? 0 : 0.4f;

                //Debug.WriteIf(!stolten, " fr1_" + (cdcPtr[0] == 0 ? "Ok" : "Err"));
                //Debug.Write(" fr2_" + (cdcPtr[138] == 0 ? "Ok" : "Err"));

                var initStruct = _ch1InitStruct;
                switch (ch)
                {
                    case 1: initStruct = _ch1InitStruct; break;
                    case 2: initStruct = _ch2InitStruct; break;
                    case 3: initStruct = _ch3InitStruct; break;
                    case 4: initStruct = _ch4InitStruct; break;
                }

                NativeMethods.tetra_sdec(cdcPtr, sdcPtr, initStruct);
                ShortToFloatPtr(sdcPtr, audioBuffer, _sdc.Length);
            }

            return noErrors;
        }

        private void ShortToFloatPtr(short* source, float* dest, int length)
        {
            var gain = 0.0001f / Int16.MaxValue;

            for (int i = 0; i < length; i++)
            {
                dest[i] = source[i] * gain;
            }
        }

        public float Mer { get; set; }

        public Mode TetraMode { get; set; }
    }
}
