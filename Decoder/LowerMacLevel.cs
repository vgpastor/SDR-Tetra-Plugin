// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.LowerMacLevel
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using SDRSharp.Radio;

namespace SDRSharp.Tetra
{
    internal class LowerMacLevel
    {
        private Scrambler _scrambler;
        private Deinterleave _deinterleaver;
        private Depuncture _depuncture;
        private CRC16 _crc;
        private Rm3014 _rmd;
        private MotherCode _mother;
        private UnsafeBuffer _type1Buffer;
        private unsafe byte* _type1BufferPtr;
        private UnsafeBuffer _type2Buffer;
        private unsafe byte* _type2BufferPtr;
        private UnsafeBuffer _type3Buffer;
        private unsafe byte* _type3BufferPtr;
        private UnsafeBuffer _type4Buffer;
        private unsafe byte* _type4BufferPtr;
        private UnsafeBuffer _tempBuffer;
        private unsafe sbyte* _tempBufferPtr;
        private uint _scramblerSquence;
        private float _ber;

        public uint ScramblerCode
        {
            set => this._scramblerSquence = value;
        }

        public int TS { get; set; }

        public int FR { get; set; }

        public float Ber => this._ber;

        public unsafe LowerMacLevel()
        {
            this._scrambler = new Scrambler();
            this._rmd = new Rm3014();
            this._deinterleaver = new Deinterleave();
            this._depuncture = new Depuncture();
            this._mother = new MotherCode();
            this._crc = new CRC16();
            this._type1Buffer = UnsafeBuffer.Create(2048);
            this._type1BufferPtr = (byte*)(void*)this._type1Buffer;
            this._type2Buffer = UnsafeBuffer.Create(2048);
            this._type2BufferPtr = (byte*)(void*)this._type2Buffer;
            this._type3Buffer = UnsafeBuffer.Create(2048);
            this._type3BufferPtr = (byte*)(void*)this._type3Buffer;
            this._type4Buffer = UnsafeBuffer.Create(2048);
            this._type4BufferPtr = (byte*)(void*)this._type4Buffer;
            this._tempBuffer = UnsafeBuffer.Create(2048);
            this._tempBufferPtr = (sbyte*)(void*)this._tempBuffer;
            this._mother.DecoderInit();
            this._rmd.Init();
        }

        public unsafe LogicChannel ExtractLogicChannelFromBB(
          byte* type5BufferPtr,
          int length)
        {
            LogicChannel logicChannel = new LogicChannel();
            logicChannel.TimeSlot = this.TS;
            logicChannel.Frame = this.FR;
            logicChannel.Ptr = this._type1BufferPtr;
            logicChannel.Length = 14;
            this._scrambler.Process(type5BufferPtr, length, this._scramblerSquence);
            logicChannel.CrcIsOk = this._rmd.Process(type5BufferPtr, this._type1BufferPtr);
            return logicChannel;
        }

        public unsafe LogicChannel ExtractVoiceDataFromBKN1BKN2(
          byte* type5Buffer1,
          byte* type5Buffer2,
          int length)
        {
            LogicChannel logicChannel = new LogicChannel()
            {
                TimeSlot = this.TS,
                Frame = this.FR,
                Ptr = this._type4BufferPtr,
                Length = 432
            };
            for (int index = 0; index < length; ++index)
            {
                this._type4BufferPtr[index] = type5Buffer1[index];
                this._type4BufferPtr[index + length] = type5Buffer2[index];
            }
            length *= 2;
            this._scrambler.Process(this._type4BufferPtr, length, this._scramblerSquence);
            return logicChannel;
        }

        public unsafe LogicChannel ExtractVoiceDataFromBKN2(byte* type5Buffer1, int length)
        {
            LogicChannel logicChannel = new LogicChannel()
            {
                TimeSlot = this.TS,
                Frame = this.FR,
                Ptr = this._type4BufferPtr,
                Length = 432
            };
            for (int index = 0; index < length; ++index)
            {
                this._type4BufferPtr[index] = (byte)0;
                this._type4BufferPtr[index + 216] = type5Buffer1[index];
            }
            this._scrambler.Process(this._type4BufferPtr + length, length, this._scramblerSquence);
            return logicChannel;
        }

