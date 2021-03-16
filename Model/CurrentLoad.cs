// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.CurrentLoad
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
  public class CurrentLoad
  {
    public int To { get; set; }

    public int From { get; set; }

    public int Type { get; set; }

    public string GroupName { get; set; }

    public int GroupPriority { get; set; }

    public bool IsClear { get; set; }

    public int CallId { get; set; }
  }
}
