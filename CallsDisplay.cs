// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.CallsDisplay
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
  public class CallsDisplay
  {
    public int Carrier { get; set; }

    public string AssignedSlot { get; set; }

    public int CallID { get; set; }

    public string Type { get; set; }

    public int From { get; set; }

    public string To { get; set; }

    public string Encrypted { get; set; }

    public string Duplex { get; set; }
  }
}
