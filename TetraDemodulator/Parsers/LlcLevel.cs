using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDRSharp.Tetra
{
    unsafe class LlcLevel
    {
        private MleLevel _mle = new MleLevel();

        public void Parse(LogicChannel channelData, int offset, Dictionary<GlobalNames, int> result)
        {
            var llcType = (LLCPduType)TetraUtils.BitsToInt32(channelData.Ptr, offset, 4);
            offset += 4;
            result.Add(GlobalNames.LLC_Pdu_Type, (int)llcType);

            Debug.Write(" " + llcType.ToString());

            switch (llcType)
            {
                case LLCPduType.BL_ADATA_FCS:
                case LLCPduType.BL_ADATA:
                    offset += 1; //N(R)
                    offset += 1; //N(S)
                    break;

                case LLCPduType.BL_DATA:
                case LLCPduType.BL_DATA_FCS:
                    offset += 1; //N(S)
                    break;

                case LLCPduType.BL_UDATA_FCS:
                case LLCPduType.BL_UDATA:
                    //No bits here
                    break;

                case LLCPduType.BL_ACK_FCS:
                case LLCPduType.BL_ACK:
                    offset += 1;//N(R)
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
                    Debug.Write(" Unknow");
                    offset = -1;
                    break;
            }

            if (offset > 0)
                _mle.Parse(channelData, offset, result);
        }
    }
}
