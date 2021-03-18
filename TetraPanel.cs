// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.TetraPanel
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

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
using Telerik.WinControls.UI;

namespace SDRSharp.Tetra
{
    public class TetraPanel : UserControl
    {
        private const float TwoPi = 6.283185f;
        private const float Pi = 3.141593f;
        private const float PiDivTwo = 1.570796f;
        private const float PiDivFor = 0.7853982f;
        private const int BurstLengthBits = 510;
        private const int BurstLengthSymbols = 255;
        private const int ChannelActiveDelay = 10;
        private ISharpControl _controlInterface;
        private IFProcessor _ifProcessor;
        private Demodulator _demodulator;
        private TetraDecoder _decoder;
        private UnsafeBuffer _iqBuffer;
        private unsafe Complex* _iqBufferPtr;
        private UnsafeBuffer _symbolsBuffer;
        private unsafe float* _symbolsBufferPtr;
        private ComplexFifoStream _radioFifoBuffer;
        private UnsafeBuffer _displayBuffer;
        private unsafe float* _displayBufferPtr;
        private UnsafeBuffer _outAudioBuffer;
        private unsafe float* _outAudioBufferPtr;
        private UnsafeBuffer _resampledAudio;
        private unsafe float* _resampledAudioPtr;
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
        private IContainer components;
        private Panel panel1;
        private System.Windows.Forms.Timer markerTimer;
        private CheckBox enabledCheckBox;
        private Display display;
        private GroupBox groupBox2;
        private GroupBox displayGroupBox;
        private GroupBox groupBox1;
        private Label connectLabel;
        private System.Windows.Forms.Timer timerGui;
        private RadioButton ch4RadioButton;
        private RadioButton ch3RadioButton;
        private RadioButton ch2RadioButton;
        private RadioButton ch1RadioButton;
        private Button netInfoButton;
        private CheckBox autoCheckBox;
        private Label colorLabel;
        private Label mncLabel;
        private Label mccLabel;
        private Label laLabel;
        private Label freqLabel;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label issiLabel;
        private Button configButton;
        private Label gssiLabel;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label currentFreqLabel;
        private NumericUpDown blockNumericUpDown;
        private Label label12;
        private Label currentCarrierLabel;
        private Label mainCarrierLabel;
        private Label currentFrequencyLabel;
        private LinkLabel mainFrequencyLinkLabel;
        private Label merLabel;
        private Label berLabel;
        private Label ferLabel;
        private System.Windows.Forms.Timer afcTimer;
        private System.Windows.Forms.Timer dataExtractorTimer;
        private Label label11;

        public unsafe TetraPanel(ISharpControl control)
        {
            try
            {
                this.InitializeComponent();
                this.InitArrays();
                this._settingsPersister = new SettingsPersister("tetraSettings.xml");
                this._tetraSettings = this._settingsPersister.ReadStored();
                if (string.IsNullOrEmpty(this._tetraSettings.LogEntryRules))
                    this._tetraSettings.LogEntryRules = "date + time + mcc + mnc + la + cc + carrier + slot + callid + type + from + to + encryption + duplex";
                if (string.IsNullOrEmpty(this._tetraSettings.LogFileNameRules))
                    this._tetraSettings.LogFileNameRules = "date \\ frequency \\ mcc \"_\" mnc \"_\" la";
                if (string.IsNullOrEmpty(this._tetraSettings.LogSeparator))
                    this._tetraSettings.LogSeparator = " ; ";
                if (this._tetraSettings.NetworkBase == null)
                    this._tetraSettings.NetworkBase = new List<GroupsEntries>();
                if (this._tetraSettings.UdpPort == 0)
                    this._tetraSettings.UdpPort = 20025;
                this.UpdateGlobals();
                this.blockNumericUpDown.Value = (Decimal)this._tetraSettings.BlockedLevel;
                this._networkBase = this.NetworkBaseDeserializer(this._tetraSettings.NetworkBase);
                this._needGroupsUpdate = true;
                this._controlInterface = control;
                this._infoWindow = new NetInfoWindow();
                this._infoWindow.FormClosing += new FormClosingEventHandler(this._infoWindow_FormClosing);
                this._ifProcessor = new IFProcessor();
                this._controlInterface.RegisterStreamHook((object)this._ifProcessor, ProcessorType.DecimatedAndFilteredIQ);
                this._ifProcessor.IQReady += new IFProcessor.IQReadyDelegate(this.IQSamplesAvailable);
                this._audioProcessor = new AudioProcessor();
                this._controlInterface.RegisterStreamHook((object)this._audioProcessor, ProcessorType.DemodulatorOutput);
                this._audioProcessor.AudioReady += new AudioProcessor.AudioReadyDelegate(this.AudioSamplesNedeed);
                this._decoder = new TetraDecoder((Control)this);
                this._decoder.DataReady += new TetraDecoder.DataReadyDelegate(this._decoder_DataReady);
                this._decoder.SyncInfoReady += new TetraDecoder.SyncInfoReadyDelegate(this._decoder_SyncInfoReady);
                this._demodulator = new Demodulator();
                this._controlInterface.PropertyChanged += new PropertyChangedEventHandler(this._controlInterface_PropertyChanged);
                this._displayBuffer = UnsafeBuffer.Create((int)byte.MaxValue, 4);
                this._displayBufferPtr = (float*)(void*)this._displayBuffer;
                this._outAudioBuffer = UnsafeBuffer.Create(480, 4);
                this._outAudioBufferPtr = (float*)(void*)this._outAudioBuffer;
                this._audioStreamChannel = new FloatFifoStream(BlockMode.None);
                this.autoCheckBox.Checked = this._tetraSettings.AutoPlay;
                this.AutoCheckBox_CheckedChanged((object)null, (EventArgs)null);
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show("Tetra plugin exception -" + ex.ToString());
            }
        }

        private void InitArrays()
        {
            for (int index = 0; index < 4; ++index)
                this._currentCellLoad[index] = new CurrentLoad();
        }

        private Dictionary<int, NetworkEntry> NetworkBaseDeserializer(
          List<GroupsEntries> list)
        {
            if (list.Count == 0)
                return new Dictionary<int, NetworkEntry>();
            Dictionary<int, NetworkEntry> dictionary = new Dictionary<int, NetworkEntry>();
            GroupsEntry groupsEntry1;
            foreach (GroupsEntries groupsEntries in list)
            {
                if (dictionary.ContainsKey(groupsEntries.NMI))
                {
                    groupsEntry1 = new GroupsEntry();
                    groupsEntry1.Name = groupsEntries.Name;
                    groupsEntry1.Priority = groupsEntries.Priopity;
                    GroupsEntry groupsEntry2 = groupsEntry1;
                    dictionary[groupsEntries.NMI].KnowGroups.Add(groupsEntries.GSSI, groupsEntry2);
                }
                else
                {
                    NetworkEntry networkEntry = new NetworkEntry()
                    {
                        KnowGroups = new Dictionary<int, GroupsEntry>()
                    };
                    groupsEntry1 = new GroupsEntry();
                    groupsEntry1.Name = groupsEntries.Name;
                    groupsEntry1.Priority = groupsEntries.Priopity;
                    GroupsEntry groupsEntry2 = groupsEntry1;
                    networkEntry.KnowGroups.Add(groupsEntries.GSSI, groupsEntry2);
                    dictionary.Add(groupsEntries.NMI, networkEntry);
                }
            }
            return dictionary;
        }

        private List<GroupsEntries> NetworkBaseSerializer(
          Dictionary<int, NetworkEntry> dict)
        {
            if (dict.Count == 0)
                return new List<GroupsEntries>();
            List<GroupsEntries> groupsEntriesList = new List<GroupsEntries>();
            foreach (KeyValuePair<int, NetworkEntry> keyValuePair in dict)
            {
                foreach (KeyValuePair<int, GroupsEntry> knowGroup in keyValuePair.Value.KnowGroups)
                {
                    GroupsEntries groupsEntries = new GroupsEntries()
                    {
                        NMI = keyValuePair.Key,
                        GSSI = knowGroup.Key,
                        Name = knowGroup.Value.Name,
                        Priopity = knowGroup.Value.Priority
                    };
                    groupsEntriesList.Add(groupsEntries);
                }
            }
            return groupsEntriesList;
        }

        private void _infoWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this._needCloseInfo = true;
        }

        public void SaveSettings()
        {
            if (this._processIsStarted)
                this.StopDecoding();
            this._tetraSettings.AutoPlay = this.autoCheckBox.Checked;
            this._tetraSettings.NetworkBase = this.NetworkBaseSerializer(this._networkBase);
            this._settingsPersister.PersistStored(this._tetraSettings);
        }