        public unsafe LogicChannel ExtractLogicChannelFromBKN1BKN2(
          byte* type5Buffer1,
          byte* type5Buffer2,
          int length)
        {
            LogicChannel logicChannel = new LogicChannel()
            {
                TimeSlot = this.TS,
                Frame = this.FR,
                Ptr = this._type1BufferPtr,
                Length = 268
            };
            for (int index = 0; index < length; ++index)
            {
                this._type4BufferPtr[index] = type5Buffer1[index];
                this._type4BufferPtr[index + length] = type5Buffer2[index];
            }
            length *= 2;
            this._scrambler.Process(this._type4BufferPtr, length, this._scramblerSquence);
            this._deinterleaver.Process(this._type4BufferPtr, this._type3BufferPtr, 432U, 103U);
            this._depuncture.Process(Depuncture.PunctType.PUNCT_2_3, this._type3BufferPtr, this._tempBufferPtr, 432);
            this._ber = this._mother.BufferDecode(this._tempBufferPtr, this._type2BufferPtr, 1152);
            logicChannel.CrcIsOk = this._crc.Process(this._type2BufferPtr, this._type1BufferPtr, 284);
            return logicChannel;
        }

        public unsafe LogicChannel ExtractLogicChannelFromBKN(byte* type5Buffer, int length)
        {
            LogicChannel logicChannel = new LogicChannel();
            logicChannel.TimeSlot = this.TS;
            logicChannel.Frame = this.FR;
            logicChannel.Ptr = this._type1BufferPtr;
            logicChannel.Length = 124;
            this._scrambler.Process(type5Buffer, length, this._scramblerSquence);
            this._deinterleaver.Process(type5Buffer, this._type3BufferPtr, 216U, 101U);
            this._depuncture.Process(Depuncture.PunctType.PUNCT_2_3, this._type3BufferPtr, this._tempBufferPtr, 216);
            this._ber = this._mother.BufferDecode(this._tempBufferPtr, this._type2BufferPtr, 576);
            logicChannel.CrcIsOk = this._crc.Process(this._type2BufferPtr, this._type1BufferPtr, 140);
            return logicChannel;
        }

        public unsafe LogicChannel ExtractLogicChannelFromSB(byte* type5Buffer, int length)
        {
            LogicChannel logicChannel = new LogicChannel();
            logicChannel.TimeSlot = this.TS;
            logicChannel.Frame = this.FR;
            logicChannel.Ptr = this._type1BufferPtr;
            logicChannel.Length = 60;
            this._scrambler.Process(type5Buffer, length, 3U);
            this._deinterleaver.Process(type5Buffer, this._type3BufferPtr, 120U, 11U);
            this._depuncture.Process(Depuncture.PunctType.PUNCT_2_3, this._type3BufferPtr, this._tempBufferPtr, 120);
            this._ber = this._mother.BufferDecode(this._tempBufferPtr, this._type2BufferPtr, 320);
            logicChannel.CrcIsOk = this._crc.Process(this._type2BufferPtr, this._type1BufferPtr, 76);
            return logicChannel;
        }

        public unsafe LogicChannel ExtractLogicChannelFromBKN2(
          byte* type5Buffer,
          int length)
        {
            LogicChannel logicChannel = new LogicChannel();
            logicChannel.TimeSlot = this.TS;
            logicChannel.Frame = this.FR;
            logicChannel.Ptr = this._type1BufferPtr;
            logicChannel.Length = 124;
            this._scrambler.Process(type5Buffer, length, 3U);
            this._deinterleaver.Process(type5Buffer, this._type3BufferPtr, 216U, 101U);
            this._depuncture.Process(Depuncture.PunctType.PUNCT_2_3, this._type3BufferPtr, this._tempBufferPtr, 216);
            this._ber = this._mother.BufferDecode(this._tempBufferPtr, this._type2BufferPtr, 576);
            logicChannel.CrcIsOk = this._crc.Process(this._type2BufferPtr, this._type1BufferPtr, 140);
            return logicChannel;
        }

        public void Dispose()
        {
        }
    }
}
