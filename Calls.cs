// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.Calls
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
  public struct Calls
  {
    public int CallIdent;
    public int TXer;
    public int SSI1;
    public int SSI2;
    public bool IsEncripted;
    public bool Duplex;
    public int AssignedSlot;
  }
}
