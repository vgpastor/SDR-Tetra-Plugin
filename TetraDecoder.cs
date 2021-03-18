// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.TetraDecoder
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SDRSharp.Tetra
{
    internal class TetraDecoder
    {
        private PhyLevel _phyLevel = new PhyLevel();
        private LowerMacLevel _lowerMac = new LowerMacLevel();
        private MacLevel _parse = new MacLevel();
        private UnsafeBuffer _bbBuffer;
        private unsafe byte* _bbBufferPtr;
        private UnsafeBuffer _bkn1Buffer;
        private unsafe byte* _bkn1BufferPtr;
        private UnsafeBuffer _bkn2Buffer;
        private unsafe byte* _bkn2BufferPtr;
        private UnsafeBuffer _sb1Buffer;
        private unsafe byte* _sb1BufferPtr;
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
        private short[] _cdc = new short[276];
        private short[] _sdc = new short[480];
        private bool _haveErrors;

        public event TetraDecoder.DataReadyDelegate DataReady;

        public event TetraDecoder.SyncInfoReadyDelegate SyncInfoReady;

        public int NetworkTimeTN { get; internal set; }

        public int NetworkTimeFN { get; internal set; }

        public int NetworkTimeMN { get; internal set; }

        public long DownFrequency { get; private set; }

        public bool BurstReceived { get; internal set; }

        public float Ber { get; internal set; }

        public bool HaveErrors => this._haveErrors;

        public unsafe TetraDecoder(Control owner)
        {
            this._bbBuffer = UnsafeBuffer.Create(30);
            this._bbBufferPtr = (byte*)(void*)this._bbBuffer;
            this._bkn1Buffer = UnsafeBuffer.Create(216);
            this._bkn1BufferPtr = (byte*)(void*)this._bkn1Buffer;
            this._bkn2Buffer = UnsafeBuffer.Create(216);
            this._bkn2BufferPtr = (byte*)(void*)this._bkn2Buffer;
            this._sb1Buffer = UnsafeBuffer.Create(120);
            this._sb1BufferPtr = (byte*)(void*)this._sb1Buffer;
            this._owner = owner;
            this._fpass = 1;
            this._ch1InitStruct = NativeMethods.tetra_decode_init();
            this._ch2InitStruct = NativeMethods.tetra_decode_init();
            this._ch3InitStruct = NativeMethods.tetra_decode_init();
            this._ch4InitStruct = NativeMethods.tetra_decode_init();
        }

        public void Dispose()
        {
        }

        /**
         * I think that this is the entry point from SDR#
         */
        public unsafe int Process(Burst burst, float* audioOut)
        {
            int num = 0;
            this.BurstReceived = (uint)burst.Type > 0U;
            ++this._timeCounter;
            this.Ber = this._averageBer;
            if (this._timeCounter > 100)
            {
                this.Mer = (float)((double)this._badBurstCounter / (double)this._timeCounter * 100.0);
                this._timeCounter = 0;
                this._badBurstCounter = 0.0f;
            }
            this._networkTime.AddTimeSlot();
            if (burst.Type == BurstType.None)
            {
                this._haveErrors = true;
                if (this.TetraMode == Mode.TMO)
                    ++this._badBurstCounter;
                return num;
            }
            this._haveErrors = false;
            this._parse.ResetAACH();
            this._data.Clear();
            this._syncInfo.Clear();
            if (burst.Type == BurstType.SYNC)
            {
                this._phyLevel.ExtractSBChannels(burst, this._sb1BufferPtr);
                this._logicChannel = this._lowerMac.ExtractLogicChannelFromSB(this._sb1BufferPtr, this._sb1Buffer.Length);
                this._badBurstCounter += this._logicChannel.CrcIsOk ? 0.0f : 0.5f;
                this._haveErrors |= !this._logicChannel.CrcIsOk;
                this._averageBer = (float)((double)this._averageBer * 0.5 + (double)this._lowerMac.Ber * 0.5);
                if (this._logicChannel.CrcIsOk)
                {
                    this._parse.SyncPDU(this._logicChannel, this._syncInfo);
                    if (this._syncInfo.Value(GlobalNames.SystemCode) < 8)
                    {
                        this.TetraMode = Mode.TMO;
                        this._lowerMac.ScramblerCode = TetraUtils.CreateScramblerCode(this._syncInfo.Value(GlobalNames.MCC), this._syncInfo.Value(GlobalNames.MNC), this._syncInfo.Value(GlobalNames.ColorCode));
                        this._networkTime.Synchronize(this._syncInfo.Value(GlobalNames.TimeSlot), this._syncInfo.Value(GlobalNames.Frame), this._syncInfo.Value(GlobalNames.MultiFrame));
                    }
                    else
                    {
                        this.TetraMode = Mode.DMO;
                        if (this._syncInfo.Value(GlobalNames.SYNC_PDU_type) == 0)
                        {
                            if (this._syncInfo.Value(GlobalNames.Master_slave_link_flag) == 1 || this._syncInfo.Value(GlobalNames.Communication_type) == 0)
                                this._networkTime.SynchronizeMaster(this._syncInfo.Value(GlobalNames.TimeSlot), this._syncInfo.Value(GlobalNames.Frame));
                            else
                                this._networkTime.SynchronizeSlave(this._syncInfo.Value(GlobalNames.TimeSlot), this._syncInfo.Value(GlobalNames.Frame));
                        }
                    }
                }
                this._phyLevel.ExtractPhyChannels(this.TetraMode, burst, this._bbBufferPtr, this._bkn1BufferPtr, this._bkn2BufferPtr);
                if (this.TetraMode == Mode.TMO)
                {
                    this._logicChannel = this._lowerMac.ExtractLogicChannelFromBKN(this._bkn2BufferPtr, this._bkn2Buffer.Length);
                    this._logicChannel.TimeSlot = this._networkTime.TimeSlot;
                    this._logicChannel.Frame = this._networkTime.Frame;
                    this._badBurstCounter += this._logicChannel.CrcIsOk ? 0.0f : 0.5f;
                    this._haveErrors |= !this._logicChannel.CrcIsOk;
                    this._averageBer = (float)((double)this._averageBer * 0.5 + (double)this._lowerMac.Ber * 0.5);
                    if (this._logicChannel.CrcIsOk)
                        this._parse.TmoParseMacPDU(this._logicChannel, this._data);
                }
                else
                {
                    this._logicChannel = this._lowerMac.ExtractLogicChannelFromBKN2(this._bkn2BufferPtr, this._bkn2Buffer.Length);
                    this._logicChannel.TimeSlot = this._networkTime.TimeSlot;
                    this._logicChannel.Frame = this._networkTime.Frame;
                    this._badBurstCounter += this._logicChannel.CrcIsOk ? 0.0f : 0.5f;
                    this._haveErrors |= !this._logicChannel.CrcIsOk;
                    this._averageBer = (float)((double)this._averageBer * 0.5 + (double)this._lowerMac.Ber * 0.5);
                    if (this._logicChannel.CrcIsOk)
                    {
                        this._parse.SyncPDUHalfSlot(this._logicChannel, this._syncInfo);
                        if (this._syncInfo.Value(GlobalNames.Communication_type) == 0)
                        {
                            if (this._syncInfo.Contains(GlobalNames.MNC) && this._syncInfo.Contains(GlobalNames.Source_address))
                                this._lowerMac.ScramblerCode = TetraUtils.CreateScramblerCode(this._syncInfo.Value(GlobalNames.MNC), this._syncInfo.Value(GlobalNames.Source_address));
                        }
                        else if (this._syncInfo.Value(GlobalNames.Communication_type) == 1 && this._syncInfo.Contains(GlobalNames.Repeater_address) && this._syncInfo.Contains(GlobalNames.Source_address))
                            this._lowerMac.ScramblerCode = TetraUtils.CreateScramblerCode(this._syncInfo.Value(GlobalNames.Repeater_address), this._syncInfo.Value(GlobalNames.Source_address));
                    }
                }
                this.UpdateSyncInfo(this._syncInfo);
            }
            this.UpdatePublicProp();
            if (burst.Type != BurstType.SYNC)
            {
                this._phyLevel.ExtractPhyChannels(this.TetraMode, burst, this._bbBufferPtr, this._bkn1BufferPtr, this._bkn2BufferPtr);
                if (this.TetraMode == Mode.TMO)
                {
                    this._logicChannel = this._lowerMac.ExtractLogicChannelFromBB(this._bbBufferPtr, this._bbBuffer.Length);
                    this._logicChannel.TimeSlot = this._networkTime.TimeSlot;
                    this._logicChannel.Frame = this._networkTime.Frame;
                    this._badBurstCounter += this._logicChannel.CrcIsOk ? 0.0f : 0.2f;
                    this._haveErrors |= !this._logicChannel.CrcIsOk;
                    if (this._logicChannel.CrcIsOk)
                        this._parse.AccessAsignPDU(this._logicChannel);
                }
                switch (burst.Type)
                {
                    case BurstType.NDB1:
                        if (this.TetraMode == Mode.TMO && this._parse.DownLinkChannelType == ChannelType.Traffic || this.TetraMode == Mode.DMO && !this._networkTime.Frame18 && this._networkTime.TimeSlot == 1 || this.TetraMode == Mode.DMO && !this._networkTime.Frame18Slave && this._networkTime.TimeSlotSlave == 1)
                        {
                            this._logicChannel = this._lowerMac.ExtractVoiceDataFromBKN1BKN2(this._bkn1BufferPtr, this._bkn2BufferPtr, this._bkn1Buffer.Length);
                            num = this.DecodeAudio(audioOut, this._logicChannel.Ptr, this._logicChannel.Length, false, this._networkTime.TimeSlot) ? this._networkTime.TimeSlot : 0;
                            break;
                        }
                        this._logicChannel = this._lowerMac.ExtractLogicChannelFromBKN1BKN2(this._bkn1BufferPtr, this._bkn2BufferPtr, this._bkn1Buffer.Length);
                        this._logicChannel.TimeSlot = this._networkTime.TimeSlot;
                        this._logicChannel.Frame = this._networkTime.Frame;
                        this._badBurstCounter += this._logicChannel.CrcIsOk ? 0.0f : 0.8f;
                        this._haveErrors |= !this._logicChannel.CrcIsOk;
                        this._averageBer = (float)((double)this._averageBer * 0.5 + (double)this._lowerMac.Ber * 0.5);
                        if (this._logicChannel.CrcIsOk)
                        {
                            if (this.TetraMode == Mode.TMO)
                            {
                                this._parse.TmoParseMacPDU(this._logicChannel, this._data);
                                break;
                            }
                            this._parse.DmoParseMacPDU(this._logicChannel, this._data);
                            break;
                        }
                        break;
                    case BurstType.NDB2:
                        this._logicChannel = this._lowerMac.ExtractLogicChannelFromBKN(this._bkn1BufferPtr, this._bkn1Buffer.Length);
                        this._logicChannel.TimeSlot = this._networkTime.TimeSlot;
                        this._logicChannel.Frame = this._networkTime.Frame;
                        this._badBurstCounter += this._logicChannel.CrcIsOk ? 0.0f : 0.4f;
                        this._haveErrors |= !this._logicChannel.CrcIsOk;
                        this._averageBer = (float)((double)this._averageBer * 0.5 + (double)this._lowerMac.Ber * 0.5);
                        if (this._logicChannel.CrcIsOk)
                        {
                            if (this.TetraMode == Mode.TMO)
                                this._parse.TmoParseMacPDU(this._logicChannel, this._data);
                            else
                                this._parse.DmoParseMacPDU(this._logicChannel, this._data);
                        }
                        if (this._parse.DownLinkChannelType == ChannelType.Traffic && !this._parse.HalfSlotStolen || this.TetraMode == Mode.DMO && !this._networkTime.Frame18 && (this._networkTime.TimeSlot == 1 && !this._parse.HalfSlotStolen) || this.TetraMode == Mode.DMO && !this._networkTime.Frame18Slave && (this._networkTime.TimeSlotSlave == 1 && !this._parse.HalfSlotStolen))
                        {
                            this._logicChannel = this._lowerMac.ExtractVoiceDataFromBKN2(this._bkn2BufferPtr, this._bkn2Buffer.Length);
                            num = this.DecodeAudio(audioOut, this._logicChannel.Ptr, this._logicChannel.Length, true, this._networkTime.TimeSlot) ? this._networkTime.TimeSlot : 0;
                            break;
                        }
                        this._logicChannel = this._lowerMac.ExtractLogicChannelFromBKN(this._bkn2BufferPtr, this._bkn2Buffer.Length);
                        this._logicChannel.TimeSlot = this._networkTime.TimeSlot;
                        this._logicChannel.Frame = this._networkTime.Frame;
                        this._badBurstCounter += this._logicChannel.CrcIsOk ? 0.0f : 0.4f;
                        this._haveErrors |= !this._logicChannel.CrcIsOk;
                        this._averageBer = (float)((double)this._averageBer * 0.5 + (double)this._lowerMac.Ber * 0.5);
                        if (this._logicChannel.CrcIsOk)
                        {
                            if (this.TetraMode == Mode.TMO)
                            {
                                this._parse.TmoParseMacPDU(this._logicChannel, this._data);
                                break;
                            }
                            this._parse.DmoParseMacPDU(this._logicChannel, this._data);
                            break;
                        }
                        break;
                }
            }
            if (this._data.Count > 0)
                this.UpdateData(this._data);
            return num;
        }

        private void UpdateSyncInfo(ReceivedData syncInfo)
        {
            if (this.SyncInfoReady == null)
                return;
            this._owner.BeginInvoke((Delegate)this.SyncInfoReady, (object)syncInfo);
        }

        public void UpdateData(List<ReceivedData> data)
        {
            if (this.DataReady == null)
                return;
            this._owner.BeginInvoke((Delegate)this.DataReady, (object)data.ToList<ReceivedData>());
        }

        private void UpdatePublicProp()
        {
            if (this._networkTime == null)
                return;
            this.NetworkTimeFN = this._networkTime.Frame;
            this.NetworkTimeTN = this._networkTime.TimeSlot;
            this.NetworkTimeMN = this._networkTime.SuperFrame;
        }

        private unsafe bool DecodeAudio(
          float* audioBuffer,
          byte* buf,
          int length,
          bool stolten,
          int ch)
        {
            bool flag;
            fixed (short* numPtr1 = this._cdc)
            fixed (short* numPtr2 = this._sdc)
            {
                NativeMethods.tetra_cdec(this._fpass, buf, numPtr1, stolten ? 1 : 0);
                this._fpass = 0;
                flag = numPtr1[0] == (short)0 || numPtr1[138] == (short)0;
                this._badBurstCounter += numPtr1[0] == (short)0 | stolten ? 0.0f : 0.4f;
                this._badBurstCounter += numPtr1[138] == (short)0 ? 0.0f : 0.4f;
                void* chStruct = this._ch1InitStruct;
                switch (ch)
                {
                    case 1:
                        chStruct = this._ch1InitStruct;
                        break;
                    case 2:
                        chStruct = this._ch2InitStruct;
                        break;
                    case 3:
                        chStruct = this._ch3InitStruct;
                        break;
                    case 4:
                        chStruct = this._ch4InitStruct;
                        break;
                }
                NativeMethods.tetra_sdec(numPtr1, numPtr2, chStruct);
                this.ShortToFloatPtr(numPtr2, audioBuffer, this._sdc.Length);
            }
            return flag;
        }

        private unsafe void ShortToFloatPtr(short* source, float* dest, int length)
        {
            float num = 3.051851E-09f;
            for (int index = 0; index < length; ++index)
                dest[index] = (float)source[index] * num;
        }

        public float Mer { get; set; }

        public Mode TetraMode { get; set; }

        public delegate void DataReadyDelegate(List<ReceivedData> data);

        public delegate void SyncInfoReadyDelegate(ReceivedData syncInfo);
    }
}
