using SDRSharp.Common;
using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace SDRSharp.Tetra
{
    public unsafe partial class TetraPanel : UserControl
    {
        private const float TwoPi = (float)(Math.PI * 2.0);
        private const float Pi = (float)Math.PI;
        private const float PiDivTwo = (float)(Math.PI / 2.0);
        private const float PiDivFor = (float)(Math.PI / 4.0);

        private const int SamplesPerSymbol = 4;
        private const int BurstLengthBits = 510;
        private const int BurstLengthSymbols = 255;
        private const int SamplesPerBurst = BurstLengthSymbols * SamplesPerSymbol;

        private const int ChannelActiveDelay = 10;

        private ISharpControl _controlInterface;
        private IFProcessor _ifProcessor;
        private Demodulator _demodulator;
        private TetraDecoder _decoder;

        private UnsafeBuffer _iqBuffer;
        private Complex* _iqBufferPtr;
        private UnsafeBuffer _symbolsBuffer;
        private float* _symbolsBufferPtr;

        private ComplexFifoStream _radioFifoBuffer;
        private UnsafeBuffer _displayBuffer;
        private float* _displayBufferPtr;

        private UnsafeBuffer _outAudioBuffer;
        private float* _outAudioBufferPtr;
        private UnsafeBuffer _resampledAudio;
        private float* _resampledAudioPtr;
        private Resampler _audioResampler;
        private FloatFifoStream _audioStreamChannel;

        private double _audioSamplerate;

        private Thread _decodingThread;
        private bool _decodingIsStarted;

        private double _iqSamplerate;
        private bool _processIsStarted;
        private int _lostBuffers;
        private bool _dispayBufferReady;

        private TextFile _textFile = new TextFile();
        private TetraSettings _tetraSettings;
        private SettingsPersister _settingsPersister;

        private bool _needDisplayBufferUpdate;

        private AudioProcessor _audioProcessor;

        private bool _showInfo;
        private bool _needCloseInfo;

        private bool _channel1Listen;
        private bool _channel2Listen;
        private bool _channel3Listen;
        private bool _channel4Listen;
        private bool _ch1IsActive;
        private bool _ch2IsActive;
        private bool _ch3IsActive;
        private bool _ch4IsActive;
        private bool _autoSelectChannel;
        private int _activeCounter1;
        private int _activeCounter2;
        private int _activeCounter3;
        private int _activeCounter4;

        private NetInfoWindow _infoWindow;

        private List<ReceivedData> _rawData = new List<ReceivedData>();
        private List<ReceivedData> _cmceData = new List<ReceivedData>();
        private List<ReceivedData> _syncData = new List<ReceivedData>();
        private ReceivedData _syncInfo = new ReceivedData();
        private ReceivedData _sysInfo = new ReceivedData();

        private CurrentLoad[] _currentCellLoad = new CurrentLoad[4];

        private int _currentCell_NMI;
        private int _currentCell_MNC;
        private int _currentCell_MCC;
        private int _currentCell_LA;
        private int _currentCell_CC;
        private int _currentCell_Carrier;
        private int _mainCell_Carrier;
        private long _mainCell_Frequency;

        private SortedDictionary<int, CallsEntry> _currentCalls = new SortedDictionary<int, CallsEntry>();
        private Dictionary<int, NetworkEntry> _networkBase = new Dictionary<int, NetworkEntry>();
        private int _resetCounter;
        private bool _writerBlocked;

        private const string DefaultLogEntryRules = "date + time + mcc + mnc + la + cc + carrier + slot + callid + type + from + to + encryption + duplex";
        private const string DefaultLogFileNameRules = "date \\ frequency \\ mcc \"_\" mnc \"_\" la";
        private const string DefaultLogSeparator = " ; ";
        private bool _needGroupsUpdate;
        private int _lastDateTime;
        private float _prevAngle;
        private int _afcCounter;
        private double _freqError;
        private float _averageAngle;
        private bool _isAfcWork;
        private Mode _tetraMode;
        private UnsafeBuffer _diBitsBuffer;
        private unsafe byte* _diBitsBufferPtr;

        private const int CallTimeout = 10;
        private int _currentChPriority;

        #region Init and store settings
        public unsafe TetraPanel(ISharpControl control)
        {
            try
            {
                InitializeComponent();

                InitArrays();

                _settingsPersister = new SettingsPersister("tetraSettings.xml");
                _tetraSettings = _settingsPersister.ReadStored();

                #region Default Settings
                if (_tetraSettings.LogEntryRules == null || _tetraSettings.LogEntryRules == string.Empty)
                    _tetraSettings.LogEntryRules = DefaultLogEntryRules;

                if (_tetraSettings.LogFileNameRules == null || _tetraSettings.LogFileNameRules == string.Empty)
                    _tetraSettings.LogFileNameRules = DefaultLogFileNameRules;

                if (_tetraSettings.LogSeparator == null || _tetraSettings.LogSeparator == string.Empty)
                    _tetraSettings.LogSeparator = DefaultLogSeparator;

                if (_tetraSettings.NetworkBase == null)
                    _tetraSettings.NetworkBase = new List<GroupsEntries>();

                if (_tetraSettings.UdpPort == 0) _tetraSettings.UdpPort = 20025;
                #endregion

                UpdateGlobals();

                blockNumericUpDown.Value = _tetraSettings.BlockedLevel;

                _networkBase = NetworkBaseDeserializer(_tetraSettings.NetworkBase);
                _needGroupsUpdate = true;

                _controlInterface = control;

                _infoWindow = new NetInfoWindow();
                _infoWindow.FormClosing += _infoWindow_FormClosing;

                _ifProcessor = new IFProcessor();
                _controlInterface.RegisterStreamHook(_ifProcessor, ProcessorType.DecimatedAndFilteredIQ);
                _ifProcessor.IQReady += IQSamplesAvailable;

                _audioProcessor = new AudioProcessor();
                _controlInterface.RegisterStreamHook(_audioProcessor, ProcessorType.DemodulatorOutput);
                _audioProcessor.AudioReady += AudioSamplesNedeed;

                _decoder = new TetraDecoder(this);
                _decoder.DataReady += _decoder_DataReady;
                _decoder.SyncInfoReady += _decoder_SyncInfoReady;

                _demodulator = new Demodulator();

                _controlInterface.PropertyChanged += _controlInterface_PropertyChanged;

                _displayBuffer = UnsafeBuffer.Create(BurstLengthBits / 2, sizeof(float));
                _displayBufferPtr = (float*)_displayBuffer;

                _outAudioBuffer = UnsafeBuffer.Create(480, sizeof(float));
                _outAudioBufferPtr = (float*)_outAudioBuffer;

                _audioStreamChannel = new FloatFifoStream(BlockMode.None);

                autoCheckBox.Checked = _tetraSettings.AutoPlay;
                AutoCheckBox_CheckedChanged(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tetra plugin exception -" + ex.ToString());
            }
        }

        private void InitArrays()
        {
            for (int i = 0; i < 4; i++)
            {
                _currentCellLoad[i] = new CurrentLoad();
            }
        }

        private Dictionary<int, NetworkEntry> NetworkBaseDeserializer(List<GroupsEntries> list)
        {
            if (list.Count == 0) return new Dictionary<int, NetworkEntry>();

            var result = new Dictionary<int, NetworkEntry>();

            foreach (var entry in list)
            {
                if (result.ContainsKey(entry.NMI))
                {
                    var newGroup = new GroupsEntry
                    {
                        Name = entry.Name,
                        Priority = entry.Priopity
                    };
                    result[entry.NMI].KnowGroups.Add(entry.GSSI, newGroup);
                }
                else
                {
                    var newLine = new NetworkEntry
                    {
                        KnowGroups = new Dictionary<int, GroupsEntry>()
                    };
                    var newGroup = new GroupsEntry
                    {
                        Name = entry.Name,
                        Priority = entry.Priopity
                    };
                    newLine.KnowGroups.Add(entry.GSSI, newGroup);
                    result.Add(entry.NMI, newLine);
                }
            }

            return result;
        }
        private List<GroupsEntries> NetworkBaseSerializer(Dictionary<int, NetworkEntry> dict)
        {
            if (dict.Count == 0) return new List<GroupsEntries>();

            var result = new List<GroupsEntries>();

            foreach (var entry in dict)
            {
                foreach (var groupEntry in entry.Value.KnowGroups)
                {
                    var newEntry = new GroupsEntries
                    {
                        NMI = entry.Key,
                        GSSI = groupEntry.Key,
                        Name = groupEntry.Value.Name,
                        Priopity = groupEntry.Value.Priority
                    };

                    result.Add(newEntry);
                }
            }

            return result;
        }

        private void _infoWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            _needCloseInfo = true;
        }
        public void SaveSettings()
        {
            if (_processIsStarted) StopDecoding();
            _tetraSettings.AutoPlay = autoCheckBox.Checked;
            _tetraSettings.NetworkBase = NetworkBaseSerializer(_networkBase);
            _settingsPersister.PersistStored(_tetraSettings);
        }

        private void _controlInterface_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "StartRadio":
                    break;

                case "StopRadio":
                    break;

                case "Frequency":
                    if (!_isAfcWork)
                    {
                        ResetDecoder();
                    }
                    _isAfcWork = false;
                    break;

                case "DetectorType":

                    break;
            }
        }

        private void ResetDecoder()
        {
            _resetCounter = 10;

            _freqError = 0;

            _currentCell_NMI = 0;
            _currentCell_MNC = 0;
            _currentCell_MCC = 0;
            _currentCell_LA = 0;
            _currentCell_CC = 0;
            _mainCell_Frequency = 0;
            _mainCell_Carrier = 0;

            _activeCounter1 = 0;
            _activeCounter2 = 0;
            _activeCounter3 = 0;
            _activeCounter4 = 0;

            _ch1IsActive = false;
            _ch2IsActive = false;
            _ch3IsActive = false;
            _ch4IsActive = false;

            _currentChPriority = int.MinValue;

            _needGroupsUpdate = true;

            _currentCell_Carrier = (int)Math.Round((_controlInterface.Frequency % 100000000) / 25000.0);

            _sysInfo.Clear();
            _currentCalls.Clear();
            _cmceData.Clear();
            _rawData.Clear();
            _syncInfo.Clear();

            _infoWindow.ResetInfo();

            InitArrays();

        }
        #endregion

        #region DQPSK demodulator

        private void StartDecoding()
        {
            _ifProcessor.Enabled = true;

            _processIsStarted = true;

            DecoderStart();
        }

        private void StopDecoding()
        {
            _ifProcessor.Enabled = false;

            _processIsStarted = false;

            DecoderStop();
        }

        /**
         * Verify if SDR# it's ready to decode tetra signals.
         */
        private bool CheckConditions()
        {
            this._ifProcessor.SampleRate = 25000;
            this._controlInterface.StartRadio();
            this._controlInterface.DetectorType = DetectorType.WFM;
            this._controlInterface.FrequencyShift = 28000;

            if (_ifProcessor.SampleRate < 25000)
            {
                if (System.Globalization.CultureInfo.CurrentCulture.Name == "ru-RU")
                {
                    MessageBox.Show("Слишком низкая частота дискретизации IF. Измените вид модуляции на WFM или установите параметр minOutputSampleRate value = 32000", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show("IF samplerate too low.  Change the modulation type to WFM or set the parameter minOutputSampleRate value = 32000", "Error",
                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                return false;
            }
            return true;
        }

        /// <summary>
        /// @todo VERIFY!!!
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="samplerate"></param>
        /// <param name="length"></param>
        public unsafe void IQSamplesAvailable(Complex* samples, double samplerate, int length)
        {
            this._iqSamplerate = samplerate;
            if (this._radioFifoBuffer == null)
                this._radioFifoBuffer = new ComplexFifoStream(BlockMode.None);
            if ((double)this._radioFifoBuffer.Length < samplerate)
                this._radioFifoBuffer.Write(samples, length);
            else
                ++this._lostBuffers;
        }

        private void AutomaticFrequencyControl(float* buffer, int length)
        {
            var found = false;
            var resultAngel = 0.0f;

            for (int i = 0; i < length; i++)
            {
                if (Math.Abs(buffer[i] - _prevAngle) < (PiDivFor))
                {
                    _afcCounter++;
                    _averageAngle += buffer[i];
                }
                else
                {
                    found = (_afcCounter == 31) || (_afcCounter == 32);
                    resultAngel = _averageAngle / _afcCounter;

                    _afcCounter = 0;
                    _averageAngle = 0;
                }

                _prevAngle = buffer[i];

                if (found)
                {
                    _freqError = _freqError * 0.9f + (0.1f * ((resultAngel - PiDivFor) / (TwoPi / 18000.0f)));
                    break;
                }
            }
        }

        #endregion

        #region Tetra-decoder

        private void DecoderStart()
        {
            _decodingIsStarted = true;
            _decodingThread = new Thread(DecodingThread)
            {
                Priority = ThreadPriority.Normal,
                Name = "TetraDecocerThread"
            };
            _decodingThread.Start();

            _audioProcessor.Enabled = true;
            _needDisplayBufferUpdate = true;
        }

        private void DecoderStop()
        {
            _audioProcessor.Enabled = false;

            _decodingIsStarted = false;

            if (_decodingThread != null)
            {
                _decodingThread.Join();
                _decodingThread = null;
            }
            _controlInterface.AudioIsMuted = false;
        }

        /**
         * Thread que espera a que se cargue el radiobuffer y luego lo procesa.
         * Buffer de 510 Bits
         * @todo verify and traspose
         */
        private unsafe void DecodingThread()
        {
            _iqBuffer = UnsafeBuffer.Create(SamplesPerBurst, sizeof(Complex));
            _iqBufferPtr = (Complex*)_iqBuffer;

            _symbolsBuffer = UnsafeBuffer.Create(BurstLengthSymbols, sizeof(float));
            _symbolsBufferPtr = (float*)_symbolsBuffer;

            _diBitsBuffer = UnsafeBuffer.Create(BurstLengthBits, sizeof(byte));
            _diBitsBufferPtr = (byte*)_diBitsBuffer;

            UdpClient server = new UdpClient("127.0.0.1", _tetraSettings.UdpPort);

            var audioSamplerate = 0d;

            var burst = new Burst
            {
                Ptr = _diBitsBufferPtr
            };
            while (this._decodingIsStarted)
            {
                if ((_radioFifoBuffer == null) || (_radioFifoBuffer.Length < SamplesPerBurst) || (_iqSamplerate == 0))
                {
                    Thread.Sleep(10);
                    continue;
                }

                burst.Mode = this._tetraMode;

                ///@todo CRITICAL PENDING
                ///Original
                ///_radioFifoBuffer.Read(_iqBufferPtr, SamplesPerBurst);
                ///_demodulator.ProcessBuffer(burst, _iqBufferPtr, _symbolsBufferPtr);
                ///SamplesPerBurst 255 *4 -> 1020
                ///BurstLengthBits -> 510
                ///Con 1024 no encuentra los paquetes!!!

                this._radioFifoBuffer.Read(this._iqBufferPtr, BurstLengthBits);
                this._demodulator.ProcessBuffer(burst, this._iqBufferPtr, this._iqSamplerate, BurstLengthBits, this._symbolsBufferPtr);
                /// END CRITICAL

                if (burst.Type == BurstType.WaitBurst)
                    continue;

                AutomaticFrequencyControl(_symbolsBufferPtr, BurstLengthSymbols);

                if (_tetraSettings.UdpEnabled)
                {
                    server.SendAsync(ConvertAngleToDiBits(_symbolsBufferPtr, BurstLengthSymbols), BurstLengthBits);
                }

                var audioChannel = this._decoder.Process(burst, this._outAudioBufferPtr);

                _tetraMode = _decoder.TetraMode;

                if (_needDisplayBufferUpdate)// && _decoder.HaveErrors)
                {
                    _needDisplayBufferUpdate = false;

                    Utils.Memcpy(_displayBufferPtr, _symbolsBufferPtr, _displayBuffer.Length * sizeof(float));

                    _dispayBufferReady = true;
                }

                if (audioChannel == 0 || _audioSamplerate == 0) continue;

                if (audioSamplerate != _audioSamplerate)
                {
                    audioSamplerate = _audioSamplerate;
                    _audioResampler = new Resampler(8000, audioSamplerate);

                    _resampledAudio = UnsafeBuffer.Create((int)audioSamplerate, sizeof(float));
                    _resampledAudioPtr = (float*)_resampledAudio;
                }

                switch (audioChannel)
                {
                    case 1:
                        _ch1IsActive = true;
                        _activeCounter1 = ChannelActiveDelay;
                        if (!_channel1Listen) continue;
                        break;
                    case 2:
                        _ch2IsActive = true;
                        _activeCounter2 = ChannelActiveDelay;
                        if (!_channel2Listen) continue;
                        break;
                    case 3:
                        _ch3IsActive = true;
                        _activeCounter3 = ChannelActiveDelay;
                        if (!_channel3Listen) continue;
                        break;
                    case 4:
                        _ch4IsActive = true;
                        _activeCounter4 = ChannelActiveDelay;
                        if (!_channel4Listen) continue;
                        break;
                }
                //resample buffer
                var audioLength = _audioResampler.Process(_outAudioBufferPtr, _resampledAudioPtr, _outAudioBuffer.Length);
                //Clone to stereo
                // audioLength = MonoToStereo(_resampledAudioPtr, audioLength);
                // Copy to output fifo
                _audioStreamChannel.Write(_resampledAudioPtr, audioLength);

            }

            _iqBuffer.Dispose();
            _iqBuffer = null;
            _iqBufferPtr = null;

            _symbolsBuffer.Dispose();
            _symbolsBuffer = null;
            _symbolsBufferPtr = null;

            _diBitsBuffer.Dispose();
            _diBitsBuffer = null;
            _diBitsBufferPtr = null;
        }

        private void ConvertAngleToDiBits(byte* bitsBuffer, float* angles, int sourceLength)
        {
            float delta;

            while (sourceLength-- > 0)
            {
                delta = *angles++;

                *bitsBuffer++ = delta < 0 ? (byte)1 : (byte)0;
                *bitsBuffer++ = Math.Abs(delta) > PiDivTwo ? (byte)1 : (byte)0;
            }
        }

        private byte[] ConvertAngleToDiBits(float* angles, int sourceLength)
        {
            var bitsBuffer = new byte[sourceLength * 2];
            float delta;
            int indexout = 0;

            while (sourceLength-- > 0)
            {
                delta = *angles++;

                bitsBuffer[indexout++] = delta < 0 ? (byte)1 : (byte)0;
                bitsBuffer[indexout++] = Math.Abs(delta) > PiDivTwo ? (byte)1 : (byte)0;
            }

            return bitsBuffer;
        }

        private int MonoToStereo(float* buffer, int monoLength)
        {
            var monoIndex = monoLength - 1;
            var stereoIndex = monoLength * 2 - 1;

            for (int i = 0; i < monoLength; i++)
            {
                buffer[stereoIndex--] = buffer[monoIndex];
                buffer[stereoIndex--] = buffer[monoIndex];
                monoIndex--;
            }

            return monoLength * 2;
        }

        #endregion

        #region Audio Out

        public void AudioSamplesNedeed(float* samples, double samplerate, int length)
        {
            _audioSamplerate = samplerate;

            if (_audioStreamChannel.Length >= length)
            {
                _audioStreamChannel.Read(samples, length);
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    samples[i] = 0;
                }
            }
        }

        #endregion

        #region Received Data Extractor

        //private const int CallTimeout = 10;
        //private int _currentChPriority;

        void _decoder_DataReady(List<ReceivedData> data)
        {

            ////Debug.WriteLine("data delegate " + Thread.CurrentThread.ManagedThreadId.ToString());

            if (_resetCounter > 0)
            {
                return;
            }

            while (data.Count > 0)
            {
                if (_rawData.Count < 100)
                    _rawData.Add(data[0]);

                data.RemoveAt(0);
            }
        }

        public void UpdateCallsInfo(ReceivedData data)
        {
            if (!data.Contains(GlobalNames.CMCE_Primitives_Type)) return;

            var callId = 0;

            if (!data.TryGetValue(GlobalNames.Call_identifier, ref callId))
                return;

            var ssi = 0;
            var type = 0;
            var value = 0;

            var logEvent = false;

            if (!_networkBase.ContainsKey(_currentCell_NMI))
            {
                var entry = new NetworkEntry
                {
                    KnowGroups = new Dictionary<int, GroupsEntry>()
                };
                _networkBase.Add(_currentCell_NMI, entry);

                var newGroup = new GroupsEntry
                {
                    Name = "Individual",
                    Priority = 0
                };
                _networkBase[_currentCell_NMI].KnowGroups.Add(0, newGroup);

                _needGroupsUpdate = true;
            }

            data.TryGetValue(GlobalNames.SSI, ref ssi);

            if (data.TryGetValue(GlobalNames.Basic_service_Communication_type, ref value))
            {
                type = value;

                if (value == (int)CommunicationType.Group)
                {
                    if (!_networkBase[_currentCell_NMI].KnowGroups.ContainsKey(ssi))
                    {
                        var newGroup = new GroupsEntry
                        {
                            Name = string.Empty,
                            Priority = 0
                        };
                        _networkBase[_currentCell_NMI].KnowGroups.Add(ssi, newGroup);
                        _needGroupsUpdate = true;
                    }
                }
            }

            if (!_currentCalls.ContainsKey(callId))
            {
                var entry = new CallsEntry
                {
                    To = ssi,
                    CallID = callId,
                    From = 0,
                    Type = type,
                    IsClear = 0,
                    Duplex = 0
                };

                _currentCalls.Add(callId, entry);

                logEvent = true;
            }

            _currentCalls[callId].To = ssi;

            if (data.TryGetValue(GlobalNames.Basic_service_Communication_type, ref value))
            {
                _currentCalls[callId].Type = value;
            }

            if (data.TryGetValue(GlobalNames.Carrier_number, ref value))
            {
                _currentCalls[callId].Carrier = value;
            }

            _currentCalls[callId].WatchDog = 5;

            var transmGrant = 0;
            var timeslot = data.Value(GlobalNames.CurrTimeSlot);
            var assignedSlot = -1;
            var fromNew = -1;

            switch ((CmcePrimitivesType)data.Value(GlobalNames.CMCE_Primitives_Type))
            {
                case CmcePrimitivesType.D_Disconnect:
                case CmcePrimitivesType.D_Release:
                    assignedSlot = 0;
                    fromNew = 0;
                    _currentCalls[callId].WatchDog = 5;
                    break;

                case CmcePrimitivesType.D_TX_Ceased:
                    fromNew = 0;
                    _currentCalls[callId].WatchDog = 5;
                    break;

                case CmcePrimitivesType.D_Info:
                    if (data.TryGetValue(GlobalNames.Slot_granting_element, ref value))
                    {
                        if (data.TryGetValue(GlobalNames.SSI, ref value))
                        {
                            fromNew = value;
                        }

                        if (data.TryGetValue(GlobalNames.Timeslot_assigned, ref value))
                        {
                            assignedSlot = value;
                        }
                        else
                        {
                            assignedSlot = 0x10 >> timeslot;
                        }
                    }
                    break;

                case CmcePrimitivesType.D_Connect:
                case CmcePrimitivesType.D_Setup:
                case CmcePrimitivesType.D_TX_Granted:

                    if (data.TryGetValue(GlobalNames.Calling_party_address_SSI, ref value))
                    {
                        _currentCalls[callId].From = value;
                    }

                    if (data.TryGetValue(GlobalNames.Transmitting_party_address_SSI, ref value))
                    {
                        _currentCalls[callId].From = value;
                    }

                    var pduEncrypted = false;
                    var baseEncrypted = false;
                    var encrypt = false;

                    if (data.TryGetValue(GlobalNames.Encryption_mode, ref value))
                    {
                        pduEncrypted = value != 0;
                    }

                    if (data.TryGetValue(GlobalNames.Encryption_control, ref value))
                    {
                        encrypt = value != 0;
                    }

                    if (data.TryGetValue(GlobalNames.Basic_service_Encryption_flag, ref value))
                    {
                        baseEncrypted = value != 0;
                    }

                    _currentCalls[callId].IsClear = (!pduEncrypted && !baseEncrypted && !encrypt) ? 1 : 0;

                    if (data.TryGetValue(GlobalNames.Simplex_duplex, ref value))
                    {
                        _currentCalls[callId].Duplex = value;
                    }

                    if (data.TryGetValue(GlobalNames.Timeslot_assigned, ref value))
                    {
                        assignedSlot = value;
                    }
                    else
                    {
                        assignedSlot = 0x10 >> timeslot;
                    }

                    if (data.TryGetValue(GlobalNames.Transmission_grant, ref transmGrant))
                    {
                        switch ((TransmissionGranted)transmGrant)
                        {
                            case TransmissionGranted.Granted:
                                if (data.TryGetValue(GlobalNames.SSI, ref value))
                                {
                                    fromNew = value;
                                }
                                break;

                            case TransmissionGranted.Granted_to_another_user:
                                if (data.TryGetValue(GlobalNames.Calling_party_address_SSI, ref value))
                                {
                                    fromNew = value;
                                }
                                else if (data.TryGetValue(GlobalNames.Transmitting_party_address_SSI, ref value))
                                {
                                    fromNew = value;
                                }
                                break;

                            default:
                                fromNew = 0;
                                break;
                        }
                    }

                    _currentCalls[callId].WatchDog = 20;

                    break;
            }

            if (assignedSlot != -1)
            {
                _currentCalls[callId].AssignedSlot = assignedSlot;
            }

            if (fromNew != -1)
            {
                if (_currentCalls[callId].From != fromNew)
                {
                    _currentCalls[callId].From = fromNew;
                    logEvent = true;
                }
            }

            if (logEvent)
            {
                Log_Tick(_currentCalls[callId]);
            }
        }

        void _decoder_SyncInfoReady(ReceivedData syncInfo)
        {
            if (_resetCounter > 0)
            {
                return;
            }

            syncInfo.Data.CopyTo(_syncInfo.Data, 0);
        }

        private void UpdateSysInfo(ReceivedData data)
        {
            if ((MAC_PDU_Type)data.Value(GlobalNames.MAC_PDU_Type) == MAC_PDU_Type.Broadcast)
            {
                data.Data.CopyTo(_sysInfo.Data, 0);

                _sysInfo.TryGetValue(GlobalNames.Location_Area, ref _currentCell_LA);

                var band = 0;
                var offset = 0;
                var carrier = 0;

                var isFull = data.TryGetValue(GlobalNames.Frequency_Band, ref band)
                    && data.TryGetValue(GlobalNames.Main_Carrier, ref carrier)
                    && data.TryGetValue(GlobalNames.Offset, ref offset);

                _mainCell_Frequency = Global.FrequencyCalc(isFull, carrier, band, offset);
                _mainCell_Carrier = carrier;

                _currentCell_Carrier = Global.CarrierCalc(_controlInterface.Frequency);
            }
        }

        #endregion

        #region GUI events

        private void MarkerTimer_Tick(object sender, EventArgs e)
        {
            if (_displayBuffer != null)
            {
                if (_dispayBufferReady)
                {
                    _dispayBufferReady = false;
                    display.Perform(_displayBufferPtr, _displayBuffer.Length);
                    display.Refresh();
                    _needDisplayBufferUpdate = true;
                }
            }

            if (!_processIsStarted) return;

            label1.Text = _currentCellLoad[0].From.ToString();
            ch1RadioButton.ForeColor = _ch1IsActive ? Color.Red : Color.Gray;

            label2.Text = _currentCellLoad[1].From.ToString();
            ch2RadioButton.ForeColor = _ch2IsActive ? Color.Red : Color.Gray;

            label3.Text = _currentCellLoad[2].From.ToString();
            ch3RadioButton.ForeColor = _ch3IsActive ? Color.Red : Color.Gray;

            label4.Text = _currentCellLoad[3].From.ToString();
            ch4RadioButton.ForeColor = _ch4IsActive ? Color.Red : Color.Gray;

            label9.Text = (_currentCellLoad[0].Type == 1 ? "g " : "") + _currentCellLoad[0].GroupName;
            label8.Text = (_currentCellLoad[1].Type == 1 ? "g " : "") + _currentCellLoad[1].GroupName;
            label7.Text = (_currentCellLoad[2].Type == 1 ? "g " : "") + _currentCellLoad[2].GroupName;
            label6.Text = (_currentCellLoad[3].Type == 1 ? "g " : "") + _currentCellLoad[3].GroupName;

            _activeCounter1--;
            if (_activeCounter1 < 0)
            {
                _activeCounter1 = 0;
                _ch1IsActive = false;
            }

            _activeCounter2--;
            if (_activeCounter2 < 0)
            {
                _activeCounter2 = 0;
                _ch2IsActive = false;
            }

            _activeCounter3--;
            if (_activeCounter3 < 0)
            {
                _activeCounter3 = 0;
                _ch3IsActive = false;
            }

            _activeCounter4--;
            if (_activeCounter4 < 0)
            {
                _activeCounter4 = 0;
                _ch4IsActive = false;
            }

            if (_autoSelectChannel)
            {
                var maxPriority = (int)blockNumericUpDown.Value;
                var priorityChannel = 0;

                if ((_currentCellLoad[0].From != 0) && (_currentCellLoad[0].GroupPriority >= maxPriority) && (_currentCellLoad[0].GroupPriority > _currentChPriority))
                {
                    if (_currentCellLoad[0].IsClear || !Global.IgnoreEncryptedSpeech)
                    {
                        maxPriority = _currentCellLoad[0].GroupPriority;
                        priorityChannel = 1;
                    }
                }

                if ((_currentCellLoad[1].From != 0) && (_currentCellLoad[1].GroupPriority >= maxPriority) && (_currentCellLoad[1].GroupPriority > _currentChPriority))
                {
                    if (_currentCellLoad[1].IsClear || !Global.IgnoreEncryptedSpeech)
                    {
                        maxPriority = _currentCellLoad[1].GroupPriority;
                        priorityChannel = 2;
                    }
                }

                if ((_currentCellLoad[2].From != 0) && (_currentCellLoad[2].GroupPriority >= maxPriority) && (_currentCellLoad[2].GroupPriority > _currentChPriority))
                {
                    if (_currentCellLoad[2].IsClear || !Global.IgnoreEncryptedSpeech)
                    {
                        maxPriority = _currentCellLoad[2].GroupPriority;
                        priorityChannel = 3;
                    }
                }

                if ((_currentCellLoad[3].From != 0) && (_currentCellLoad[3].GroupPriority >= maxPriority) && (_currentCellLoad[3].GroupPriority > _currentChPriority))
                {
                    if (_currentCellLoad[3].IsClear || !Global.IgnoreEncryptedSpeech)
                    {
                        maxPriority = _currentCellLoad[3].GroupPriority;
                        priorityChannel = 4;
                    }
                }

                if (priorityChannel != 0)
                {
                    if (priorityChannel == 1 && !_channel1Listen) { ch1RadioButton.Checked = true; _currentChPriority = _currentCellLoad[0].GroupPriority; }
                    else if (priorityChannel == 2 && !_channel2Listen) { ch2RadioButton.Checked = true; _currentChPriority = _currentCellLoad[1].GroupPriority; }
                    else if (priorityChannel == 3 && !_channel3Listen) { ch3RadioButton.Checked = true; _currentChPriority = _currentCellLoad[2].GroupPriority; }
                    else if (priorityChannel == 4 && !_channel4Listen) { ch4RadioButton.Checked = true; _currentChPriority = _currentCellLoad[3].GroupPriority; }
                }
                else
                {
                    if (_channel1Listen && _currentCellLoad[0].From == 0) { ch1RadioButton.Checked = false; _currentChPriority = int.MinValue; }
                    if (_channel2Listen && _currentCellLoad[1].From == 0) { ch2RadioButton.Checked = false; _currentChPriority = int.MinValue; }
                    if (_channel3Listen && _currentCellLoad[2].From == 0) { ch3RadioButton.Checked = false; _currentChPriority = int.MinValue; }
                    if (_channel4Listen && _currentCellLoad[3].From == 0) { ch4RadioButton.Checked = false; _currentChPriority = int.MinValue; }
                }
            }

            _controlInterface.AudioIsMuted = !(_channel1Listen || _channel2Listen || _channel3Listen || _channel4Listen);

            if (_decoder != null)
            {
                colorLabel.Visible = _tetraMode == Mode.TMO;
                laLabel.Visible = _tetraMode == Mode.TMO;

                mccLabel.Text = "MCC:" + _currentCell_MCC.ToString();
                mncLabel.Text = "MNC:" + _currentCell_MNC.ToString();
                colorLabel.Text = "Color:" + _currentCell_CC.ToString();
                connectLabel.Visible = _decoder.BurstReceived;
                laLabel.Text = "LA:" + _currentCell_LA.ToString();
                mainCarrierLabel.Text = _mainCell_Carrier.ToString();
                mainFrequencyLinkLabel.Text = string.Format("{0:0,0.000###} MHz", _mainCell_Frequency * 0.000001m);
                currentCarrierLabel.Text = _currentCell_Carrier.ToString();
                currentFrequencyLabel.Text = string.Format("{0:0,0.000###} MHz", _controlInterface.Frequency * 0.000001m);
            }
        }

        private void EnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (enabledCheckBox.Checked && !_processIsStarted)
            {
                if (CheckConditions() == false)
                {
                    enabledCheckBox.Checked = false;
                    return;
                }

                ResetDecoder();

                StartDecoding();
            }
            else if (!enabledCheckBox.Checked && _processIsStarted)
            {
                StopDecoding();
            }
        }

        private void TimerGui_Tick(object sender, EventArgs e)
        {
            label11.Text = _lostBuffers.ToString();

            if (_decoder != null)
            {
                berLabel.Text = string.Format("BER {0:0.0}%", _decoder.Ber);
                merLabel.Text = string.Format("MER {0:0.0}%", _decoder.Mer);
            }

            if (_infoWindow == null) return;

            if (_resetCounter > 0) return;

            if (_showInfo)
            {
                _infoWindow.UpdateTextBox(_cmceData);

                _infoWindow.UpdateCalls(_currentCalls);

                _infoWindow.UpdateSysInfo(_syncInfo, _sysInfo);

                _infoWindow.UpdateNeighBour();

                if (_needCloseInfo)
                {
                    _tetraSettings.TopMostInfo = _infoWindow.TopMost;
                    _showInfo = false;
                    _needCloseInfo = false;
                    _infoWindow.Visible = false;
                }
            }

            if (_infoWindow.GroupsChanged)
            {
                var groupsList = _infoWindow.GetUpdatedGroups();
                _networkBase[_currentCell_NMI].KnowGroups.Clear();

                foreach (var entry in groupsList)
                {
                    var newEntry = new GroupsEntry
                    {
                        Name = entry.Name,
                        Priority = entry.Priority
                    };

                    _networkBase[_currentCell_NMI].KnowGroups.Add(entry.GSSI, newEntry);
                }
                _infoWindow.GroupsChanged = false;
            }

            if (_needGroupsUpdate)
            {
                if (_currentCell_NMI != 0 && _networkBase.ContainsKey(_currentCell_NMI))
                {
                    _needGroupsUpdate = false;

                    var groupsList = new List<GroupDisplay>();

                    foreach (var entry in _networkBase[_currentCell_NMI].KnowGroups)
                    {
                        var newEntry = new GroupDisplay
                        {
                            GSSI = entry.Key,
                            Name = entry.Value.Name,
                            Priority = entry.Value.Priority
                        };

                        groupsList.Add(newEntry);
                    }

                    _infoWindow.UpdateGroupGrid(groupsList);
                }
            }
        }

        private void Ch1RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _channel1Listen = ch1RadioButton.Checked;
        }

        private void Ch2RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _channel2Listen = ch2RadioButton.Checked;
        }

        private void Ch3RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _channel3Listen = ch3RadioButton.Checked;
        }

        private void Ch4RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _channel4Listen = ch4RadioButton.Checked;
        }

        private void AutoCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _autoSelectChannel = autoCheckBox.Checked;
            if (_autoSelectChannel)
            {
                ch1RadioButton.Checked = true;
            }
        }

        private void NetInfoButton_Click(object sender, EventArgs e)
        {
            if (_infoWindow.Visible == true)
            {
                _needCloseInfo = true;
            }
            else
            {
                _infoWindow.TopMost = _tetraSettings.TopMostInfo;
                _infoWindow.Show();
                _showInfo = true;
            }
        }

        private void ConfigButton_Click(object sender, EventArgs e)
        {
            var configureDialog = new DialogConfigure(_tetraSettings, this);
            configureDialog.ShowDialog();

            UpdateGlobals();
        }

        private void UpdateGlobals()
        {
            Global.IgnoreEncryptedSpeech = _tetraSettings.IgnoreEncodedSpeech;
        }

        private void BlockNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _tetraSettings.BlockedLevel = (int)blockNumericUpDown.Value;
        }

        private void MainFrequencyLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _controlInterface.Frequency = _mainCell_Frequency;
        }

        #endregion

        #region Log writer

        private void Log_Tick(CallsEntry currentCalls)
        {
            if (!_tetraSettings.LogEnabled || !_processIsStarted) return;

            var line = ParseStringToEntries(_tetraSettings.LogEntryRules, currentCalls);
            var path = MakeFileName(_tetraSettings.LogWriteFolder, _tetraSettings.LogFileNameRules, ".csv");

            try
            {
                _textFile.Write(line, path);
            }
            catch
            {
                if (_writerBlocked) return;

                _writerBlocked = true;

                if (MessageBox.Show("Unable to open file " + path, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    _writerBlocked = false;
                }
            }
        }

        public string ParseStringToEntries(string entryString, CallsEntry call)
        {
            var dateString = DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            var timeString = DateTime.Now.ToString("HH:mm:ss.fff");
            var mccString = _currentCell_MCC.ToString();
            var mncString = _currentCell_MNC.ToString();
            var laString = _currentCell_LA.ToString();
            var ccString = _currentCell_CC.ToString();
            var typeString = call != null ? call.Type.ToString() : "unknown";
            var fromString = call != null ? call.From.ToString() : "unknown";
            var toString = call != null ? call.To.ToString() : "unknown";
            var encryptString = call != null ? call.IsClear == 1 ? "Clear" : "Encrypted" : "unknown";
            var duplexString = call != null ? call.Duplex == 0 ? "Simplex" : "Duplex" : "unknown";
            var tsString = call != null ? call.AssignedSlot.ToString() : "unknown";
            var idString = call != null ? call.CallID.ToString() : "unknown";
            var carrierString = call != null ? call.Carrier.ToString() : "unknown"; ;

            var entry = (string)"";
            var index = 0;

            while (index < entryString.Length)
            {
                if (CompareString(entryString, "date", index))
                {
                    index += "date".Length;
                    entry += dateString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "time", index))
                {
                    index += "time".Length;
                    entry += timeString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "carrier", index))
                {
                    index += "carrier".Length;
                    entry += carrierString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "mnc", index))
                {
                    index += "mnc".Length;
                    entry += mncString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "mcc", index))
                {
                    index += "mcc".Length;
                    entry += mccString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "la", index))
                {
                    index += "la".Length;
                    entry += laString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "cc", index))
                {
                    index += "cc".Length;
                    entry += ccString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "type", index))
                {
                    index += "type".Length;
                    entry += typeString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "from", index))
                {
                    index += "from".Length;
                    entry += fromString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "to", index))
                {
                    index += "to".Length;
                    entry += toString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "encryption", index))
                {
                    index += "encryption".Length;
                    entry += encryptString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "duplex", index))
                {
                    index += "duplex".Length;
                    entry += duplexString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "slot", index))
                {
                    index += "slot".Length;
                    entry += tsString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "callid", index))
                {
                    index += "callid".Length;
                    entry += idString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "+", index) || CompareString(entryString, " ", index))
                {
                    index++;
                }
                else if (CompareString(entryString, "\"", index))
                {
                    index++;
                    var end = entryString.IndexOf('"', index);

                    if (end <= 0) return entry + "-error!";

                    for (var i = index; i <= end; i++)
                    {
                        var currentChar = entryString[i];
                        index++;
                        entry += currentChar;
                    }
                    entry += _tetraSettings.LogSeparator;
                }
                else
                {
                    return entry + "-error!";
                }
            }

            return entry;
        }

        private static string GetFrequencyDisplay(long frequency)
        {
            string result;
            var absFrequency = Math.Abs(frequency);
            if (absFrequency == 0)
            {
                result = "DC";
            }
            else if (absFrequency > 1500000000)
            {
                result = string.Format("{0:#,0.000 000} GHz", frequency / 1000000000.0);
            }
            else if (absFrequency > 30000000)
            {
                result = string.Format("{0:0,0.000###} MHz", frequency / 1000000.0);
            }
            else if (absFrequency > 1000)
            {
                result = string.Format("{0:#,#.###} kHz", frequency / 1000.0);
            }
            else
            {
                result = frequency.ToString();
            }
            return result;
        }

        private string MakeFileName(string folder, string nameRules, string fileExtension)
        {
            var filename = folder;
            filename += ParseStringToPath(nameRules, fileExtension);
            var dir = filename.Substring(0, filename.LastIndexOf("\\"));
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch
            {
                MessageBox.Show("Unable to create directory", dir, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return filename;
        }

        public string ParseStringToPath(string nameString, string extension)
        {
            var currentFrequencyString = GetFrequencyDisplay(_controlInterface.Frequency);
            var currentDateString = DateTime.Now.ToString("yyyy_MM_dd");
            var currentStartTimeString = DateTime.Now.ToString("HH-mm-ss");
            var mccString = _currentCell_MCC.ToString();
            var mncString = _currentCell_MNC.ToString();
            var laString = _currentCell_LA.ToString();
            var ccString = _currentCell_CC.ToString();
            var groupString = string.Empty;

            var filename = string.Empty; ;
            var index = 0;
            while (index < nameString.Length)
            {
                if (CompareString(nameString, "date", index))
                {
                    index += "date".Length;
                    filename += currentDateString;
                }
                else if (CompareString(nameString, "time", index))
                {
                    index += "time".Length;
                    filename += currentStartTimeString;
                }
                else if (CompareString(nameString, "frequency", index))
                {
                    index += "frequency".Length;
                    filename += currentFrequencyString;
                }
                else if (CompareString(nameString, "mcc", index))
                {
                    index += "mcc".Length;
                    filename += mccString;
                }
                else if (CompareString(nameString, "mnc", index))
                {
                    index += "mnc".Length;
                    filename += mncString;
                }
                else if (CompareString(nameString, "cc", index))
                {
                    index += "cc".Length;
                    filename += ccString;
                }
                else if (CompareString(nameString, "la", index))
                {
                    index += "la".Length;
                    filename += laString;
                }
                else if (CompareString(nameString, "to", index))
                {
                    index += "to".Length;
                    filename += groupString;
                }
                else if (CompareString(nameString, "\\", index) || CompareString(nameString, "/", index))
                {
                    index++;
                    filename += "\\";
                }
                else if (CompareString(nameString, "+", index) || CompareString(nameString, " ", index))
                {
                    index++;
                }
                else if (CompareString(nameString, "\"", index))
                {
                    index++;
                    var end = nameString.IndexOf('"', index);

                    if (end <= 0) return filename + "-error!";

                    for (var i = index; i <= end; i++)
                    {
                        var currentChar = nameString[i];
                        index++;

                        if (currentChar == '?' || currentChar == '/' || currentChar == '\\' ||
                            currentChar == ':' || currentChar == '<' || currentChar == '>' ||
                            currentChar == '|' || currentChar == '*' || currentChar == '"')
                            continue;

                        filename += currentChar;
                    }
                }
                else
                {
                    return filename + "-error!";
                }
            }
            if (filename.Length > 5)
            {
                if (filename.Substring(filename.Length - extension.Length, extension.Length) != extension) filename += extension;
                if (filename.Substring(0, 1) != "\\") filename = "\\" + filename;
            }
            return filename;
        }

        private bool CompareString(string source, string compare, int index)
        {
            if (index + compare.Length > source.Length) return false;
            return source.Substring(index, compare.Length) == compare;
        }

        #endregion

        private void AfcTimer_Tick(object sender, EventArgs e)
        {
            ferLabel.Text = string.Format("FER {0:F0}Hz", _freqError);
            if (!_tetraSettings.AfcDisabled)
            {
                if (_freqError > 200 || _freqError < -200)
                {
                    _isAfcWork = true;
                    _controlInterface.Frequency += (long)_freqError;
                    _freqError = 0;
                }
            }
        }

        private void DataExtractorTimer_Tick(object sender, EventArgs e)
        {
            if (_resetCounter > 0)
            {
                _resetCounter--;
                return;
            }


            if (_syncInfo != null)
            {
                _syncInfo.TryGetValue(GlobalNames.MCC, ref _currentCell_MCC);
                _syncInfo.TryGetValue(GlobalNames.MNC, ref _currentCell_MNC);
                _syncInfo.TryGetValue(GlobalNames.ColorCode, ref _currentCell_CC);

                _currentCell_NMI = (_currentCell_MCC << 14) | _currentCell_MNC;
            }

            while (_rawData.Count > 0)
            {
                UpdateSysInfo(_rawData[0]);

                UpdateCmceInfo(_rawData[0]);

                if (_currentCell_NMI != 0) UpdateCallsInfo(_rawData[0]);

                _rawData.RemoveAt(0);
            }

            if (_currentCell_NMI == 0) return;

            var index = 0;

            if (_lastDateTime != DateTime.Now.Second)
            {
                _lastDateTime = DateTime.Now.Second;

                while (index < _currentCalls.Count)
                {
                    if (_currentCalls.ElementAt(index).Value.WatchDog > 0)
                    {
                        _currentCalls.ElementAt(index).Value.WatchDog--;
                    }
                    else
                    {
                        _currentCalls.Remove(_currentCalls.ElementAt(index).Key);
                        continue;
                    }
                    index++;
                }
            }

            foreach (CallsEntry entry in _currentCalls.Values)
            {
                if (entry.Carrier != _currentCell_Carrier && entry.Carrier != 0) continue;

                var assignedSlot = entry.AssignedSlot;
                var priority = 0;
                var name = "";

                if (_networkBase.ContainsKey(_currentCell_NMI))
                {
                    if (_networkBase[_currentCell_NMI].KnowGroups.ContainsKey(entry.To))
                    {
                        priority = _networkBase[_currentCell_NMI].KnowGroups[entry.To].Priority;
                        name = _networkBase[_currentCell_NMI].KnowGroups[entry.To].Name;
                    }
                }

                if (entry.From != 0)
                {
                    if ((assignedSlot & 0x8) != 0)
                    {
                        _currentCellLoad[0].CallId = entry.CallID;
                        _currentCellLoad[0].Type = entry.Type;
                        _currentCellLoad[0].From = entry.From;
                        _currentCellLoad[0].To = entry.To;
                        _currentCellLoad[0].GroupPriority = priority;
                        _currentCellLoad[0].GroupName = name != "" ? name : entry.To.ToString();
                        _currentCellLoad[0].IsClear = entry.IsClear == 1;

                    }
                    if ((assignedSlot & 0x4) != 0)
                    {
                        _currentCellLoad[1].CallId = entry.CallID;
                        _currentCellLoad[1].Type = entry.Type;
                        _currentCellLoad[1].From = entry.From;
                        _currentCellLoad[1].To = entry.To;
                        _currentCellLoad[1].GroupPriority = priority;
                        _currentCellLoad[1].GroupName = name != "" ? name : entry.To.ToString();
                        _currentCellLoad[1].IsClear = entry.IsClear == 1;
                    }
                    if ((assignedSlot & 0x2) != 0)
                    {
                        _currentCellLoad[2].CallId = entry.CallID;
                        _currentCellLoad[2].Type = entry.Type;
                        _currentCellLoad[2].From = entry.From;
                        _currentCellLoad[2].To = entry.To;
                        _currentCellLoad[2].GroupPriority = priority;
                        _currentCellLoad[2].GroupName = name != "" ? name : entry.To.ToString();
                        _currentCellLoad[2].IsClear = entry.IsClear == 1;
                    }
                    if ((assignedSlot & 0x1) != 0)
                    {
                        _currentCellLoad[3].CallId = entry.CallID;
                        _currentCellLoad[3].Type = entry.Type;
                        _currentCellLoad[3].From = entry.From;
                        _currentCellLoad[3].To = entry.To;
                        _currentCellLoad[3].GroupPriority = priority;
                        _currentCellLoad[3].GroupName = name != "" ? name : entry.To.ToString();
                        _currentCellLoad[3].IsClear = entry.IsClear == 1;
                    }
                }
            }

            if (!_currentCalls.ContainsKey(_currentCellLoad[0].CallId) || _currentCalls[_currentCellLoad[0].CallId].AssignedSlot == 0) _currentCellLoad[0] = new CurrentLoad();
            if (!_currentCalls.ContainsKey(_currentCellLoad[1].CallId) || _currentCalls[_currentCellLoad[1].CallId].AssignedSlot == 0) _currentCellLoad[1] = new CurrentLoad();
            if (!_currentCalls.ContainsKey(_currentCellLoad[2].CallId) || _currentCalls[_currentCellLoad[2].CallId].AssignedSlot == 0) _currentCellLoad[2] = new CurrentLoad();
            if (!_currentCalls.ContainsKey(_currentCellLoad[3].CallId) || _currentCalls[_currentCellLoad[3].CallId].AssignedSlot == 0) _currentCellLoad[3] = new CurrentLoad();

        }

        private void UpdateCmceInfo(ReceivedData data)
        {
            if (!data.Contains(GlobalNames.CMCE_Primitives_Type)) return;

            if (_cmceData.Count > 100)
                _cmceData.RemoveAt(0);

            _cmceData.Add(data);
        }
    }
}
