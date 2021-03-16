// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.TetraUtils
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
  internal class TetraUtils
  {
    public static unsafe byte BitsToByte(byte* bitsBuffer, int offset, int length)
    {
      byte num = 0;
      for (int index = 0; index < length; ++index)
        num = (byte) ((uint) (byte) ((uint) num << 1) | (uint) (byte) ((uint) bitsBuffer[index + offset] & 1U));
      return num;
    }

    public static unsafe byte BitsToChar(byte* bitsBuffer, int offset, int length)
    {
      byte num = 0;
      for (int index = 0; index < length; ++index)
        num = (byte) ((uint) (byte) ((uint) num >> 1) | (uint) (byte) (((int) bitsBuffer[index + offset] & 1) << 7));
      return num;
    }

    public static unsafe uint BitsToUInt32(byte* bitsBuffer, int offset, int length)
    {
      uint num = 0;
      for (int index = 0; index < length; ++index)
        num = num << 1 | (uint) bitsBuffer[index + offset] & 1U;
      return num;
    }

    public static unsafe int BitsToInt32(byte* bitsBuffer, int offset, int length)
    {
      int num = 0;
      for (int index = 0; index < length; ++index)
        num = num << 1 | (int) bitsBuffer[index + offset] & 1;
      return num;
    }

    public static unsafe ulong BitsToULong(byte* bitsBuffer, int offset, int length)
    {
      ulong num = 0;
      for (int index = 0; index < length; ++index)
        num = num << 1 | (ulong) bitsBuffer[index + offset] & 1UL;
      return num;
    }

    public static unsafe long BitsToLong(byte* bitsBuffer, int offset, int length)
    {
      long num = 0;
      for (int index = 0; index < length; ++index)
        num = num << 1 | (long) bitsBuffer[index + offset] & 1L;
      return num;
    }

    public static unsafe string BitsToString(byte* bitsBuffer, int offset, int length)
    {
      string empty = string.Empty;
      for (int index = 0; index < length; ++index)
        empty += bitsBuffer[index + offset] == (byte) 0 ? "0" : "1";
      return empty;
    }

    public static unsafe string BitsToString(sbyte* bitsBuffer, int offset, int length)
    {
      string empty = string.Empty;
      for (int index = 0; index < length; ++index)
        empty += bitsBuffer[index + offset] == (sbyte) 0 ? "-" : (bitsBuffer[index + offset] == (sbyte) -1 ? "0" : "1");
      return empty;
    }

    public static unsafe void ByteToBits(byte data, byte* bitsBuffer, int bufferOffset)
    {
      int num = 8;
      for (int index = 0; index < num; ++index)
      {
        bitsBuffer[bufferOffset + index] = ((int) data & 128) == 0 ? (byte) 0 : (byte) 1;
        data <<= 1;
      }
    }

    public static unsafe void IntToBits(int data, byte* bitsBuffer, int bufferOffset)
    {
      int num = 32;
      for (int index = 0; index < num; ++index)
      {
        bitsBuffer[bufferOffset + index] = ((long) data & 2147483648L) == 0L ? (byte) 0 : (byte) 1;
        data <<= 1;
      }
    }

    public static unsafe void UIntToBits(uint data, byte* bitsBuffer, int firstBitOffset)
    {
      int num1 = 32 - firstBitOffset;
      uint num2 = 2147483648U >> firstBitOffset;
      for (int index = 0; index < num1; ++index)
      {
        bitsBuffer[index] = ((int) data & (int) num2) == 0 ? (byte) 0 : (byte) 1;
        data <<= 1;
      }
    }

    public static uint CreateScramblerCode(int mcc, int mnc, int colour)
    {
      mcc &= 1023;
      mnc &= 16383;
      colour &= 63;
      return (uint) (mcc << 22 | mnc << 8 | colour << 2 | 3);
    }

    public static uint CreateScramblerCode(int mnc, int sourceAddress)
    {
      mnc &= 63;
      sourceAddress &= 16777215;
      return (uint) (mnc << 26 | sourceAddress << 2 | 3);
    }
  }
}
