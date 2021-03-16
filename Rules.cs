// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.Rules
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
  public struct Rules
  {
    public GlobalNames GlobalName;
    public int Length;
    public RulesType Type;
    public int Ext1;
    public int Ext2;
    public int Ext3;

    public Rules(
      GlobalNames globalName,
      int length,
      RulesType type = RulesType.Direct,
      int ext1 = 0,
      int ext2 = 0,
      int ext3 = 0)
    {
      this.GlobalName = globalName;
      this.Length = length;
      this.Type = type;
      this.Ext1 = ext1;
      this.Ext2 = ext2;
      this.Ext3 = ext3;
    }
  }
}
