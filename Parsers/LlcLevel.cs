using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SDRSharp.Tetra
{
    unsafe class LlcLevel
    {
        private const UInt32 Poly = 0xedb88320;
        private const UInt32 GoodFCS = 0xdebb20e3;

        private MleLevel _mle = new MleLevel();

        public void Parse(LogicChannel channelData, int offset, ReceivedData result)
        {
            var llcType = (LLCPduType)TetraUtils.BitsToInt32(channelData.Ptr, offset, 4);
            offset += 4;
            result.Add(GlobalNames.LLC_Pdu_Type, (int)llcType);

            //Debug.WriteLine(" " + llcType.ToString());

            var fcsIsGood = true;

            switch (llcType)
            {
                case LLCPduType.BL_ADATA:
                    offset += 1; //N(R)
                    offset += 1; //N(S)
                    break;

                case LLCPduType.BL_ADATA_FCS:
                    offset += 1; //N(R)
                    offset += 1; //N(S)
                    fcsIsGood = CalculateFCS(channelData, offset);
                    break;

                case LLCPduType.BL_DATA:
                    offset += 1; //N(S)
                    break;

                case LLCPduType.BL_DATA_FCS:
                    offset += 1; //N(S)
                    fcsIsGood = CalculateFCS(channelData, offset);
                    break;

                case LLCPduType.BL_UDATA:
                    //No bits here
                    break;

                case LLCPduType.BL_UDATA_FCS:
                    //No bits here
                    fcsIsGood = CalculateFCS(channelData, offset);
                    break;

                case LLCPduType.BL_ACK:
                    offset += 1;//N(R)
                    break;

                case LLCPduType.BL_ACK_FCS:
                    offset += 1;//N(R)
                    fcsIsGood = CalculateFCS(channelData, offset);
                    break;

                case LLCPduType.AL_SETUP:
                case LLCPduType.AL_DATA_AR_FINAL:
                case LLCPduType.AL_UDATA_UFINAL:
                case LLCPduType.AL_ACK_RNR:
                case LLCPduType.AL_RECONNECT:
                case LLCPduType.Reserved1:
                case LLCPduType.Reserved2:
                case LLCPduType.AL_DISC:
                default:
                    Debug.WriteLine(" Unknow_LLC_PDU "+ llcType);
                    result.Add(GlobalNames.UnknowData, 1);
                    return;
            }

            if (fcsIsGood)
            {
                _mle.Parse(channelData, offset, result);
            }
            else
            {
                result.Add(GlobalNames.UnknowData, 1);
            }
        }

        private bool CalculateFCS(LogicChannel channelData, int offset)
        {
            var lsfr = (UInt32)0xffffffff;
            var bit = (UInt32)0;

            for (int i = offset; i < channelData.Length; i++)
            {
                bit = channelData.Ptr[i] ^ (lsfr & 0x1);

                lsfr >>= 1;

                if (bit != 0) lsfr ^= Poly;
            }

            Debug.WriteLine(" FCS_" + (lsfr == GoodFCS ? "Ok" : "Err"));

            return lsfr == GoodFCS;
        }
    }
}
