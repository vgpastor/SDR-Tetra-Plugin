using SDRSharp.Radio;

namespace SDRSharp.Tetra
{

    unsafe class LowerMacLevel   
    {
        private Scrambler _scrambler;
        private Deinterleave _deinterleaver;
        private Depuncture _depuncture;
        private CRC16 _crc;
        private Rm3014 _rmd;
        private MotherCode _mother;

        private UnsafeBuffer _type1Buffer;
        private byte* _type1BufferPtr;
        private UnsafeBuffer _type2Buffer;
        private byte* _type2BufferPtr;
        private UnsafeBuffer _type3Buffer;
        private byte* _type3BufferPtr;
        private UnsafeBuffer _type4Buffer;
        private byte* _type4BufferPtr;
        private UnsafeBuffer _tempBuffer;
        private sbyte* _tempBufferPtr;

        private uint _scramblerSquence;
        private float _ber;
  
        public uint ScramblerCode
        {
            set { _scramblerSquence = value; }
        }

        public int TS
        {
            get;
            set;
        }

        public int FR
        {
            get;
            set;
        }

        public float Ber
        {
            get { return _ber; } 
        }
        public LowerMacLevel()
        {
            _scrambler = new Scrambler();
            _rmd = new Rm3014();
            _deinterleaver = new Deinterleave();
            _depuncture = new Depuncture();
            _mother = new MotherCode();
            _crc = new CRC16();

            _type1Buffer = UnsafeBuffer.Create(2048);
            _type1BufferPtr = (byte*)_type1Buffer;
            _type2Buffer = UnsafeBuffer.Create(2048);
            _type2BufferPtr = (byte*)_type2Buffer;
            _type3Buffer = UnsafeBuffer.Create(2048);
            _type3BufferPtr = (byte*)_type3Buffer;
            _type4Buffer = UnsafeBuffer.Create(2048);
            _type4BufferPtr = (byte*)_type4Buffer;
            _tempBuffer = UnsafeBuffer.Create(2048);
            _tempBufferPtr = (sbyte*)_tempBuffer;

            _mother.DecoderInit();
            _rmd.Init();
        }

        public LogicChannel ExtractLogicChannelFromBB(byte* type5BufferPtr, int length)
        {

            var result = new LogicChannel
            {
                TimeSlot = TS,
                Frame = FR,
                Ptr = _type1BufferPtr,
                Length = 14
            };

            _scrambler.Process(type5BufferPtr, length, _scramblerSquence);
            result.CrcIsOk = _rmd.Process(type5BufferPtr, _type1BufferPtr);

            return result;
        }

        public LogicChannel ExtractVoiceDataFromBKN1BKN2(byte* type5Buffer1, byte* type5Buffer2, int length)
        {
            var result = new LogicChannel
            {
                TimeSlot = TS,
                Frame = FR,
                Ptr = _type4BufferPtr,
                Length = 432
            };

            for (int i = 0; i < length; i++)
            {
                _type4BufferPtr[i] = type5Buffer1[i];
                _type4BufferPtr[i + length] = type5Buffer2[i];
            }

            length *= 2;

            _scrambler.Process(_type4BufferPtr, length, _scramblerSquence);

            return result;
        }

        public LogicChannel ExtractVoiceDataFromBKN2(byte* type5Buffer1, int length)
        {
            var result = new LogicChannel
            {
                TimeSlot = TS,
                Frame = FR,
                Ptr = _type4BufferPtr,
                Length = 432
            };

            for (int i = 0; i < length; i++)
            {
                _type4BufferPtr[i] = 0;
                _type4BufferPtr[i + 216] = type5Buffer1[i];
            }

            _scrambler.Process(_type4BufferPtr + length , length, _scramblerSquence);

            return result;
        }

        public LogicChannel ExtractLogicChannelFromBKN1BKN2(byte* type5Buffer1, byte* type5Buffer2, int length)
        {

            var result = new LogicChannel
            {
                TimeSlot = TS,
                Frame = FR,
                Ptr = _type1BufferPtr,
                Length = 268
            };

            for (int i = 0; i < length; i++)
            {
                _type4BufferPtr[i] = type5Buffer1[i];
                _type4BufferPtr[i + length] = type5Buffer2[i];
            }

            length *= 2;

            _scrambler.Process(_type4BufferPtr, length, _scramblerSquence);
            _deinterleaver.Process(_type4BufferPtr, _type3BufferPtr, 432, 103);
            _depuncture.Process(Depuncture.PunctType.PUNCT_2_3, _type3BufferPtr, _tempBufferPtr, 432);

            _ber = _mother.BufferDecode(_tempBufferPtr, _type2BufferPtr, 288 * 4);

            result.CrcIsOk = _crc.Process(_type2BufferPtr, _type1BufferPtr, 284);

            return result;
        }

        public LogicChannel ExtractLogicChannelFromBKN(byte* type5Buffer, int length)
        {
            //SCH_HD BNCH
            var result = new LogicChannel
            {
                TimeSlot = TS,
                Frame = FR,
                Ptr = _type1BufferPtr,
                Length = 124
            };

            _scrambler.Process(type5Buffer, length, _scramblerSquence);
            _deinterleaver.Process(type5Buffer, _type3BufferPtr, 216, 101);
            _depuncture.Process(Depuncture.PunctType.PUNCT_2_3, _type3BufferPtr, _tempBufferPtr, 216);

            _ber = _mother.BufferDecode(_tempBufferPtr, _type2BufferPtr, 144 * 4);

            result.CrcIsOk = _crc.Process(_type2BufferPtr, _type1BufferPtr, 140);

            return result;
        }

        public LogicChannel ExtractLogicChannelFromSB(byte* type5Buffer, int length)
        {
            //BSCH SCH_HD
            var result = new LogicChannel
            {
                TimeSlot = TS,
                Frame = FR,
                Ptr = _type1BufferPtr,
                Length = 60
            };

            _scrambler.Process(type5Buffer, length, Scrambler.DefaultScramblerInit);

            _deinterleaver.Process(type5Buffer, _type3BufferPtr, 120, 11);

            _depuncture.Process(Depuncture.PunctType.PUNCT_2_3, _type3BufferPtr, _tempBufferPtr, 120);

            _ber = _mother.BufferDecode(_tempBufferPtr, _type2BufferPtr, 80 * 4);

            result.CrcIsOk = _crc.Process(_type2BufferPtr, _type1BufferPtr, 76);

            return result;
        }

        public LogicChannel ExtractLogicChannelFromBKN2(byte* type5Buffer, int length)
        {
            //SCH_HD
            var result = new LogicChannel
            {
                TimeSlot = TS,
                Frame = FR,
                Ptr = _type1BufferPtr,
                Length = 124
            };

            _scrambler.Process(type5Buffer, length, Scrambler.DefaultScramblerInit);
            _deinterleaver.Process(type5Buffer, _type3BufferPtr, 216, 101);
            _depuncture.Process(Depuncture.PunctType.PUNCT_2_3, _type3BufferPtr, _tempBufferPtr, 216);

            _ber = _mother.BufferDecode(_tempBufferPtr, _type2BufferPtr, 144 * 4);

            result.CrcIsOk = _crc.Process(_type2BufferPtr, _type1BufferPtr, 140);

            return result;
        }

        public void Dispose()
        {
            
        }
    }
}
