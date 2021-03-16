// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.Deinterleave
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
  internal class Deinterleave
  {
    public unsafe void Process(byte* source, byte* dest, uint destLength, uint a)
    {
      for (uint index = 1; index <= destLength; ++index)
      {
        uint num = 1U + a * index % destLength;
        dest[index - 1U] = source[num - 1U];
      }
    }

    public unsafe void MatrixProcess(byte* source, byte* dest, uint lines, uint columns)
    {
      for (uint index1 = 0; index1 < columns; ++index1)
      {
        for (uint index2 = 0; index2 < lines; ++index2)
          dest[index2 * columns + lines] = source[index1 * lines + columns];
      }
    }
  }
}
