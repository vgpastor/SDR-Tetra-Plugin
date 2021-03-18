using SDRSharp.Common;
using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace SDRSharp.Tetra
{
    public unsafe partial class TetraPanel : UserControl
    {
        private const float TwoPi = (float)(Math.PI * 2.0);
        private const float Pi = (float)Math.PI;
        private const float PiDivTwo = (float)(Math.PI / 2.0);
        private const float PiDivFor = (float)(Math.PI / 4.0);

        private const float ByteToMb = 1f / 1024f / 1024f;
        private const int OutDataLength = 510;
        private const int DemodulatorBufferLength = 4096;

        private const int ChannelActiveDelay = 10;

        private ISharpControl _controlInterface;
        private IFProcessor _ifProcessor;
        private Demodulator _demodulator;
        private TetraDecoder _decoder;

        private UnsafeBuffer _demodulatorInBuffer;
        private Complex* _demodulatorInBufferPtr;
        private UnsafeBuffer _demodulatorOutBuffer;
        private float* _demodulatorOutBufferPtr;

        private UnsafeBuffer _toDecodeBuffer;
        private float* _toDecodeBufferPtr;
        private ComplexFifoStream _radioFifoBuffer;
        private FloatFifoStream _tetraFifoBuffer;
        private UnsafeBuffer _displayBuffer;
        private float* _displayBufferPtr;

        private UnsafeBuffer _outAudioBuffer;
        private float* _outAudioBufferPtr;
        private UnsafeBuffer _resampledAudio;
        private float* _resampledAudioPtr;
        private Resampler _audioResampler;
        private FloatFifoStream _audioStreamChannel;

        private double _audioSamplerate;

        private Thread _outputThread;
        private bool _outputIsStarted;

        private Thread _workerThread;
        private double _iqSamplerate;
        private bool _processIsStarted;
        private int _lostBuffers;
        private bool _dispayBufferReady;

        private UnsafeBuffer _outputBuffer;
        private byte* _outputBufferPtr;

        private TextFile _textFile = new TextFile();
        private TetraSettings _tetraSettings;
        private SettingsPersister _settingsPersister;

        private bool _needDisplayBufferUpdate;

        private int _fpass;
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

        private List<Dictionary<GlobalNames, int>> _rawData = new List<Dictionary<GlobalNames, int>>();
        private List<Dictionary<GlobalNames, int>> _cmceData = new List<Dictionary<GlobalNames, int>>();
        private List<Dictionary<GlobalNames, int>> _syncData = new List<Dictionary<GlobalNames, int>>();
        private Dictionary<GlobalNames, int> _syncInfo = new Dictionary<GlobalNames, int>();
        private Dictionary<GlobalNames, int> _sysInfo = new Dictionary<GlobalNames, int>();

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

        private const string DefaultLogEntryRules = "date + time + mcc + mnc + la + cc + carrier + slot + callid + group + tx + encryption + duplex";
        private const string DefaultLogFileNameRules = "date \\ frequency \\ mcc \"_\" mnc \"_\" la";
        private const string DefaultLogSeparator = " ; ";
        private bool _needGroupsUpdate;
        private int _lastDateTime;
        private float _prevAngle;
        private int _afcCounter;
        private double _freqError;
        private float _averageAngle;
        private bool _isAfcWork;


        #region Init and store settings

        public TetraPanel(ISharpControl control)
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
                _controlInterface.RegisterStreamHook(_audioProcessor, ProcessorType.FilteredAudioOutput);
                _audioProcessor.AudioReady += AudioSamplesNedeed;

                _decoder = new TetraDecoder(this);
                _decoder.DataReady += _decoder_DataReady;
                _decoder.SyncInfoReady += _decoder_SyncInfoReady;

                _demodulator = new Demodulator();

                _controlInterface.PropertyChanged += _controlInterface_PropertyChanged;

                _displayBuffer = UnsafeBuffer.Create(OutDataLength / 2, sizeof(float));
                _displayBufferPtr = (float*)_displayBuffer;

                _toDecodeBuffer = UnsafeBuffer.Create(OutDataLength / 2, sizeof(float));
                _toDecodeBufferPtr = (float*)_toDecodeBuffer;

                _outputBuffer = UnsafeBuffer.Create(OutDataLength);
                _outputBufferPtr = (byte*)_outputBuffer;

                _outAudioBuffer = UnsafeBuffer.Create(480, sizeof(float));
                _outAudioBufferPtr = (float*)_outAudioBuffer;

                _tetraFifoBuffer = new FloatFifoStream(BlockMode.None);

                _audioStreamChannel = new FloatFifoStream(BlockMode.None);

                autoCheckBox.Checked = _tetraSettings.AutoPlay;
                autoCheckBox_CheckedChanged(null, null);
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
                    var newGroup = new GroupsEntry();
                    newGroup.Name = entry.Name;
                    newGroup.Priority = entry.Priopity;
                    result[entry.NMI].KnowGroups.Add(entry.GSSI, newGroup);
                }
                else
                {
                    var newLine = new NetworkEntry();
                    newLine.KnowGroups = new Dictionary<int, GroupsEntry>();
                    var newGroup = new GroupsEntry();
                    newGroup.Name = entry.Name;
                    newGroup.Priority = entry.Priopity;
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
                    var newEntry = new GroupsEntries();
                    newEntry.NMI = entry.Key;
                    newEntry.GSSI = groupEntry.Key;
                    newEntry.Name = groupEntry.Value.Name;
                    newEntry.Priopity = groupEntry.Value.Priority;

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
            _freqError = 0;
            _resetCounter = 3;

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

            _needGroupsUpdate = true;

            _currentCell_Carrier = (int)Math.Round((_controlInterface.Frequency % 100000000) / 25000.0);

            _sysInfo.Clear();
            _currentCalls.Clear();
            _cmceData.Clear();
            _syncData.Clear();

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

            _workerThread = new Thread(DemodulatorProcess)
            {
                Name = "PSK demodulator",
                Priority = ThreadPriority.Normal
            };
            _workerThread.Start();

            DecoderStart();
        }

        private void StopDecoding()
        {
            _ifProcessor.Enabled = false;

            _processIsStarted = false;

            if (_workerThread != null)
            {
                _workerThread.Join();
                _workerThread = null;
            }

            DecoderStop();
        }


        private bool CheckConditions()
        {
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


        public void IQSamplesAvailable(Complex* samples, double samplerate, int length)
        {
            _iqSamplerate = samplerate;

            if (_radioFifoBuffer == null)
            {
                _radioFifoBuffer = new ComplexFifoStream(BlockMode.None);
            }

            if (_radioFifoBuffer.Length < samplerate)
            {
                _radioFifoBuffer.Write(samples, length);
            }
            else
            {
                _lostBuffers++;
            }
        }


        private void DemodulatorProcess()
        {
            _demodulatorInBuffer = UnsafeBuffer.Create(DemodulatorBufferLength, sizeof(Complex));
            _demodulatorInBufferPtr = (Complex*)_demodulatorInBuffer;

            _demodulatorOutBuffer = UnsafeBuffer.Create(DemodulatorBufferLength, sizeof(float));
            _demodulatorOutBufferPtr = (float*)_demodulatorOutBuffer;

            while (_processIsStarted)
            {
                if ((_radioFifoBuffer == null) || (_radioFifoBuffer.Length < DemodulatorBufferLength) || (_iqSamplerate == 0))
                {
                    Thread.Sleep(10);
                    continue;
                }

                Debug.WriteLine("demod " + Thread.CurrentThread.ManagedThreadId.ToString());

                _radioFifoBuffer.Read(_demodulatorInBufferPtr, DemodulatorBufferLength);

                var dataLength = _demodulator.ProcessBuffer(_demodulatorInBufferPtr, _iqSamplerate, DemodulatorBufferLength, _demodulatorOutBufferPtr);

                AutomaticFrequencyControl(_demodulatorOutBufferPtr, dataLength);

                if (_tetraFifoBuffer.Length + dataLength < OutDataLength * 8)
                {
                    _tetraFifoBuffer.Write(_demodulatorOutBufferPtr, dataLength);
                }
                else
                {
                    _lostBuffers++;
                }
            }

            _demodulatorInBuffer.Dispose();
            _demodulatorInBuffer = null;
            _demodulatorInBufferPtr = null;

            _demodulatorOutBuffer.Dispose();
            _demodulatorOutBuffer = null;
            _demodulatorOutBufferPtr = null;
        }

        private void AutomaticFrequencyControl(float* buffer, int length)
        {

            for (int i = 0; i < length; i++)
            {
                if (Math.Abs(buffer[i] - _prevAngle) < (PiDivFor))
                {
                    _afcCounter++;
                    _averageAngle += buffer[i];
                }
                else
                {
                    _afcCounter = 0;
                    _averageAngle = 0;
                }

                _prevAngle = buffer[i];

                if (_afcCounter > 30)
                {
                    _averageAngle /= _afcCounter;
                    _freqError = (_averageAngle - PiDivFor) / (TwoPi / 18000.0f);
                    _averageAngle = 0;
                    _afcCounter = 0;
                    break;
                }
            }
        }

        #endregion

        #region Tetra-decoder

        private void DecoderStart()
        {
            _outputIsStarted = true;
            _outputThread = new Thread(DecodingThread)
            {
                Priority = ThreadPriority.Normal,
                Name = "TetraDecocerThread"
            };
            _outputThread.Start();

            _audioProcessor.Enabled = true;
            _needDisplayBufferUpdate = true;
        }

        private void DecoderStop()
        {
            _audioProcessor.Enabled = false;

            _outputIsStarted = false;

            if (_outputThread != null)
            {
                _outputThread.Join();
                _outputThread = null;
            }
            _controlInterface.AudioIsMuted = false;
        }

        private void DecodingThread()
        {
            _fpass = 1;
            NativeMethods.tetra_decode_init();

            UdpClient server = new UdpClient("127.0.0.1", _tetraSettings.UdpPort);

            var audioSamplerate = 0d;

            while (_outputIsStarted)
            {

                Debug.WriteLine("decoder " + Thread.CurrentThread.ManagedThreadId.ToString());

                if (_tetraFifoBuffer.Length < _toDecodeBuffer.Length)
                {
                    Thread.Sleep(10);
                    continue;
                }

                _tetraFifoBuffer.Read(_toDecodeBufferPtr, _toDecodeBuffer.Length);

                if (_tetraSettings.UdpEnabled)
                {
                    server.SendAsync(BufferToBytes(_toDecodeBufferPtr, _toDecodeBuffer.Length), _toDecodeBuffer.Length * 2);
                }

                var audioChannel = _decoder.Process(_toDecodeBufferPtr, _toDecodeBuffer.Length, _outputBufferPtr);

                if (_needDisplayBufferUpdate)
                {
                    _needDisplayBufferUpdate = false;

                    Utils.Memcpy(_displayBufferPtr, _toDecodeBufferPtr, _displayBuffer.Length * sizeof(float));

                    _dispayBufferReady = true;
                }


                if (audioSamplerate != _audioSamplerate)
                {
                    audioSamplerate = _audioSamplerate;
                    _audioResampler = new Resampler(8000, audioSamplerate);

                    _resampledAudio = UnsafeBuffer.Create((int)audioSamplerate, sizeof(float));
                    _resampledAudioPtr = (float*)_resampledAudio;
                }

                if (audioChannel == 0 || audioSamplerate == 0) continue;

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

                DecodeAudio(_outputBufferPtr, _outAudioBufferPtr);

                //resample buffer
                var audioLength = _audioResampler.Process(_outAudioBufferPtr, _resampledAudioPtr, _outAudioBuffer.Length);
                //Clone to stereo
                audioLength = MonoToStereo(_resampledAudioPtr, audioLength);
                // Copy to output fifo
                _audioStreamChannel.Write(_resampledAudioPtr, audioLength);
            }
        }

        private byte[] BufferToBytes(float* buffer, int length)
        {
            var bitsBuffer = new byte[length * 2];

            float delta;

            int indexout = 0;

            for (var i = 0; i < length; i++)
            {
                delta = buffer[i];

                if (delta > PiDivTwo)
                {
                    bitsBuffer[indexout++] = 0;
                    bitsBuffer[indexout++] = 1;
                }
                else if (delta > 0)
                {
                    bitsBuffer[indexout++] = 0;
                    bitsBuffer[indexout++] = 0;
                }
                else if (delta < -PiDivTwo)
                {
                    bitsBuffer[indexout++] = 1;
                    bitsBuffer[indexout++] = 1;
                }
                else
                {
                    bitsBuffer[indexout++] = 1;
                    bitsBuffer[indexout++] = 0;
                }
            }

            return bitsBuffer;
        }


        private int DecodeAudio(byte* buf, float* audioBuffer)
        {
            int i;
            short[] payload = new short[690];

            for (i = 0; i < 6; i++)
                payload[115 * i] = (short)(0x6b21 + i);

            for (i = 0; i < 114; i++)
                payload[1 + i] = (short)(buf[i] != 0 ? -127 : 127);

            for (i = 0; i < 114; i++)
                payload[116 + i] = (short)(buf[114 + i] != 0 ? -127 : 127);

            for (i = 0; i < 114; i++)
                payload[231 + i] = (short)(buf[228 + i] != 0 ? -127 : 127);

            for (i = 0; i < 90; i++)
                payload[346 + i] = (short)(buf[342 + i] != 0 ? -127 : 127);

            short[] cdc = new short[276];
            short[] sdc = new short[480];
            fixed (short* cdcPtr = cdc, sdcPtr = sdc, payloadPtr = payload)
            {
                NativeMethods.tetra_cdec(_fpass, payload, cdc);
                _fpass = 0;

                NativeMethods.tetra_sdec(cdc, sdc);

                ShortToFloatPtr(sdcPtr, audioBuffer, sdc.Length);
            }

            return sdc.Length;
        }

        private void ShortToFloatPtr(short* source, float* dest, int length)
        {
            var gain = 1.0f / Int16.MaxValue / 100000.0f;

            for (int i = 0; i < length; i++)
            {
                dest[i] = source[i] * gain;
            }
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

            var haveAudio = false;

            if (_audioStreamChannel.Length >= length)
            {
                haveAudio = true;
                _audioStreamChannel.Read(samples, length);
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    samples[i] = 0;
                }
            }

            if (haveAudio)
            {
                //_oneChannelIsActive = true;
                BeginInvoke(new Action(() => _controlInterface.AudioIsMuted = false));
            }
            else
            {
                //_oneChannelIsActive = false;
                BeginInvoke(new Action(() => _controlInterface.AudioIsMuted = true));
            }
        }

        #endregion

        #region FM modulator //Not used
        /*
        private void ModulatorFM(float* audio, Complex* iq_output, int length)
        {
            var freq = 0.0f;
            var centerFreq = 0.0f;
            var deviation = 1.0f;

            for (var i = 0; i < length; i++)
            {
                freq = centerFreq + deviation * audio[i];

                _phase += freq;
                _phase %= TwoPi;

                iq_output[i] = Trig.SinCos(_phase);
            }
        }
        */
        #endregion

        #region Received Data Extractor

        void _decoder_DataReady(List<Dictionary<GlobalNames, int>> data)
        {

            Debug.WriteLine("data delegate " + Thread.CurrentThread.ManagedThreadId.ToString());

            if (_resetCounter > 0)
            {
                return;
            }

            while (data.Count > 0)
            {
                if (data[0].Count > 0)
                {
                    if (_rawData.Count < 100)
                        _rawData.Add(data[0].ToDictionary(entry => entry.Key, entry => entry.Value));
                }

                data.RemoveAt(0);
            }
        }

        public void UpdateCallsInfo(Dictionary<GlobalNames, int> data)
        {

            if (!data.ContainsKey(GlobalNames.CMCE_Primitives_Type)) return;

            var callId = 0;

            if (!data.TryGetValue(GlobalNames.Call_identifier, out callId))
                return;

            var call = new CallsEntry();
            var ssi = 0;
            var isGroup = false;
            var value = 0;

            var logEvent = false;

            if (!_networkBase.ContainsKey(_currentCell_NMI))
            {
                var entry = new NetworkEntry();
                entry.KnowGroups = new Dictionary<int, GroupsEntry>();
                _networkBase.Add(_currentCell_NMI, entry);

                var newGroup = new GroupsEntry();
                newGroup.Name = "Individual";
                newGroup.Priority = 0;
                _networkBase[_currentCell_NMI].KnowGroups.Add(0, newGroup);

                _needGroupsUpdate = true;
            }

            data.TryGetValue(GlobalNames.SSI, out ssi);

            if (data.TryGetValue(GlobalNames.Basic_service_Communication_type, out value))
            {
                if (value == (int)CommunicationType.Group_call)
                {
                    if (!_networkBase[_currentCell_NMI].KnowGroups.ContainsKey(ssi))
                    {
                        var newGroup = new GroupsEntry();
                        newGroup.Name = string.Empty;
                        newGroup.Priority = 0;
                        _networkBase[_currentCell_NMI].KnowGroups.Add(ssi, newGroup);
                        _needGroupsUpdate = true;
                    }
                }
            }

            isGroup = _networkBase[_currentCell_NMI].KnowGroups.ContainsKey(ssi);

            if (!_currentCalls.ContainsKey(callId))
            {
                var entry = new CallsEntry();
                entry.SSI = ssi;
                entry.CallID = callId;
                entry.Users = new List<int>();
                entry.TXer = 0;
                entry.Group = 0;
                entry.IsClear = 0;
                entry.Duplex = 0;

                _currentCalls.Add(callId, entry);

                logEvent = true;
            }

            if (data.TryGetValue(GlobalNames.Carrier_number, out value))
            {
                _currentCalls[callId].Carrier = value;
            }

            _currentCalls[callId].WatchDog = 0;

            if (isGroup)
            {
                _currentCalls[callId].Group = ssi;
                _currentCalls[callId].CallID = callId;

                if (_currentCalls[callId].Users.Contains(ssi))
                {
                    _currentCalls[callId].Users.Remove(ssi);
                }
            }
            else
            {
                if (!_currentCalls[callId].Users.Contains(ssi))
                {
                    _currentCalls[callId].Users.Add(ssi);
                }
            }

            var transmGrant = 0;
            var timeslot = data[GlobalNames.CurrTimeSlot];
            var assignedSlot = -1;
            var txerNew = -1;

            switch ((CmcePrimitivesType)data[GlobalNames.CMCE_Primitives_Type])
            {
                case CmcePrimitivesType.D_Disconnect:
                case CmcePrimitivesType.D_Release:
                case CmcePrimitivesType.D_TX_Ceased:
                    txerNew = 0;
                    assignedSlot = 0;
                    break;

                case CmcePrimitivesType.D_Info:
                    if (data.TryGetValue(GlobalNames.Slot_granting_element, out value))
                    {
                        if (data.TryGetValue(GlobalNames.SSI, out value))
                        {
                            txerNew = value;
                        }

                        if (data.TryGetValue(GlobalNames.Timeslot_assigned, out value))
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

                    if (data.TryGetValue(GlobalNames.Calling_party_address_SSI, out value))
                    {
                        if (!_currentCalls[callId].Users.Contains(value))
                        {
                            _currentCalls[callId].Users.Add(value);
                        }
                    }

                    if (data.TryGetValue(GlobalNames.Transmitting_party_address_SSI, out value))
                    {
                        if (!_currentCalls[callId].Users.Contains(value))
                        {
                            _currentCalls[callId].Users.Add(value);
                        }
                    }

                    var pduEncrypted = false;
                    var baseEncrypted = false;
                    var encrypt = false;

                    if (data.TryGetValue(GlobalNames.Encryption_mode, out value))
                    {
                        pduEncrypted = value != 0;
                    }

                    if (data.TryGetValue(GlobalNames.Encryption_control, out value))
                    {
                        encrypt = value != 0;
                    }

                    if (data.TryGetValue(GlobalNames.Basic_service_Encryption_flag, out value))
                    {
                        baseEncrypted = value != 0;
                    }

                    _currentCalls[callId].IsClear = (!pduEncrypted && !baseEncrypted && !encrypt) ? 1 : 0;

                    if (data.TryGetValue(GlobalNames.Simplex_duplex, out value))
                    {
                        _currentCalls[callId].Duplex = value;
                    }

                    if (data.TryGetValue(GlobalNames.Timeslot_assigned, out value))
                    {
                        assignedSlot = value;
                    }
                    else
                    {
                        assignedSlot = 0x10 >> timeslot;
                    }

                    if (data.TryGetValue(GlobalNames.Transmission_grant, out transmGrant))
                    {
                        switch ((TransmissionGranted)transmGrant)
                        {
                            case TransmissionGranted.Granted:
                                if (data.TryGetValue(GlobalNames.SSI, out value))
                                {
                                    txerNew = value;
                                }
                                break;

                            case TransmissionGranted.Granted_to_another_user:
                                if (data.TryGetValue(GlobalNames.Calling_party_address_SSI, out value))
                                {
                                    txerNew = value;
                                }
                                else if (data.TryGetValue(GlobalNames.Transmitting_party_address_SSI, out value))
                                {
                                    txerNew = value;
                                }
                                break;

                            default:
                                txerNew = 0;
                                break;
                        }
                    }

                    break;
            }

            if (assignedSlot != -1)
            {
                _currentCalls[callId].AssignedSlot = assignedSlot;
            }

            if (txerNew != -1)
            {
                if (_currentCalls[callId].TXer != txerNew)
                {
                    _currentCalls[callId].TXer = txerNew;
                    logEvent = true;
                }
            }

            if (logEvent)
            {
                Log_Tick(_currentCalls[callId]);
            }
        }

        void _decoder_SyncInfoReady(Dictionary<GlobalNames, int> syncInfo)
        {
            if (_resetCounter > 0)
            {
                return;
            }

            Debug.WriteLine("sync delegate " + Thread.CurrentThread.ManagedThreadId.ToString());

            if (_syncData.Count < 100)
                _syncData.Add(syncInfo.ToDictionary(entry => entry.Key, entry => entry.Value));
        }

        private void UpdateSysInfo(Dictionary<GlobalNames, int> data)
        {
            var value = 0;

            Debug.WriteLine("sync delegate " + Thread.CurrentThread.ManagedThreadId.ToString());

            if (data.TryGetValue(GlobalNames.MAC_PDU_Type, out value))
            {
                if ((MAC_PDU_Type)value == MAC_PDU_Type.Broadcast)
                {
                    _sysInfo = data.ToDictionary(entry => entry.Key, entry => entry.Value);

                    if (_sysInfo.TryGetValue(GlobalNames.Location_Area, out value))
                    {
                        _currentCell_LA = value;
                    }

                    var band = 0;
                    var offset = 0;
                    var carrier = 0;

                    var isFull = data.TryGetValue(GlobalNames.Frequency_Band, out band)
                        && data.TryGetValue(GlobalNames.Main_Carrier, out carrier)
                        && data.TryGetValue(GlobalNames.Offset, out offset);

                    _mainCell_Frequency = GlobalFunction.FrequencyCalc(isFull, carrier, band, offset);
                    _mainCell_Carrier = carrier;

                    _currentCell_Carrier = GlobalFunction.CarrierCalc(_controlInterface.Frequency);

                }
            }
        }


        #endregion

        #region GUI events

        private void MarkerTimer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("gui " + Thread.CurrentThread.ManagedThreadId.ToString());

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

            label1.Text = (_currentCellLoad[0].IsClear ? "c " : "e ") + _currentCellLoad[0].ISSI.ToString();
            ch1RadioButton.ForeColor = _ch1IsActive ? Color.Red : Color.Gray;

            label2.Text = (_currentCellLoad[1].IsClear ? "c " : "e ") + _currentCellLoad[1].ISSI.ToString();
            ch2RadioButton.ForeColor = _ch2IsActive ? Color.Red : Color.Gray;

            label3.Text = (_currentCellLoad[2].IsClear ? "c " : "e ") + _currentCellLoad[2].ISSI.ToString();
            ch3RadioButton.ForeColor = _ch3IsActive ? Color.Red : Color.Gray;

            label4.Text = (_currentCellLoad[3].IsClear ? "c " : "e ") + _currentCellLoad[3].ISSI.ToString();
            ch4RadioButton.ForeColor = _ch4IsActive ? Color.Red : Color.Gray;

            label9.Text = _currentCellLoad[0].GSSI.ToString();
            label8.Text = _currentCellLoad[1].GSSI.ToString();
            label7.Text = _currentCellLoad[2].GSSI.ToString();
            label6.Text = _currentCellLoad[3].GSSI.ToString();

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

                if (_currentCellLoad[0].GroupPriority >= maxPriority && _ch1IsActive)
                {
                    if (_currentCellLoad[3].IsClear || !GlobalFunction.IgnoreEncryptedSpeech)
                    {
                        maxPriority = _currentCellLoad[0].GroupPriority;
                        priorityChannel = 1;
                    }
                }

                if (_currentCellLoad[1].GroupPriority >= maxPriority && _ch2IsActive)
                {
                    if (_currentCellLoad[1].IsClear || !GlobalFunction.IgnoreEncryptedSpeech)
                    {
                        maxPriority = _currentCellLoad[1].GroupPriority;
                        priorityChannel = 2;
                    }
                }

                if (_currentCellLoad[2].GroupPriority >= maxPriority && _ch3IsActive)
                {
                    if (_currentCellLoad[2].IsClear || !GlobalFunction.IgnoreEncryptedSpeech)
                    {
                        maxPriority = _currentCellLoad[2].GroupPriority;
                        priorityChannel = 3;
                    }
                }

                if (_currentCellLoad[3].GroupPriority >= maxPriority && _ch4IsActive)
                {
                    if (_currentCellLoad[3].IsClear || !GlobalFunction.IgnoreEncryptedSpeech)
                    {
                        maxPriority = _currentCellLoad[3].GroupPriority;
                        priorityChannel = 4;
                    }
                }

                if (priorityChannel != 0)
                {
                    if (priorityChannel == 1 && !_channel1Listen) ch1RadioButton.Checked = true;
                    if (priorityChannel == 2 && !_channel2Listen) ch2RadioButton.Checked = true;
                    if (priorityChannel == 3 && !_channel3Listen) ch3RadioButton.Checked = true;
                    if (priorityChannel == 4 && !_channel4Listen) ch4RadioButton.Checked = true;
                }
                else
                {
                    if (_channel1Listen) ch1RadioButton.Checked = false;
                    if (_channel2Listen) ch2RadioButton.Checked = false;
                    if (_channel3Listen) ch3RadioButton.Checked = false;
                    if (_channel4Listen) ch4RadioButton.Checked = false;
                }
            }

            if (_decoder != null)
            {
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
                    var newEntry = new GroupsEntry();
                    newEntry.Name = entry.Name;
                    newEntry.Priority = entry.Priority;

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
                        var newEntry = new GroupDisplay();
                        newEntry.GSSI = entry.Key;
                        newEntry.Name = entry.Value.Name;
                        newEntry.Priority = entry.Value.Priority;

                        groupsList.Add(newEntry);
                    }

                    _infoWindow.UpdateGroupGrid(groupsList);
                }
            }
        }

        private void ch1RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _channel1Listen = ch1RadioButton.Checked;
        }

        private void ch2RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _channel2Listen = ch2RadioButton.Checked;
        }

        private void ch3RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _channel3Listen = ch3RadioButton.Checked;
        }

        private void ch4RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _channel4Listen = ch4RadioButton.Checked;
        }

        private void autoCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _autoSelectChannel = autoCheckBox.Checked;
            if (_autoSelectChannel)
            {
                ch1RadioButton.Checked = true;
            }
        }

        private void netInfoButton_Click(object sender, EventArgs e)
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

        private void configButton_Click(object sender, EventArgs e)
        {
            var configureDialog = new DialogConfigure(_tetraSettings, this);
            configureDialog.ShowDialog();

            UpdateGlobals();
        }

        private void UpdateGlobals()
        {
            GlobalFunction.IgnoreEncryptedData = _tetraSettings.IgnoreEncodedData;
            GlobalFunction.IgnoreEncryptedSpeech = _tetraSettings.IgnoreEncodedSpeech;
        }
        private void blockNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _tetraSettings.BlockedLevel = (int)blockNumericUpDown.Value;
        }

        private void mainFrequencyLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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
            var groupString = call != null ? call.Group.ToString() : "unknown";
            var txString = call != null ? call.TXer.ToString() : "unknown";
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
                else if (CompareString(entryString, "group", index))
                {
                    index += "group".Length;
                    entry += groupString;
                    entry += _tetraSettings.LogSeparator;
                }
                else if (CompareString(entryString, "tx", index))
                {
                    index += "tx".Length;
                    entry += txString;
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
                else if (CompareString(nameString, "group", index))
                {
                    index += "group".Length;
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

        private void afcTimer_Tick(object sender, EventArgs e)
        {
            ferLabel.Text = string.Format("FER {0:F0}Hz", _freqError);
            if (_tetraSettings.AfcEnabled)
            {
                if (_freqError > 200 || _freqError < -200)
                {
                    _isAfcWork = true;
                    _controlInterface.Frequency += (long)_freqError;
                    _freqError = 0;
                }
            }
        }

        private void dataExtractorTimer_Tick(object sender, EventArgs e)
        {
            if (_resetCounter > 0)
            {
                _resetCounter--;
                return;
            }

            while (_syncData.Count > 0)
            {
                _syncInfo = _syncData[0];

                _syncInfo.TryGetValue(GlobalNames.MCC, out _currentCell_MCC);
                _syncInfo.TryGetValue(GlobalNames.MNC, out _currentCell_MNC);
                _syncInfo.TryGetValue(GlobalNames.ColorCode, out _currentCell_CC);
                _currentCell_NMI = (_currentCell_MCC << 14) | _currentCell_MNC;
                
                _syncData.RemoveAt(0);
            }

            while (_rawData.Count > 0)
            {
                if (_rawData[0].Count > 0)
                {
                    UpdateSysInfo(_rawData[0]);

                    UpdateCmceInfo(_rawData[0]);
                    
                    if (_currentCell_NMI != 0) UpdateCallsInfo(_rawData[0]);
                }

                _rawData.RemoveAt(0);
            }

            if (_currentCell_NMI == 0) return;

            var index = 0;

            if (_lastDateTime != DateTime.Now.Second)
            {
                _lastDateTime = DateTime.Now.Second;

                while (index < _currentCalls.Count)
                {
                    if (_currentCalls.ElementAt(index).Value.WatchDog < 30)
                    {
                        _currentCalls.ElementAt(index).Value.WatchDog++;
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

                if (_networkBase.ContainsKey(_currentCell_NMI))
                {
                    if (_networkBase[_currentCell_NMI].KnowGroups.ContainsKey(entry.Group))
                    {
                        priority = _networkBase[_currentCell_NMI].KnowGroups[entry.Group].Priority;
                    }
                }

                if (entry.TXer != 0)
                {
                    if ((assignedSlot & 0x8) != 0)
                    {
                        _currentCellLoad[0].ISSI = entry.TXer;
                        _currentCellLoad[0].GSSI = entry.Group;
                        _currentCellLoad[0].GroupPriority = priority;
                        _currentCellLoad[0].IsClear = entry.IsClear == 1;
                    }
                    if ((assignedSlot & 0x4) != 0)
                    {
                        _currentCellLoad[1].ISSI = entry.TXer;
                        _currentCellLoad[1].GSSI = entry.Group;
                        _currentCellLoad[1].GroupPriority = priority;
                        _currentCellLoad[1].IsClear = entry.IsClear == 1;
                    }
                    if ((assignedSlot & 0x2) != 0)
                    {
                        _currentCellLoad[2].ISSI = entry.TXer;
                        _currentCellLoad[2].GSSI = entry.Group;
                        _currentCellLoad[2].GroupPriority = priority;
                        _currentCellLoad[2].IsClear = entry.IsClear == 1;
                    }
                    if ((assignedSlot & 0x1) != 0)
                    {
                        _currentCellLoad[3].ISSI = entry.TXer;
                        _currentCellLoad[3].GSSI = entry.Group;
                        _currentCellLoad[3].GroupPriority = priority;
                        _currentCellLoad[3].IsClear = entry.IsClear == 1;
                    }
                }
            }
        }

        private void UpdateCmceInfo(Dictionary<GlobalNames, int> data)
        {
            if (!data.ContainsKey(GlobalNames.CMCE_Primitives_Type)) return;

            if (data.Count == 0)
                return;

            if (_cmceData.Count > 100)
                _cmceData.RemoveAt(0);

            _cmceData.Add(data.ToDictionary(entry => entry.Key, entry => entry.Value));
        }
    }

    public class TetraSettings
    {
        public bool DontWritePause { get; set; }
        public int ContinueRecordTime { get; set; }
        public bool NewFileTimeEnable { get; set; }
        public int NewFileTime { get; set; }
        public int SampleFormatSelectedIndex { get; set; }
        public int SamplerateOut { get; set; }
        public string OutputSamplerateArray { get; set; }
        public string FileNameRules { get; set; }
        public string WriteFolder { get; set; }

        public string LogFileNameRules { get; set; }
        public string LogWriteFolder { get; set; }
        public string LogEntryRules { get; set; }
        public string LogSeparator { get; set; }

        public bool LogEnabled { get; set; }

        public int BlockedLevel { get; set; }

        public bool TopMostInfo { get; set; }

        public List<GroupsEntries> NetworkBase { get; set; }

        public bool AutoPlay { get; set; }

        public bool IgnoreEncodedData { get; set; }

        public bool IgnoreEncodedSpeech { get; set; }

        public bool ShowGroupName { get; set; }

        public bool UdpEnabled { get; set; }

        public int UdpPort { get; set; }

        public bool AfcEnabled { get; set; }
    }

    public class GroupsEntries
    {
        public int NMI { get; set; }
        public int GSSI { get; set; }
        public string Name { get; set; }
        public int Priopity { get; set; }
        public bool Blocked { get; set; }
    }

    public class CurrentLoad
    {
        public int GSSI { get; set; }
        public int ISSI { get; set; }
        public string GroupName { get; set; }
        public int GroupPriority { get; set; }
        public bool IsClear { get; set; }
    }
}