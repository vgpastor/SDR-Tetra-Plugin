// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.CRC16
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
  internal class CRC16
  {
    private const int GenPoly = 33800;
    private const int GoodCRC = 61624;

    public unsafe int CalcBuffer(byte* buffer, int length)
    {
      int maxValue = (int) ushort.MaxValue;
      for (int index = 0; index < length; ++index)
      {
        int num = (int) (ushort) ((uint) buffer[index] ^ (uint) (maxValue & 1));
        maxValue >>= 1;
        if (num != 0)
          maxValue ^= 33800;
      }
      return maxValue;
    }

    public unsafe bool Process(byte* source, byte* dest, int sourceLength)
    {
      int num = this.CalcBuffer(source, sourceLength);
      sourceLength -= 16;
      for (int index = 0; index < sourceLength; ++index)
        dest[index] = source[index];
      return num == 61624;
    }
  }
}
