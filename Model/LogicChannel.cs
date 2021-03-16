// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.LogicChannel
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
  public class LogicChannel
  {
    public unsafe byte* Ptr;
    public int Length;
    public bool CrcIsOk;
    public int TimeSlot;
    public int Frame;
  }
}
