// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.LlcLevel
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
    internal class LlcLevel
    {
        private const uint Poly = 3988292384;
        private const uint GoodFCS = 3736805603;
        private MleLevel _mle = new MleLevel();

        public unsafe void Parse(LogicChannel channelData, int offset, ReceivedData result)
        {
            LLCPduType int32 = (LLCPduType)TetraUtils.BitsToInt32(channelData.Ptr, offset, 4);
            offset += 4;
            result.Add(GlobalNames.LLC_Pdu_Type, (int)int32);
            bool flag = true;
            switch (int32)
            {
                case LLCPduType.BL_ADATA:
                    ++offset;
                    ++offset;
                    goto case LLCPduType.BL_UDATA;
                case LLCPduType.BL_DATA:
                    ++offset;
                    goto case LLCPduType.BL_UDATA;
                case LLCPduType.BL_UDATA:
                    if (flag)
                    {
                        this._mle.Parse(channelData, offset, result);
                        break;
                    }
                    result.Add(GlobalNames.UnknowData, 1);
                    break;
                case LLCPduType.BL_ACK:
                    ++offset;
                    goto case LLCPduType.BL_UDATA;
                case LLCPduType.BL_ADATA_FCS:
                    ++offset;
                    ++offset;
                    flag = this.CalculateFCS(channelData, offset);
                    goto case LLCPduType.BL_UDATA;
                case LLCPduType.BL_DATA_FCS:
                    ++offset;
                    flag = this.CalculateFCS(channelData, offset);
                    goto case LLCPduType.BL_UDATA;
                case LLCPduType.BL_UDATA_FCS:
                    flag = this.CalculateFCS(channelData, offset);
                    goto case LLCPduType.BL_UDATA;
                case LLCPduType.BL_ACK_FCS:
                    ++offset;
                    flag = this.CalculateFCS(channelData, offset);
                    goto case LLCPduType.BL_UDATA;
                default:
                    result.Add(GlobalNames.UnknowData, 1);
                    break;
            }
        }

        private unsafe bool CalculateFCS(LogicChannel channelData, int offset)
        {
            uint maxValue = uint.MaxValue;
            for (int index = offset; index < channelData.Length; ++index)
            {
                int num = (int)channelData.Ptr[index] ^ (int)maxValue & 1;
                maxValue >>= 1;
                if (num != 0)
                    maxValue ^= 3988292384U;
            }
            return maxValue == 3736805603U;
        }
    }
}
