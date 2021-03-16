// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.DMAC_Message_Type
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
    public enum DMAC_Message_Type
    {
        DM_RESERVED,
        DM_SDS_OCCUPIED,
        DM_TIMING_REQUEST,
        DM_TIMING_ACK,
        Reserved4,
        Reserved5,
        Reserved6,
        Reserved7,
        DM_SETUP,
        DM_SETUP_PRES,
        DM_CONNECT,
        DM_DISCONNECT,
        DM_CONNECT_ACK,
        DM_OCCUPIED,
        DM_RELEASE,
        DM_TX_CEASED,
        DM_TX_REQUEST,
        DM_TX_ACCEPT,
        DM_PREEMPT,
        DM_PRE_ACCEPT,
        DM_REJECT,
        DM_INFO,
        DM_SDS_UDATA,
        DM_SDS_DATA,
        DM_SDS_ACK,
        Gateway_specific_messages,
        Reserved26,
        Reserved27,
        Reserved28,
        Reserved29,
        Proprietary30,
        Proprietary31,
    }
}
