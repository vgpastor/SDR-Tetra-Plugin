// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.Scrambler
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using SDRSharp.Radio;

namespace SDRSharp.Tetra
{
  internal class Scrambler
  {
    public const uint DefaultScramblerInit = 3;
    private const uint ScramblerPoly = 3681617473;
    private UnsafeBuffer _onesCount;
    private unsafe byte* _onesCountPtr;

    public unsafe Scrambler()
    {
      this._onesCount = UnsafeBuffer.Create(256, 1);
      this._onesCountPtr = (byte*) (void*) this._onesCount;
      for (uint index = 0; index < 256U; ++index)
      {
        uint num1 = index;
        uint num2 = 0;
        for (; num1 != 0U; num1 >>= 1)
          num2 += num1 & 1U;
        this._onesCountPtr[index] = (byte) num2;
      }
    }

    public unsafe void Process(byte* buffer, int length, uint scrambSequence)
    {
      for (int index = 0; index < length; ++index)
      {
        uint num1 = scrambSequence & 3681617473U;
        uint num2 = (uint) ((int) this._onesCountPtr[num1 >> 24 & (uint) byte.MaxValue] + (int) this._onesCountPtr[num1 >> 16 & (uint) byte.MaxValue] + (int) this._onesCountPtr[num1 >> 8 & (uint) byte.MaxValue] + (int) this._onesCountPtr[num1 & (uint) byte.MaxValue] & 1);
        scrambSequence = scrambSequence >> 1 | num2 << 31;
        byte* numPtr = buffer + index;
        *numPtr = (byte) ((uint) *numPtr ^ (uint) (byte) num2);
      }
    }
  }
}
