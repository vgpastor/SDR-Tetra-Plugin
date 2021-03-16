// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.LLCPduType
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
  public enum LLCPduType
  {
    BL_ADATA,
    BL_DATA,
    BL_UDATA,
    BL_ACK,
    BL_ADATA_FCS,
    BL_DATA_FCS,
    BL_UDATA_FCS,
    BL_ACK_FCS,
    AL_SETUP,
    AL_DATA_AR_FINAL,
    AL_UDATA_UFINAL,
    AL_ACK_RNR,
    AL_RECONNECT,
    Reserved1,
    Reserved2,
    AL_DISC,
  }
}