        private void _controlInterface_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            TetraPlugin.Logger("SDR# Property updated: " + propertyName);
            if (propertyName == "StartRadio" || propertyName == "StopRadio")
                return;
            if (!(propertyName == "Frequency"))
            {
                int num = propertyName == "DetectorType" ? 1 : 0;
            }
            else
            {
                if (!this._isAfcWork)
                    this.ResetDecoder();
                this._isAfcWork = false;
            }
        }

        private void ResetDecoder()
        {
            this._resetCounter = 10;
            this._freqError = 0.0;
            this._currentCell_NMI = 0;
            this._currentCell_MNC = 0;
            this._currentCell_MCC = 0;
            this._currentCell_LA = 0;
            this._currentCell_CC = 0;
            this._mainCell_Frequency = 0L;
            this._mainCell_Carrier = 0;
            this._activeCounter1 = 0;
            this._activeCounter2 = 0;
            this._activeCounter3 = 0;
            this._activeCounter4 = 0;
            this._ch1IsActive = false;
            this._ch2IsActive = false;
            this._ch3IsActive = false;
            this._ch4IsActive = false;
            this._currentChPriority = int.MinValue;
            this._needGroupsUpdate = true;
            this._currentCell_Carrier = (int)Math.Round((double)(this._controlInterface.Frequency % 100000000L) / 25000.0);
            this._sysInfo.Clear();
            this._currentCalls.Clear();
            this._cmceData.Clear();
            this._rawData.Clear();
            this._syncInfo.Clear();
            this._infoWindow.ResetInfo();
            this.InitArrays();
        }

        private void StartDecoding()
        {
            this._ifProcessor.Enabled = true;
            this._processIsStarted = true;
            this.DecoderStart();
        }

        private void StopDecoding()
        {
            this._ifProcessor.Enabled = false;
            this._processIsStarted = false;
            this.DecoderStop();
        }

        /**
         * Verify if SDR# it's ready to decode tetra signals.
         */
        private bool CheckConditions()
        {
            this._ifProcessor.SampleRate = 25000;
            this._controlInterface.Frequency = 390087500;
            this._controlInterface.StartRadio();
            this._controlInterface.DetectorType = DetectorType.WFM;
            this._controlInterface.FrequencyShift = 28000;

            if (this._ifProcessor.SampleRate >= 25000.0)
                return true;
            if (CultureInfo.CurrentCulture.Name == "ru-RU")
            {
                int num1 = (int)MessageBox.Show("Слишком низкая частота дискретизации IF. Измените вид модуляции на WFM или установите параметр minOutputSampleRate value = 32000", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                int num2 = (int)MessageBox.Show("IF samplerate too low.  Change the modulation type to WFM or set the parameter minOutputSampleRate value = 32000", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return false;
        }

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

        private unsafe void AutomaticFrequencyControl(float* buffer, int length)
        {
            bool flag = false;
            float num = 0.0f;
            for (int index = 0; index < length; ++index)
            {
                if ((double)Math.Abs(buffer[index] - this._prevAngle) < 0.785398185253143)
                {
                    ++this._afcCounter;
                    this._averageAngle += buffer[index];
                }
                else
                {
                    flag = this._afcCounter == 31 || this._afcCounter == 32;
                    num = this._averageAngle / (float)this._afcCounter;
                    this._afcCounter = 0;
                    this._averageAngle = 0.0f;
                }
                this._prevAngle = buffer[index];
                if (flag)
                {
                    this._freqError = this._freqError * 0.899999976158142 + 0.100000001490116 * (((double)num - 0.785398185253143) / 0.000349065870977938);
                    break;
                }
            }
        }

        private void DecoderStart()
        {
            this._decodingIsStarted = true;
            this._decodingThread = new Thread(new ThreadStart(this.DecodingThread))
            {
                Priority = ThreadPriority.Normal,
                Name = "TetraDecocerThread"
            };
            this._decodingThread.Start();
            this._audioProcessor.Enabled = true;
            this._needDisplayBufferUpdate = true;
        }

        private void DecoderStop()
        {
            this._audioProcessor.Enabled = false;
            this._decodingIsStarted = false;
            if (this._decodingThread != null)
            {
                this._decodingThread.Join();
                this._decodingThread = (Thread)null;
            }
            this._controlInterface.AudioIsMuted = false;
        }

        /**
         * Thread que espera a que se cargue el radiobuffer y luego lo procesa.
         * Buffer de 510 Bits
         */
        private unsafe void DecodingThread()
        {
            this._iqBuffer = UnsafeBuffer.Create(510, sizeof(Complex));
            this._iqBufferPtr = (Complex*)(void*)this._iqBuffer;
            this._symbolsBuffer = UnsafeBuffer.Create(256, 4);
            this._symbolsBufferPtr = (float*)(void*)this._symbolsBuffer;
            this._diBitsBuffer = UnsafeBuffer.Create(510, 1);
            this._diBitsBufferPtr = (byte*)(void*)this._diBitsBuffer;
            UdpClient udpClient = new UdpClient("127.0.0.1", this._tetraSettings.UdpPort);
            double outputSampleRate = 0.0;
            Burst burst = new Burst()
            {
                Ptr = this._diBitsBufferPtr
            };
            while (this._decodingIsStarted)
            {
                if (this._radioFifoBuffer == null || this._radioFifoBuffer.Length < 510 || this._iqSamplerate == 0.0)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    burst.Mode = this._tetraMode;
                    this._radioFifoBuffer.Read(this._iqBufferPtr, 510);
                    this._demodulator.ProcessBuffer(burst, this._iqBufferPtr, this._iqSamplerate, 510, this._symbolsBufferPtr);
                    if (burst.Type != BurstType.WaitBurst)
                    {
                        this.AutomaticFrequencyControl(this._symbolsBufferPtr, (int)byte.MaxValue);
                        //Send Bit to udp
                        if (this._tetraSettings.UdpEnabled)
                            udpClient.SendAsync(this.ConvertAngleToDiBits(this._symbolsBufferPtr, (int)byte.MaxValue), 510);

                        int num = this._decoder.Process(burst, this._outAudioBufferPtr);
                        this._tetraMode = this._decoder.TetraMode;
                        if (this._needDisplayBufferUpdate)
                        {
                            this._needDisplayBufferUpdate = false;
                            Utils.Memcpy((void*)this._displayBufferPtr, (void*)this._symbolsBufferPtr, this._displayBuffer.Length * 4);
                            this._dispayBufferReady = true;
                        }
                        if (num != 0 && this._audioSamplerate != 0.0)
                        {
                            if (outputSampleRate != this._audioSamplerate)
                            {
                                outputSampleRate = this._audioSamplerate;
                                this._audioResampler = new Resampler(8000.0, outputSampleRate);
                                this._resampledAudio = UnsafeBuffer.Create((int)outputSampleRate, 4);
                                this._resampledAudioPtr = (float*)(void*)this._resampledAudio;
                            }
                            switch (num)
                            {
                                case 1:
                                    this._ch1IsActive = true;
                                    this._activeCounter1 = 10;
                                    if (this._channel1Listen)
                                        break;
                                    continue;
                                case 2:
                                    this._ch2IsActive = true;
                                    this._activeCounter2 = 10;
                                    if (this._channel2Listen)
                                        break;
                                    continue;
                                case 3:
                                    this._ch3IsActive = true;
                                    this._activeCounter3 = 10;
                                    if (this._channel3Listen)
                                        break;
                                    continue;
                                case 4:
                                    this._ch4IsActive = true;
                                    this._activeCounter4 = 10;
                                    if (!this._channel4Listen)
                                        continue;
                                    break;
                            }
                            this._audioStreamChannel.Write(this._resampledAudioPtr, this._audioResampler.Process(this._outAudioBufferPtr, this._resampledAudioPtr, this._outAudioBuffer.Length));
                        }
                    }
                }
            }
            this._iqBuffer.Dispose();
            this._iqBuffer = (UnsafeBuffer)null;
            this._iqBufferPtr = (Complex*)null;
            this._symbolsBuffer.Dispose();
            this._symbolsBuffer = (UnsafeBuffer)null;
            this._symbolsBufferPtr = (float*)null;
            this._diBitsBuffer.Dispose();
            this._diBitsBuffer = (UnsafeBuffer)null;
            this._diBitsBufferPtr = (byte*)null;
        }

        private unsafe void ConvertAngleToDiBits(byte* bitsBuffer, float* angles, int sourceLength)
        {
            while (sourceLength-- > 0)
            {
                float num = *angles++;
                *bitsBuffer++ = (double)num < 0.0 ? (byte)1 : (byte)0;
                *bitsBuffer++ = (double)Math.Abs(num) > 1.57079637050629 ? (byte)1 : (byte)0;
            }
        }

        private unsafe byte[] ConvertAngleToDiBits(float* angles, int sourceLength)
        {
            byte[] numArray1 = new byte[sourceLength * 2];
            int num1 = 0;
            while (sourceLength-- > 0)
            {
                float num2 = *angles++;
                byte[] numArray2 = numArray1;
                int index1 = num1;
                int num3 = index1 + 1;
                int num4 = (double)num2 < 0.0 ? 1 : 0;
                numArray2[index1] = (byte)num4;
                byte[] numArray3 = numArray1;
                int index2 = num3;
                num1 = index2 + 1;
                int num5 = (double)Math.Abs(num2) > 1.57079637050629 ? 1 : 0;
                numArray3[index2] = (byte)num5;
            }
            return numArray1;
        }

        private unsafe int MonoToStereo(float* buffer, int monoLength)
        {
            int index1 = monoLength - 1;
            int num1 = monoLength * 2 - 1;
            for (int index2 = 0; index2 < monoLength; ++index2)
            {
                float* numPtr1 = buffer;
                int num2 = num1;
                int num3 = num2 - 1;
                int num4 = num2 * 4;
                *(float*)((IntPtr)numPtr1 + num4) = buffer[index1];
                float* numPtr2 = buffer;
                int num5 = num3;
                num1 = num5 - 1;
                int num6 = num5 * 4;
                *(float*)((IntPtr)numPtr2 + num6) = buffer[index1];
                --index1;
            }
            return monoLength * 2;
        }

        public unsafe void AudioSamplesNedeed(float* samples, double samplerate, int length)
        {
            this._audioSamplerate = samplerate;
            if (this._audioStreamChannel.Length >= length)
            {
                this._audioStreamChannel.Read(samples, length);
            }
            else
            {
                for (int index = 0; index < length; ++index)
                    samples[index] = 0.0f;
            }
        }

        private void _decoder_DataReady(List<ReceivedData> data)
        {
            if (this._resetCounter > 0)
                return;
            while (data.Count > 0)
            {
                if (this._rawData.Count < 100)
                    this._rawData.Add(data[0]);
                data.RemoveAt(0);
            }
        }

        public void UpdateCallsInfo(ReceivedData data)
        {
            if (!data.Contains(GlobalNames.CMCE_Primitives_Type))
                return;
            int key1 = 0;
            if (!data.TryGetValue(GlobalNames.Call_identifier, ref key1))
                return;
            int key2 = 0;
            int num1 = 0;
            int num2 = 0;
            bool flag1 = false;
            GroupsEntry groupsEntry1;
            if (!this._networkBase.ContainsKey(this._currentCell_NMI))
            {
                this._networkBase.Add(this._currentCell_NMI, new NetworkEntry()
                {
                    KnowGroups = new Dictionary<int, GroupsEntry>()
                });
                groupsEntry1 = new GroupsEntry();
                groupsEntry1.Name = "Individual";
                groupsEntry1.Priority = 0;
                this._networkBase[this._currentCell_NMI].KnowGroups.Add(0, groupsEntry1);
                this._needGroupsUpdate = true;
            }
            data.TryGetValue(GlobalNames.SSI, ref key2);
            if (data.TryGetValue(GlobalNames.Basic_service_Communication_type, ref num2))
            {
                num1 = num2;
                if (num2 == 1 && !this._networkBase[this._currentCell_NMI].KnowGroups.ContainsKey(key2))
                {
                    groupsEntry1 = new GroupsEntry();
                    groupsEntry1.Name = string.Empty;
                    groupsEntry1.Priority = 0;
                    GroupsEntry groupsEntry2 = groupsEntry1;
                    this._networkBase[this._currentCell_NMI].KnowGroups.Add(key2, groupsEntry2);
                    this._needGroupsUpdate = true;
                }
            }
            if (!this._currentCalls.ContainsKey(key1))
            {
                CallsEntry callsEntry = new CallsEntry()
                {
                    To = key2,
                    CallID = key1,
                    From = 0,
                    Type = num1,
                    IsClear = 0,
                    Duplex = 0
                };
                this._currentCalls.Add(key1, callsEntry);
                flag1 = true;
            }
            this._currentCalls[key1].To = key2;
            if (data.TryGetValue(GlobalNames.Basic_service_Communication_type, ref num2))
                this._currentCalls[key1].Type = num2;
            if (data.TryGetValue(GlobalNames.Carrier_number, ref num2))
                this._currentCalls[key1].Carrier = num2;
            this._currentCalls[key1].WatchDog = 5;
            int num3 = 0;
            int num4 = data.Value(GlobalNames.CurrTimeSlot);
            int num5 = -1;
            int num6 = -1;
            switch (data.Value(GlobalNames.CMCE_Primitives_Type))
            {
                case 2:
                case 7:
                case 11:
                    if (data.TryGetValue(GlobalNames.Calling_party_address_SSI, ref num2))
                        this._currentCalls[key1].From = num2;
                    if (data.TryGetValue(GlobalNames.Transmitting_party_address_SSI, ref num2))
                        this._currentCalls[key1].From = num2;
                    bool flag2 = false;
                    bool flag3 = false;
                    bool flag4 = false;
                    if (data.TryGetValue(GlobalNames.Encryption_mode, ref num2))
                        flag2 = (uint)num2 > 0U;
                    if (data.TryGetValue(GlobalNames.Encryption_control, ref num2))
                        flag4 = (uint)num2 > 0U;
                    if (data.TryGetValue(GlobalNames.Basic_service_Encryption_flag, ref num2))
                        flag3 = (uint)num2 > 0U;
                    this._currentCalls[key1].IsClear = flag2 || flag3 || flag4 ? 0 : 1;
                    if (data.TryGetValue(GlobalNames.Simplex_duplex, ref num2))
                        this._currentCalls[key1].Duplex = num2;
                    num5 = !data.TryGetValue(GlobalNames.Timeslot_assigned, ref num2) ? 16 >> num4 : num2;
                    if (data.TryGetValue(GlobalNames.Transmission_grant, ref num3))
                    {
                        switch ((TransmissionGranted)num3)
                        {
                            case TransmissionGranted.Granted:
                                if (data.TryGetValue(GlobalNames.SSI, ref num2))
                                {
                                    num6 = num2;
                                    break;
                                }
                                break;
                            case TransmissionGranted.Granted_to_another_user:
                                if (data.TryGetValue(GlobalNames.Calling_party_address_SSI, ref num2))
                                {
                                    num6 = num2;
                                    break;
                                }
                                if (data.TryGetValue(GlobalNames.Transmitting_party_address_SSI, ref num2))
                                {
                                    num6 = num2;
                                    break;
                                }
                                break;
                            default:
                                num6 = 0;
                                break;
                        }
                    }
                    else
                        break;
                    break;
                case 4:
                case 6:
                    num5 = 0;
                    num6 = 0;
                    break;
                case 5:
                    if (data.TryGetValue(GlobalNames.Slot_granting_element, ref num2))
                    {
                        if (data.TryGetValue(GlobalNames.SSI, ref num2))
                            num6 = num2;
                        num5 = !data.TryGetValue(GlobalNames.Timeslot_assigned, ref num2) ? 16 >> num4 : num2;
                        break;
                    }
                    break;
                case 9:
                    num6 = 0;
                    break;
            }
            if (num5 != -1)
                this._currentCalls[key1].AssignedSlot = num5;
            if (num6 != -1 && this._currentCalls[key1].From != num6)
            {
                this._currentCalls[key1].From = num6;
                flag1 = true;
            }
            if (!flag1)
                return;
            this.Log_Tick(this._currentCalls[key1]);
        }

        private void _decoder_SyncInfoReady(ReceivedData syncInfo)
        {
            if (this._resetCounter > 0)
                return;
            syncInfo.Data.CopyTo((Array)this._syncInfo.Data, 0);
        }

        private void UpdateSysInfo(ReceivedData data)
        {
            if (data.Value(GlobalNames.MAC_PDU_Type) != 2)
                return;
            data.Data.CopyTo((Array)this._sysInfo.Data, 0);
            this._sysInfo.TryGetValue(GlobalNames.Location_Area, ref this._currentCell_LA);
            int band = 0;
            int offset = 0;
            int carrier = 0;
            this._mainCell_Frequency = GlobalFunction.FrequencyCalc(data.TryGetValue(GlobalNames.Frequency_Band, ref band) && data.TryGetValue(GlobalNames.Main_Carrier, ref carrier) && data.TryGetValue(GlobalNames.Offset, ref offset), carrier, band, offset);
            this._mainCell_Carrier = carrier;
            this._currentCell_Carrier = GlobalFunction.CarrierCalc(this._controlInterface.Frequency);
        }

        private unsafe void MarkerTimer_Tick(object sender, EventArgs e)
        {
            if (this._displayBuffer != null && this._dispayBufferReady)
            {
                this._dispayBufferReady = false;
                this.display.Perform(this._displayBufferPtr, this._displayBuffer.Length);
                this.display.Refresh();
                this._needDisplayBufferUpdate = true;
            }
            if (!this._processIsStarted)
                return;
            Label label1 = this.label1;
            int from = this._currentCellLoad[0].From;
            string str1 = from.ToString();
            label1.Text = str1;
            this.ch1RadioButton.ForeColor = this._ch1IsActive ? Color.Red : Color.Gray;
            Label label2 = this.label2;
            from = this._currentCellLoad[1].From;
            string str2 = from.ToString();
            label2.Text = str2;
            this.ch2RadioButton.ForeColor = this._ch2IsActive ? Color.Red : Color.Gray;
            Label label3 = this.label3;
            from = this._currentCellLoad[2].From;
            string str3 = from.ToString();
            label3.Text = str3;
            this.ch3RadioButton.ForeColor = this._ch3IsActive ? Color.Red : Color.Gray;
            Label label4 = this.label4;
            from = this._currentCellLoad[3].From;
            string str4 = from.ToString();
            label4.Text = str4;
            this.ch4RadioButton.ForeColor = this._ch4IsActive ? Color.Red : Color.Gray;
            this.label9.Text = (this._currentCellLoad[0].Type == 1 ? "g " : "") + this._currentCellLoad[0].GroupName;
            this.label8.Text = (this._currentCellLoad[1].Type == 1 ? "g " : "") + this._currentCellLoad[1].GroupName;
            this.label7.Text = (this._currentCellLoad[2].Type == 1 ? "g " : "") + this._currentCellLoad[2].GroupName;
            this.label6.Text = (this._currentCellLoad[3].Type == 1 ? "g " : "") + this._currentCellLoad[3].GroupName;
            --this._activeCounter1;
            if (this._activeCounter1 < 0)
            {
                this._activeCounter1 = 0;
                this._ch1IsActive = false;
            }
            --this._activeCounter2;
            if (this._activeCounter2 < 0)
            {
                this._activeCounter2 = 0;
                this._ch2IsActive = false;
            }
            --this._activeCounter3;
            if (this._activeCounter3 < 0)
            {
                this._activeCounter3 = 0;
                this._ch3IsActive = false;
            }
            --this._activeCounter4;
            if (this._activeCounter4 < 0)
            {
                this._activeCounter4 = 0;
                this._ch4IsActive = false;
            }
            if (this._autoSelectChannel)
            {
                int groupPriority1 = (int)this.blockNumericUpDown.Value;
                int num = 0;
                if (this._currentCellLoad[0].From != 0 && this._currentCellLoad[0].GroupPriority >= groupPriority1 && this._currentCellLoad[0].GroupPriority > this._currentChPriority && (this._currentCellLoad[0].IsClear || !GlobalFunction.IgnoreEncryptedSpeech))
                {
                    groupPriority1 = this._currentCellLoad[0].GroupPriority;
                    num = 1;
                }
                if (this._currentCellLoad[1].From != 0 && this._currentCellLoad[1].GroupPriority >= groupPriority1 && this._currentCellLoad[1].GroupPriority > this._currentChPriority && (this._currentCellLoad[1].IsClear || !GlobalFunction.IgnoreEncryptedSpeech))
                {
                    groupPriority1 = this._currentCellLoad[1].GroupPriority;
                    num = 2;
                }
                if (this._currentCellLoad[2].From != 0 && this._currentCellLoad[2].GroupPriority >= groupPriority1 && this._currentCellLoad[2].GroupPriority > this._currentChPriority && (this._currentCellLoad[2].IsClear || !GlobalFunction.IgnoreEncryptedSpeech))
                {
                    groupPriority1 = this._currentCellLoad[2].GroupPriority;
                    num = 3;
                }
                if (this._currentCellLoad[3].From != 0 && this._currentCellLoad[3].GroupPriority >= groupPriority1 && this._currentCellLoad[3].GroupPriority > this._currentChPriority && (this._currentCellLoad[3].IsClear || !GlobalFunction.IgnoreEncryptedSpeech))
                {
                    int groupPriority2 = this._currentCellLoad[3].GroupPriority;
                    num = 4;
                }
                if (num != 0)
                {
                    if (num == 1 && !this._channel1Listen)
                    {
                        this.ch1RadioButton.Checked = true;
                        this._currentChPriority = this._currentCellLoad[0].GroupPriority;
                    }
                    else if (num == 2 && !this._channel2Listen)
                    {
                        this.ch2RadioButton.Checked = true;
                        this._currentChPriority = this._currentCellLoad[1].GroupPriority;
                    }
                    else if (num == 3 && !this._channel3Listen)
                    {
                        this.ch3RadioButton.Checked = true;
                        this._currentChPriority = this._currentCellLoad[2].GroupPriority;
                    }
                    else if (num == 4 && !this._channel4Listen)
                    {
                        this.ch4RadioButton.Checked = true;
                        this._currentChPriority = this._currentCellLoad[3].GroupPriority;
                    }
                }
                else
                {
                    if (this._channel1Listen && this._currentCellLoad[0].From == 0)
                    {
                        this.ch1RadioButton.Checked = false;
                        this._currentChPriority = int.MinValue;
                    }
                    if (this._channel2Listen && this._currentCellLoad[1].From == 0)
                    {
                        this.ch2RadioButton.Checked = false;
                        this._currentChPriority = int.MinValue;
                    }
                    if (this._channel3Listen && this._currentCellLoad[2].From == 0)
                    {
                        this.ch3RadioButton.Checked = false;
                        this._currentChPriority = int.MinValue;
                    }
                    if (this._channel4Listen && this._currentCellLoad[3].From == 0)
                    {
                        this.ch4RadioButton.Checked = false;
                        this._currentChPriority = int.MinValue;
                    }
                }
            }
            this._controlInterface.AudioIsMuted = !this._channel1Listen && !this._channel2Listen && !this._channel3Listen && !this._channel4Listen;
            if (this._decoder == null)
                return;
            this.colorLabel.Visible = this._tetraMode == Mode.TMO;
            this.laLabel.Visible = this._tetraMode == Mode.TMO;
            this.mccLabel.Text = "MCC:" + this._currentCell_MCC.ToString();
            this.mncLabel.Text = "MNC:" + this._currentCell_MNC.ToString();
            this.colorLabel.Text = "Color:" + this._currentCell_CC.ToString();
            this.connectLabel.Visible = this._decoder.BurstReceived;
            this.laLabel.Text = "LA:" + this._currentCell_LA.ToString();
            this.mainCarrierLabel.Text = this._mainCell_Carrier.ToString();
            this.mainFrequencyLinkLabel.Text = string.Format("{0:0,0.000###} MHz", (object)((Decimal)this._mainCell_Frequency * 0.000001M));
            this.currentCarrierLabel.Text = this._currentCell_Carrier.ToString();
            this.currentFrequencyLabel.Text = string.Format("{0:0,0.000###} MHz", (object)((Decimal)this._controlInterface.Frequency * 0.000001M));
        }

        private void EnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.enabledCheckBox.Checked && !this._processIsStarted)
            {
                if (!this.CheckConditions())
                {
                    this.enabledCheckBox.Checked = false;
                }
                else
                {
                    this.ResetDecoder();
                    this.StartDecoding();
                }
            }
            else
            {
                if (this.enabledCheckBox.Checked || !this._processIsStarted)
                    return;
                this.StopDecoding();
            }
        }

        /**
         * Update user panel information
         */
        private void TimerGui_Tick(object sender, EventArgs e)
        {
            this.label11.Text = this._lostBuffers.ToString();
            if (this._decoder != null)
            {
                this.berLabel.Text = string.Format("BER {0:0.0}%", (object)this._decoder.Ber);
                this.merLabel.Text = string.Format("MER {0:0.0}%", (object)this._decoder.Mer);
            }
            if (this._infoWindow == null || this._resetCounter > 0)
                return;
            if (this._showInfo)
            {
                this._infoWindow.UpdateTextBox(this._cmceData);
                this._infoWindow.UpdateCalls(this._currentCalls);
                this._infoWindow.UpdateSysInfo(this._syncInfo, this._sysInfo);
                this._infoWindow.UpdateNeighBour();
                if (this._needCloseInfo)
                {
                    this._tetraSettings.TopMostInfo = this._infoWindow.TopMost;
                    this._showInfo = false;
                    this._needCloseInfo = false;
                    this._infoWindow.Visible = false;
                }
            }
            if (this._infoWindow.GroupsChanged)
            {
                List<GroupDisplay> updatedGroups = this._infoWindow.GetUpdatedGroups();
                this._networkBase[this._currentCell_NMI].KnowGroups.Clear();
                foreach (GroupDisplay groupDisplay in updatedGroups)
                {
                    GroupsEntry groupsEntry = new GroupsEntry()
                    {
                        Name = groupDisplay.Name,
                        Priority = groupDisplay.Priority
                    };
                    this._networkBase[this._currentCell_NMI].KnowGroups.Add(groupDisplay.GSSI, groupsEntry);
                }
                this._infoWindow.GroupsChanged = false;
            }
            if (!this._needGroupsUpdate || this._currentCell_NMI == 0 || !this._networkBase.ContainsKey(this._currentCell_NMI))
                return;
            this._needGroupsUpdate = false;
            List<GroupDisplay> groups = new List<GroupDisplay>();
            foreach (KeyValuePair<int, GroupsEntry> knowGroup in this._networkBase[this._currentCell_NMI].KnowGroups)
            {
                GroupDisplay groupDisplay1 = new GroupDisplay();
                groupDisplay1.GSSI = knowGroup.Key;
                GroupsEntry groupsEntry = knowGroup.Value;
                groupDisplay1.Name = groupsEntry.Name;
                groupsEntry = knowGroup.Value;
                groupDisplay1.Priority = groupsEntry.Priority;
                GroupDisplay groupDisplay2 = groupDisplay1;
                groups.Add(groupDisplay2);
            }
            this._infoWindow.UpdateGroupGrid(groups);
        }

        private void Ch1RadioButton_CheckedChanged(object sender, EventArgs e) => this._channel1Listen = this.ch1RadioButton.Checked;

        private void Ch2RadioButton_CheckedChanged(object sender, EventArgs e) => this._channel2Listen = this.ch2RadioButton.Checked;

        private void Ch3RadioButton_CheckedChanged(object sender, EventArgs e) => this._channel3Listen = this.ch3RadioButton.Checked;

        private void Ch4RadioButton_CheckedChanged(object sender, EventArgs e) => this._channel4Listen = this.ch4RadioButton.Checked;

        private void AutoCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this._autoSelectChannel = this.autoCheckBox.Checked;
            if (!this._autoSelectChannel)
                return;
            this.ch1RadioButton.Checked = true;
        }

        private void NetInfoButton_Click(object sender, EventArgs e)
        {
            if (this._infoWindow.Visible)
            {
                this._needCloseInfo = true;
            }
            else
            {
                this._infoWindow.TopMost = this._tetraSettings.TopMostInfo;
                this._infoWindow.Show();
                this._showInfo = true;
            }
        }

        private void ConfigButton_Click(object sender, EventArgs e)
        {
            int num = (int)new DialogConfigure(this._tetraSettings, this).ShowDialog();
            this.UpdateGlobals();
        }

        /**
         * @todo verify of save results
         */
        private void UpdateGlobals() => GlobalFunction.IgnoreEncryptedSpeech = this._tetraSettings.IgnoreEncodedSpeech;

        private void BlockNumericUpDown_ValueChanged(object sender, EventArgs e) => this._tetraSettings.BlockedLevel = (int)this.blockNumericUpDown.Value;

        private void MainFrequencyLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => this._controlInterface.Frequency = this._mainCell_Frequency;

        private void Log_Tick(CallsEntry currentCalls)
        {
            if (!this._tetraSettings.LogEnabled || !this._processIsStarted)
                return;
            string stringToEntries = this.ParseStringToEntries(this._tetraSettings.LogEntryRules, currentCalls);
            string path = this.MakeFileName(this._tetraSettings.LogWriteFolder, this._tetraSettings.LogFileNameRules, ".csv");
            try
            {
                this._textFile.Write(stringToEntries, path);
            }
            catch
            {
                if (this._writerBlocked)
                    return;
                this._writerBlocked = true;
                if (MessageBox.Show("Unable to open file " + path, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand) != DialogResult.OK)
                    return;
                this._writerBlocked = false;
            }
        }

        public string ParseStringToEntries(string entryString, CallsEntry call)
        {
            string str1 = DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            string str2 = DateTime.Now.ToString("HH:mm:ss.fff");
            string str3 = this._currentCell_MCC.ToString();
            string str4 = this._currentCell_MNC.ToString();
            string str5 = this._currentCell_LA.ToString();
            string str6 = this._currentCell_CC.ToString();
            int num1;
            string str7;
            if (call == null)
            {
                str7 = "unknown";
            }
            else
            {
                num1 = call.Type;
                str7 = num1.ToString();
            }
            string str8 = str7;
            string str9;
            if (call == null)
            {
                str9 = "unknown";
            }
            else
            {
                num1 = call.From;
                str9 = num1.ToString();
            }
            string str10 = str9;
            string str11;
            if (call == null)
            {
                str11 = "unknown";
            }
            else
            {
                num1 = call.To;
                str11 = num1.ToString();
            }
            string str12 = str11;
            string str13 = call != null ? (call.IsClear == 1 ? "Clear" : "Encrypted") : "unknown";
            string str14 = call != null ? (call.Duplex == 0 ? "Simplex" : "Duplex") : "unknown";
            string str15;
            if (call == null)
            {
                str15 = "unknown";
            }
            else
            {
                num1 = call.AssignedSlot;
                str15 = num1.ToString();
            }
            string str16 = str15;
            string str17;
            if (call == null)
            {
                str17 = "unknown";
            }
            else
            {
                num1 = call.CallID;
                str17 = num1.ToString();
            }
            string str18 = str17;
            string str19;
            if (call == null)
            {
                str19 = "unknown";
            }
            else
            {
                num1 = call.Carrier;
                str19 = num1.ToString();
            }
            string str20 = str19;
            string str21 = "";
            int num2 = 0;
            while (num2 < entryString.Length)
            {
                if (this.CompareString(entryString, "date", num2))
                {
                    num2 += "date".Length;
                    str21 = str21 + str1 + this._tetraSettings.LogSeparator;
                }
                else if (this.CompareString(entryString, "time", num2))
                {
                    num2 += "time".Length;
                    str21 = str21 + str2 + this._tetraSettings.LogSeparator;
                }
                else if (this.CompareString(entryString, "carrier", num2))
                {
                    num2 += "carrier".Length;
                    str21 = str21 + str20 + this._tetraSettings.LogSeparator;
                }
                else if (this.CompareString(entryString, "mnc", num2))
                {
                    num2 += "mnc".Length;
                    str21 = str21 + str4 + this._tetraSettings.LogSeparator;
                }
                else if (this.CompareString(entryString, "mcc", num2))
                {
                    num2 += "mcc".Length;
                    str21 = str21 + str3 + this._tetraSettings.LogSeparator;
                }
                else if (this.CompareString(entryString, "la", num2))
                {
                    num2 += "la".Length;
                    str21 = str21 + str5 + this._tetraSettings.LogSeparator;
                }
                else if (this.CompareString(entryString, "cc", num2))
                {
                    num2 += "cc".Length;
                    str21 = str21 + str6 + this._tetraSettings.LogSeparator;
                }
                else if (this.CompareString(entryString, "type", num2))
                {
                    num2 += "type".Length;
                    str21 = str21 + str8 + this._tetraSettings.LogSeparator;
                }
                else if (this.CompareString(entryString, "from", num2))
                {
                    num2 += "from".Length;
                    str21 = str21 + str10 + this._tetraSettings.LogSeparator;
                }
                else if (this.CompareString(entryString, "to", num2))
                {
                    num2 += "to".Length;
                    str21 = str21 + str12 + this._tetraSettings.LogSeparator;
                }
                else if (this.CompareString(entryString, "encryption", num2))
                {
                    num2 += "encryption".Length;
                    str21 = str21 + str13 + this._tetraSettings.LogSeparator;
                }
                else if (this.CompareString(entryString, "duplex", num2))
                {
                    num2 += "duplex".Length;
                    str21 = str21 + str14 + this._tetraSettings.LogSeparator;
                }
                else if (this.CompareString(entryString, "slot", num2))
                {
                    num2 += "slot".Length;
                    str21 = str21 + str16 + this._tetraSettings.LogSeparator;
                }
                else if (this.CompareString(entryString, "callid", num2))
                {
                    num2 += "callid".Length;
                    str21 = str21 + str18 + this._tetraSettings.LogSeparator;
                }
                else if (this.CompareString(entryString, "+", num2) || this.CompareString(entryString, " ", num2))
                {
                    ++num2;
                }
                else
                {
                    if (!this.CompareString(entryString, "\"", num2))
                        return str21 + "-error!";
                    ++num2;
                    int num3 = entryString.IndexOf('"', num2);
                    if (num3 <= 0)
                        return str21 + "-error!";
                    for (int index = num2; index <= num3; ++index)
                    {
                        char ch = entryString[index];
                        ++num2;
                        str21 += ch.ToString();
                    }
                    str21 += this._tetraSettings.LogSeparator;
                }
            }
            return str21;
        }

        private static string GetFrequencyDisplay(long frequency)
        {
            long num = Math.Abs(frequency);
            return num != 0L ? (num <= 1500000000L ? (num <= 30000000L ? (num <= 1000L ? frequency.ToString() : string.Format("{0:#,#.###} kHz", (object)((double)frequency / 1000.0))) : string.Format("{0:0,0.000###} MHz", (object)((double)frequency / 1000000.0))) : string.Format("{0:#,0.000 000} GHz", (object)((double)frequency / 1000000000.0))) : "DC";
        }

        private string MakeFileName(string folder, string nameRules, string fileExtension)
        {
            string str1 = folder + this.ParseStringToPath(nameRules, fileExtension);
            string str2 = str1.Substring(0, str1.LastIndexOf("\\"));
            try
            {
                Directory.CreateDirectory(str2);
            }
            catch
            {
                int num = (int)MessageBox.Show("Unable to create directory", str2, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            return str1;
        }

        public string ParseStringToPath(string nameString, string extension)
        {
            string frequencyDisplay = TetraPanel.GetFrequencyDisplay(this._controlInterface.Frequency);
            string str1 = DateTime.Now.ToString("yyyy_MM_dd");
            string str2 = DateTime.Now.ToString("HH-mm-ss");
            string str3 = this._currentCell_MCC.ToString();
            string str4 = this._currentCell_MNC.ToString();
            string str5 = this._currentCell_LA.ToString();
            string str6 = this._currentCell_CC.ToString();
            string empty = string.Empty;
            string str7 = string.Empty;
            int num1 = 0;
            while (num1 < nameString.Length)
            {
                if (this.CompareString(nameString, "date", num1))
                {
                    num1 += "date".Length;
                    str7 += str1;
                }
                else if (this.CompareString(nameString, "time", num1))
                {
                    num1 += "time".Length;
                    str7 += str2;
                }
                else if (this.CompareString(nameString, "frequency", num1))
                {
                    num1 += "frequency".Length;
                    str7 += frequencyDisplay;
                }
                else if (this.CompareString(nameString, "mcc", num1))
                {
                    num1 += "mcc".Length;
                    str7 += str3;
                }
                else if (this.CompareString(nameString, "mnc", num1))
                {
                    num1 += "mnc".Length;
                    str7 += str4;
                }
                else if (this.CompareString(nameString, "cc", num1))
                {
                    num1 += "cc".Length;
                    str7 += str6;
                }
                else if (this.CompareString(nameString, "la", num1))
                {
                    num1 += "la".Length;
                    str7 += str5;
                }
                else if (this.CompareString(nameString, "to", num1))
                {
                    num1 += "to".Length;
                    str7 += empty;
                }
                else if (this.CompareString(nameString, "\\", num1) || this.CompareString(nameString, "/", num1))
                {
                    ++num1;
                    str7 += "\\";
                }
                else if (this.CompareString(nameString, "+", num1) || this.CompareString(nameString, " ", num1))
                {
                    ++num1;
                }
                else
                {
                    if (!this.CompareString(nameString, "\"", num1))
                        return str7 + "-error!";
                    ++num1;
                    int num2 = nameString.IndexOf('"', num1);
                    if (num2 <= 0)
                        return str7 + "-error!";
                    for (int index = num1; index <= num2; ++index)
                    {
                        char ch = nameString[index];
                        ++num1;
                        if (ch != '?' && ch != '/' && (ch != '\\' && ch != ':') && (ch != '<' && ch != '>' && (ch != '|' && ch != '*')) && ch != '"')
                            str7 += ch.ToString();
                    }
                }
            }
            if (str7.Length > 5)
            {
                if (str7.Substring(str7.Length - extension.Length, extension.Length) != extension)
                    str7 += extension;
                if (str7.Substring(0, 1) != "\\")
                    str7 = "\\" + str7;
            }
            return str7;
        }

        private bool CompareString(string source, string compare, int index) => index + compare.Length <= source.Length && source.Substring(index, compare.Length) == compare;

        /**
         * Autofrecuency Timer
         */
        private void AfcTimer_Tick(object sender, EventArgs e)
        {
            this.ferLabel.Text = string.Format("FER {0:F0}Hz", (object)this._freqError);
            if (this._tetraSettings.AfcDisabled || this._freqError <= 200.0 && this._freqError >= -200.0)
                return;
            this._isAfcWork = true;
            this._controlInterface.Frequency += (long)this._freqError;
            this._freqError = 0.0;
        }

        private void DataExtractorTimer_Tick(object sender, EventArgs e)
        {
            if (this._resetCounter > 0)
            {
                --this._resetCounter;
            }
            else
            {
                if (this._syncInfo != null)
                {
                    this._syncInfo.TryGetValue(GlobalNames.MCC, ref this._currentCell_MCC);
                    this._syncInfo.TryGetValue(GlobalNames.MNC, ref this._currentCell_MNC);
                    this._syncInfo.TryGetValue(GlobalNames.ColorCode, ref this._currentCell_CC);
                    this._currentCell_NMI = this._currentCell_MCC << 14 | this._currentCell_MNC;
                }
                while (this._rawData.Count > 0)
                {
                    this.UpdateSysInfo(this._rawData[0]);
                    this.UpdateCmceInfo(this._rawData[0]);
                    if (this._currentCell_NMI != 0)
                        this.UpdateCallsInfo(this._rawData[0]);
                    this._rawData.RemoveAt(0);
                }
                if (this._currentCell_NMI == 0)
                    return;
                int index = 0;
                int lastDateTime = this._lastDateTime;
                DateTime now = DateTime.Now;
                int second = now.Second;
                if (lastDateTime != second)
                {
                    now = DateTime.Now;
                    this._lastDateTime = now.Second;
                    while (index < this._currentCalls.Count)
                    {
                        if (this._currentCalls.ElementAt<KeyValuePair<int, CallsEntry>>(index).Value.WatchDog > 0)
                        {
                            --this._currentCalls.ElementAt<KeyValuePair<int, CallsEntry>>(index).Value.WatchDog;
                            ++index;
                        }
                        else
                            this._currentCalls.Remove(this._currentCalls.ElementAt<KeyValuePair<int, CallsEntry>>(index).Key);
                    }
                }
                foreach (CallsEntry callsEntry in this._currentCalls.Values)
                {
                    if (callsEntry.Carrier == this._currentCell_Carrier || callsEntry.Carrier == 0)
                    {
                        int assignedSlot = callsEntry.AssignedSlot;
                        int num = 0;
                        string str1 = "";
                        if (this._networkBase.ContainsKey(this._currentCell_NMI) && this._networkBase[this._currentCell_NMI].KnowGroups.ContainsKey(callsEntry.To))
                        {
                            GroupsEntry knowGroup = this._networkBase[this._currentCell_NMI].KnowGroups[callsEntry.To];
                            num = knowGroup.Priority;
                            knowGroup = this._networkBase[this._currentCell_NMI].KnowGroups[callsEntry.To];
                            str1 = knowGroup.Name;
                        }
                        if (callsEntry.From != 0)
                        {
                            int to;
                            if ((assignedSlot & 8) != 0)
                            {
                                this._currentCellLoad[0].CallId = callsEntry.CallID;
                                this._currentCellLoad[0].Type = callsEntry.Type;
                                this._currentCellLoad[0].From = callsEntry.From;
                                this._currentCellLoad[0].To = callsEntry.To;
                                this._currentCellLoad[0].GroupPriority = num;
                                CurrentLoad currentLoad = this._currentCellLoad[0];
                                string str2;
                                if (!(str1 != ""))
                                {
                                    to = callsEntry.To;
                                    str2 = to.ToString();
                                }
                                else
                                    str2 = str1;
                                currentLoad.GroupName = str2;
                                this._currentCellLoad[0].IsClear = callsEntry.IsClear == 1;
                            }
                            if ((assignedSlot & 4) != 0)
                            {
                                this._currentCellLoad[1].CallId = callsEntry.CallID;
                                this._currentCellLoad[1].Type = callsEntry.Type;
                                this._currentCellLoad[1].From = callsEntry.From;
                                this._currentCellLoad[1].To = callsEntry.To;
                                this._currentCellLoad[1].GroupPriority = num;
                                CurrentLoad currentLoad = this._currentCellLoad[1];
                                string str2;
                                if (!(str1 != ""))
                                {
                                    to = callsEntry.To;
                                    str2 = to.ToString();
                                }
                                else
                                    str2 = str1;
                                currentLoad.GroupName = str2;
                                this._currentCellLoad[1].IsClear = callsEntry.IsClear == 1;
                            }
                            if ((assignedSlot & 2) != 0)
                            {
                                this._currentCellLoad[2].CallId = callsEntry.CallID;
                                this._currentCellLoad[2].Type = callsEntry.Type;
                                this._currentCellLoad[2].From = callsEntry.From;
                                this._currentCellLoad[2].To = callsEntry.To;
                                this._currentCellLoad[2].GroupPriority = num;
                                CurrentLoad currentLoad = this._currentCellLoad[2];
                                string str2;
                                if (!(str1 != ""))
                                {
                                    to = callsEntry.To;
                                    str2 = to.ToString();
                                }
                                else
                                    str2 = str1;
                                currentLoad.GroupName = str2;
                                this._currentCellLoad[2].IsClear = callsEntry.IsClear == 1;
                            }
                            if ((assignedSlot & 1) != 0)
                            {
                                this._currentCellLoad[3].CallId = callsEntry.CallID;
                                this._currentCellLoad[3].Type = callsEntry.Type;
                                this._currentCellLoad[3].From = callsEntry.From;
                                this._currentCellLoad[3].To = callsEntry.To;
                                this._currentCellLoad[3].GroupPriority = num;
                                CurrentLoad currentLoad = this._currentCellLoad[3];
                                string str2;
                                if (!(str1 != ""))
                                {
                                    to = callsEntry.To;
                                    str2 = to.ToString();
                                }
                                else
                                    str2 = str1;
                                currentLoad.GroupName = str2;
                                this._currentCellLoad[3].IsClear = callsEntry.IsClear == 1;
                            }
                        }
                    }
                }
                if (!this._currentCalls.ContainsKey(this._currentCellLoad[0].CallId) || this._currentCalls[this._currentCellLoad[0].CallId].AssignedSlot == 0)
                    this._currentCellLoad[0] = new CurrentLoad();
                if (!this._currentCalls.ContainsKey(this._currentCellLoad[1].CallId) || this._currentCalls[this._currentCellLoad[1].CallId].AssignedSlot == 0)
                    this._currentCellLoad[1] = new CurrentLoad();
                if (!this._currentCalls.ContainsKey(this._currentCellLoad[2].CallId) || this._currentCalls[this._currentCellLoad[2].CallId].AssignedSlot == 0)
                    this._currentCellLoad[2] = new CurrentLoad();
                if (this._currentCalls.ContainsKey(this._currentCellLoad[3].CallId) && this._currentCalls[this._currentCellLoad[3].CallId].AssignedSlot != 0)
                    return;
                this._currentCellLoad[3] = new CurrentLoad();
            }
        }

        private void UpdateCmceInfo(ReceivedData data)
        {
            if (!data.Contains(GlobalNames.CMCE_Primitives_Type))
                return;
            if (this._cmceData.Count > 100)
                this._cmceData.RemoveAt(0);
            this._cmceData.Add(data);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = (IContainer)new Container();
            this.panel1 = new Panel();
            this.groupBox2 = new GroupBox();
            this.label11 = new Label();
            this.merLabel = new Label();
            this.berLabel = new Label();
            this.ferLabel = new Label();
            this.mainFrequencyLinkLabel = new LinkLabel();
            this.currentFrequencyLabel = new Label();
            this.currentCarrierLabel = new Label();
            this.mainCarrierLabel = new Label();
            this.freqLabel = new Label();
            this.currentFreqLabel = new Label();
            this.netInfoButton = new Button();
            this.laLabel = new Label();
            this.colorLabel = new Label();
            this.mncLabel = new Label();
            this.mccLabel = new Label();
            this.configButton = new Button();
            this.groupBox1 = new GroupBox();
            this.label12 = new Label();
            this.blockNumericUpDown = new NumericUpDown();
            this.gssiLabel = new Label();
            this.label6 = new Label();
            this.label7 = new Label();
            this.label8 = new Label();
            this.label9 = new Label();
            this.issiLabel = new Label();
            this.label4 = new Label();
            this.label3 = new Label();
            this.label2 = new Label();
            this.label1 = new Label();
            this.autoCheckBox = new CheckBox();
            this.ch4RadioButton = new RadioButton();
            this.ch3RadioButton = new RadioButton();
            this.ch2RadioButton = new RadioButton();
            this.ch1RadioButton = new RadioButton();
            this.displayGroupBox = new GroupBox();
            this.display = new Display();
            this.enabledCheckBox = new CheckBox();
            this.connectLabel = new Label();
            this.markerTimer = new System.Windows.Forms.Timer(this.components);
            this.timerGui = new System.Windows.Forms.Timer(this.components);
            this.afcTimer = new System.Windows.Forms.Timer(this.components);
            this.dataExtractorTimer = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.blockNumericUpDown.BeginInit();
            this.displayGroupBox.SuspendLayout();
            this.SuspendLayout();
            this.panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.panel1.Controls.Add((Control)this.groupBox2);
            this.panel1.Location = new Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(226, 493);
            this.panel1.TabIndex = 0;
            this.groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.groupBox2.Controls.Add((Control)this.label11);
            this.groupBox2.Controls.Add((Control)this.merLabel);
            this.groupBox2.Controls.Add((Control)this.berLabel);
            this.groupBox2.Controls.Add((Control)this.ferLabel);
            this.groupBox2.Controls.Add((Control)this.mainFrequencyLinkLabel);
            this.groupBox2.Controls.Add((Control)this.currentFrequencyLabel);
            this.groupBox2.Controls.Add((Control)this.currentCarrierLabel);
            this.groupBox2.Controls.Add((Control)this.mainCarrierLabel);
            this.groupBox2.Controls.Add((Control)this.freqLabel);
            this.groupBox2.Controls.Add((Control)this.currentFreqLabel);
            this.groupBox2.Controls.Add((Control)this.netInfoButton);
            this.groupBox2.Controls.Add((Control)this.laLabel);
            this.groupBox2.Controls.Add((Control)this.colorLabel);
            this.groupBox2.Controls.Add((Control)this.mncLabel);
            this.groupBox2.Controls.Add((Control)this.mccLabel);
            this.groupBox2.Controls.Add((Control)this.configButton);
            this.groupBox2.Controls.Add((Control)this.groupBox1);
            this.groupBox2.Controls.Add((Control)this.displayGroupBox);
            this.groupBox2.Controls.Add((Control)this.enabledCheckBox);
            this.groupBox2.Controls.Add((Control)this.connectLabel);
            this.groupBox2.Location = new Point(3, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(220, 490);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Demodulator";
            this.label11.AutoSize = true;
            this.label11.Location = new Point(91, 104);
            this.label11.Name = "label11";
            this.label11.Size = new Size(41, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "label11";
            this.merLabel.AutoSize = true;
            this.merLabel.Location = new Point(143, 271);
            this.merLabel.Name = "merLabel";
            this.merLabel.Size = new Size(31, 13);
            this.merLabel.TabIndex = 29;
            this.merLabel.Text = "MER";
            this.berLabel.AutoSize = true;
            this.berLabel.Location = new Point(81, 271);
            this.berLabel.Name = "berLabel";
            this.berLabel.Size = new Size(29, 13);
            this.berLabel.TabIndex = 29;
            this.berLabel.Text = "BER";
            this.ferLabel.AutoSize = true;
            this.ferLabel.Location = new Point(14, 271);
            this.ferLabel.Name = "ferLabel";
            this.ferLabel.Size = new Size(28, 13);
            this.ferLabel.TabIndex = 29;
            this.ferLabel.Text = "FER";
            this.mainFrequencyLinkLabel.AutoSize = true;
            this.mainFrequencyLinkLabel.Location = new Point(130, 38);
            this.mainFrequencyLinkLabel.Name = "mainFrequencyLinkLabel";
            this.mainFrequencyLinkLabel.Size = new Size(38, 13);
            this.mainFrequencyLinkLabel.TabIndex = 70;
            this.mainFrequencyLinkLabel.TabStop = true;
            this.mainFrequencyLinkLabel.Text = "0 MHz";
            this.mainFrequencyLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.MainFrequencyLinkLabel_LinkClicked);
            this.currentFrequencyLabel.AutoSize = true;
            this.currentFrequencyLabel.Location = new Point(130, 74);
            this.currentFrequencyLabel.Name = "currentFrequencyLabel";
            this.currentFrequencyLabel.Size = new Size(38, 13);
            this.currentFrequencyLabel.TabIndex = 69;
            this.currentFrequencyLabel.Text = "0 MHz";
            this.currentCarrierLabel.AutoSize = true;
            this.currentCarrierLabel.Location = new Point(92, 74);
            this.currentCarrierLabel.Name = "currentCarrierLabel";
            this.currentCarrierLabel.Size = new Size(31, 13);
            this.currentCarrierLabel.TabIndex = 68;
            this.currentCarrierLabel.Text = "0000";
            this.mainCarrierLabel.AutoSize = true;
            this.mainCarrierLabel.Location = new Point(91, 38);
            this.mainCarrierLabel.Name = "mainCarrierLabel";
            this.mainCarrierLabel.Size = new Size(31, 13);
            this.mainCarrierLabel.TabIndex = 67;
            this.mainCarrierLabel.Text = "0000";
            this.freqLabel.AutoSize = true;
            this.freqLabel.Location = new Point(90, 20);
            this.freqLabel.Name = "freqLabel";
            this.freqLabel.Size = new Size(33, 13);
            this.freqLabel.TabIndex = 54;
            this.freqLabel.Text = "Main:";
            this.currentFreqLabel.AutoSize = true;
            this.currentFreqLabel.Location = new Point(91, 56);
            this.currentFreqLabel.Name = "currentFreqLabel";
            this.currentFreqLabel.Size = new Size(44, 13);
            this.currentFreqLabel.TabIndex = 60;
            this.currentFreqLabel.Text = "Current:";
            this.netInfoButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.netInfoButton.Location = new Point(143, 99);
            this.netInfoButton.Name = "netInfoButton";
            this.netInfoButton.Size = new Size(69, 23);
            this.netInfoButton.TabIndex = 59;
            this.netInfoButton.Text = "Net Info";
            this.netInfoButton.UseVisualStyleBackColor = true;
            this.netInfoButton.Click += new EventHandler(this.NetInfoButton_Click);
            this.laLabel.AutoSize = true;
            this.laLabel.Location = new Point(6, 56);
            this.laLabel.Name = "laLabel";
            this.laLabel.Size = new Size(23, 13);
            this.laLabel.TabIndex = 53;
            this.laLabel.Text = "LA:";
            this.colorLabel.AutoSize = true;
            this.colorLabel.Location = new Point(6, 74);
            this.colorLabel.Name = "colorLabel";
            this.colorLabel.Size = new Size(34, 13);
            this.colorLabel.TabIndex = 49;
            this.colorLabel.Text = "Color:";
            this.mncLabel.AutoSize = true;
            this.mncLabel.Location = new Point(6, 38);
            this.mncLabel.Name = "mncLabel";
            this.mncLabel.Size = new Size(34, 13);
            this.mncLabel.TabIndex = 48;
            this.mncLabel.Text = "MNC:";
            this.mccLabel.AutoSize = true;
            this.mccLabel.Location = new Point(6, 20);
            this.mccLabel.Name = "mccLabel";
            this.mccLabel.Size = new Size(33, 13);
            this.mccLabel.TabIndex = 47;
            this.mccLabel.Text = "MCC:";
            this.configButton.Location = new Point(6, 99);
            this.configButton.Name = "configButton";
            this.configButton.Size = new Size(69, 23);
            this.configButton.TabIndex = 66;
            this.configButton.Text = "Config";
            this.configButton.UseVisualStyleBackColor = true;
            this.configButton.Click += new EventHandler(this.ConfigButton_Click);
            this.groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.groupBox1.Controls.Add((Control)this.label12);
            this.groupBox1.Controls.Add((Control)this.blockNumericUpDown);
            this.groupBox1.Controls.Add((Control)this.gssiLabel);
            this.groupBox1.Controls.Add((Control)this.label6);
            this.groupBox1.Controls.Add((Control)this.label7);
            this.groupBox1.Controls.Add((Control)this.label8);
            this.groupBox1.Controls.Add((Control)this.label9);
            this.groupBox1.Controls.Add((Control)this.issiLabel);
            this.groupBox1.Controls.Add((Control)this.label4);
            this.groupBox1.Controls.Add((Control)this.label3);
            this.groupBox1.Controls.Add((Control)this.label2);
            this.groupBox1.Controls.Add((Control)this.label1);
            this.groupBox1.Controls.Add((Control)this.autoCheckBox);
            this.groupBox1.Controls.Add((Control)this.ch4RadioButton);
            this.groupBox1.Controls.Add((Control)this.ch3RadioButton);
            this.groupBox1.Controls.Add((Control)this.ch2RadioButton);
            this.groupBox1.Controls.Add((Control)this.ch1RadioButton);
            this.groupBox1.Location = new Point(5, 128);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(209, 136);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Voice";
            this.label12.AutoSize = true;
            this.label12.Location = new Point(6, 111);
            this.label12.Name = "label12";
            this.label12.Size = new Size(94, 13);
            this.label12.TabIndex = 73;
            this.label12.Text = "Blocking threshold";
            this.blockNumericUpDown.Location = new Point(128, 109);
            this.blockNumericUpDown.Minimum = new Decimal(new int[4]
            {
        10,
        0,
        0,
        int.MinValue
            });
            this.blockNumericUpDown.Name = "blockNumericUpDown";
            this.blockNumericUpDown.Size = new Size(47, 20);
            this.blockNumericUpDown.TabIndex = 72;
            this.blockNumericUpDown.Value = new Decimal(new int[4]
            {
        1,
        0,
        0,
        int.MinValue
            });
            this.blockNumericUpDown.ValueChanged += new EventHandler(this.BlockNumericUpDown_ValueChanged);
            this.gssiLabel.AutoSize = true;
            this.gssiLabel.Location = new Point(118, 17);
            this.gssiLabel.Name = "gssiLabel";
            this.gssiLabel.Size = new Size(20, 13);
            this.gssiLabel.TabIndex = 71;
            this.gssiLabel.Text = "To";
            this.label6.Location = new Point(118, 88);
            this.label6.Name = "label6";
            this.label6.Size = new Size(85, 13);
            this.label6.TabIndex = 70;
            this.label6.Text = "0";
            this.label7.Location = new Point(118, 71);
            this.label7.Name = "label7";
            this.label7.Size = new Size(85, 13);
            this.label7.TabIndex = 69;
            this.label7.Text = "0";
            this.label8.Location = new Point(118, 54);
            this.label8.Name = "label8";
            this.label8.Size = new Size(85, 13);
            this.label8.TabIndex = 68;
            this.label8.Text = "0";
            this.label9.Location = new Point(118, 37);
            this.label9.Name = "label9";
            this.label9.Size = new Size(85, 13);
            this.label9.TabIndex = 67;
            this.label9.Text = "0";
            this.issiLabel.AutoSize = true;
            this.issiLabel.Location = new Point(53, 17);
            this.issiLabel.Name = "issiLabel";
            this.issiLabel.Size = new Size(30, 13);
            this.issiLabel.TabIndex = 65;
            this.issiLabel.Text = "From";
            this.label4.Location = new Point(53, 88);
            this.label4.Name = "label4";
            this.label4.Size = new Size(64, 13);
            this.label4.TabIndex = 64;
            this.label4.Text = "0";
            this.label3.Location = new Point(53, 71);
            this.label3.Name = "label3";
            this.label3.Size = new Size(64, 13);
            this.label3.TabIndex = 63;
            this.label3.Text = "0";
            this.label2.Location = new Point(53, 54);
            this.label2.Name = "label2";
            this.label2.Size = new Size(64, 13);
            this.label2.TabIndex = 62;
            this.label2.Text = "0";
            this.label1.Location = new Point(53, 37);
            this.label1.Name = "label1";
            this.label1.Size = new Size(64, 13);
            this.label1.TabIndex = 61;
            this.label1.Text = "0";
            this.autoCheckBox.AutoSize = true;
            this.autoCheckBox.Location = new Point(3, 16);
            this.autoCheckBox.Name = "autoCheckBox";
            this.autoCheckBox.Size = new Size(48, 17);
            this.autoCheckBox.TabIndex = 60;
            this.autoCheckBox.Text = "Auto";
            this.autoCheckBox.UseVisualStyleBackColor = true;
            this.autoCheckBox.CheckedChanged += new EventHandler(this.AutoCheckBox_CheckedChanged);
            this.ch4RadioButton.AutoSize = true;
            this.ch4RadioButton.Location = new Point(3, 86);
            this.ch4RadioButton.Name = "ch4RadioButton";
            this.ch4RadioButton.Size = new Size(46, 17);
            this.ch4RadioButton.TabIndex = 57;
            this.ch4RadioButton.TabStop = true;
            this.ch4RadioButton.Text = "Ts 4";
            this.ch4RadioButton.UseVisualStyleBackColor = true;
            this.ch4RadioButton.CheckedChanged += new EventHandler(this.Ch4RadioButton_CheckedChanged);
            this.ch3RadioButton.AutoSize = true;
            this.ch3RadioButton.Location = new Point(3, 69);
            this.ch3RadioButton.Name = "ch3RadioButton";
            this.ch3RadioButton.Size = new Size(46, 17);
            this.ch3RadioButton.TabIndex = 56;
            this.ch3RadioButton.TabStop = true;
            this.ch3RadioButton.Text = "Ts 3";
            this.ch3RadioButton.UseVisualStyleBackColor = true;
            this.ch3RadioButton.CheckedChanged += new EventHandler(this.Ch3RadioButton_CheckedChanged);
            this.ch2RadioButton.AutoSize = true;
            this.ch2RadioButton.Location = new Point(3, 52);
            this.ch2RadioButton.Name = "ch2RadioButton";
            this.ch2RadioButton.Size = new Size(46, 17);
            this.ch2RadioButton.TabIndex = 55;
            this.ch2RadioButton.TabStop = true;
            this.ch2RadioButton.Text = "Ts 2";
            this.ch2RadioButton.UseVisualStyleBackColor = true;
            this.ch2RadioButton.CheckedChanged += new EventHandler(this.Ch2RadioButton_CheckedChanged);
            this.ch1RadioButton.AutoSize = true;
            this.ch1RadioButton.Location = new Point(3, 35);
            this.ch1RadioButton.Name = "ch1RadioButton";
            this.ch1RadioButton.Size = new Size(46, 17);
            this.ch1RadioButton.TabIndex = 54;
            this.ch1RadioButton.TabStop = true;
            this.ch1RadioButton.Text = "Ts 1";
            this.ch1RadioButton.UseVisualStyleBackColor = true;
            this.ch1RadioButton.CheckedChanged += new EventHandler(this.Ch1RadioButton_CheckedChanged);
            this.displayGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.displayGroupBox.Controls.Add((Control)this.display);
            this.displayGroupBox.Location = new Point(5, 291);
            this.displayGroupBox.Name = "displayGroupBox";
            this.displayGroupBox.Size = new Size(209, 171);
            this.displayGroupBox.TabIndex = 42;
            this.displayGroupBox.TabStop = false;
            this.displayGroupBox.Text = "Diagram";
            this.display.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.display.BackColor = Color.Black;
            this.display.BackgroundImageLayout = ImageLayout.Zoom;
            this.display.BorderStyle = BorderStyle.Fixed3D;
            this.display.Location = new Point(6, 19);
            this.display.Name = "display";
            this.display.Size = new Size(193, 148);
            this.display.TabIndex = 27;
            this.enabledCheckBox.AutoSize = true;
            this.enabledCheckBox.Location = new Point(9, 0);
            this.enabledCheckBox.Name = "enabledCheckBox";
            this.enabledCheckBox.Size = new Size(86, 17);
            this.enabledCheckBox.TabIndex = 0;
            this.enabledCheckBox.Text = "Demodulator";
            this.enabledCheckBox.UseVisualStyleBackColor = true;
            this.enabledCheckBox.CheckedChanged += new EventHandler(this.EnabledCheckBox_CheckedChanged);
            this.connectLabel.AutoSize = true;
            this.connectLabel.ForeColor = Color.Red;
            this.connectLabel.Location = new Point(143, 1);
            this.connectLabel.Name = "connectLabel";
            this.connectLabel.Size = new Size(53, 13);
            this.connectLabel.TabIndex = 45;
            this.connectLabel.Text = "Received";
            this.connectLabel.Visible = false;
            this.markerTimer.Enabled = true;
            this.markerTimer.Interval = 50;
            this.markerTimer.Tick += new EventHandler(this.MarkerTimer_Tick);
            this.timerGui.Enabled = true;
            this.timerGui.Interval = 500;
            this.timerGui.Tick += new EventHandler(this.TimerGui_Tick);
            this.afcTimer.Enabled = true;
            this.afcTimer.Interval = 1000;
            this.afcTimer.Tick += new EventHandler(this.AfcTimer_Tick);
            this.dataExtractorTimer.Enabled = true;
            this.dataExtractorTimer.Interval = 50;
            this.dataExtractorTimer.Tick += new EventHandler(this.DataExtractorTimer_Tick);
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add((Control)this.panel1);
            this.Name = "TetraPanel";
            this.Size = new Size(229, 499);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.blockNumericUpDown.EndInit();
            this.displayGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
