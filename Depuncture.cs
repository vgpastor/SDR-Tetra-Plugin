// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.Depuncture
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
  internal class Depuncture
  {
    private static byte[] P_rate1_3 = new byte[7]
    {
      (byte) 0,
      (byte) 1,
      (byte) 2,
      (byte) 3,
      (byte) 5,
      (byte) 6,
      (byte) 7
    };
    private static byte[] P_rate2_3 = new byte[4]
    {
      (byte) 0,
      (byte) 1,
      (byte) 2,
      (byte) 5
    };
    private static byte[] P_rate8_12 = new byte[4]
    {
      (byte) 0,
      (byte) 1,
      (byte) 2,
      (byte) 4
    };
    private static byte[] P_rate8_18 = new byte[10]
    {
      (byte) 0,
      (byte) 1,
      (byte) 2,
      (byte) 3,
      (byte) 4,
      (byte) 5,
      (byte) 7,
      (byte) 8,
      (byte) 10,
      (byte) 11
    };
    private static byte[] P_rate8_17 = new byte[18]
    {
      (byte) 0,
      (byte) 1,
      (byte) 2,
      (byte) 3,
      (byte) 4,
      (byte) 5,
      (byte) 7,
      (byte) 8,
      (byte) 10,
      (byte) 11,
      (byte) 13,
      (byte) 14,
      (byte) 16,
      (byte) 17,
      (byte) 19,
      (byte) 20,
      (byte) 22,
      (byte) 23
    };

    public unsafe void Process(
      Depuncture.PunctType puncType,
      byte* source,
      sbyte* dest,
      int sourceLength)
    {
      for (int index = 0; index < sourceLength * 4; ++index)
        dest[index] = (sbyte) 0;
      fixed (byte* numPtr1 = Depuncture.P_rate1_3)
        fixed (byte* numPtr2 = Depuncture.P_rate2_3)
          fixed (byte* numPtr3 = Depuncture.P_rate8_12)
            fixed (byte* numPtr4 = Depuncture.P_rate8_18)
              fixed (byte* numPtr5 = Depuncture.P_rate8_17)
              {
                byte* numPtr6;
                byte num1;
                byte num2;
                Depuncture.FunctionType functionType;
                switch (puncType)
                {
                  case Depuncture.PunctType.PUNCT_2_3:
                    numPtr6 = numPtr2;
                    num1 = (byte) 3;
                    num2 = (byte) 8;
                    functionType = Depuncture.FunctionType.Equals;
                    break;
                  case Depuncture.PunctType.PUNCT_1_3:
                    numPtr6 = numPtr1;
                    num1 = (byte) 6;
                    num2 = (byte) 8;
                    functionType = Depuncture.FunctionType.Equals;
                    break;
                  case Depuncture.PunctType.PUNCT_292_432:
                    numPtr6 = numPtr2;
                    num1 = (byte) 3;
                    num2 = (byte) 8;
                    functionType = Depuncture.FunctionType.Func_292;
                    break;
                  case Depuncture.PunctType.PUNCT_148_432:
                    numPtr6 = numPtr1;
                    num1 = (byte) 6;
                    num2 = (byte) 8;
                    functionType = Depuncture.FunctionType.Func_148;
                    break;
                  case Depuncture.PunctType.PUNCT_112_168:
                    numPtr6 = numPtr3;
                    num1 = (byte) 3;
                    num2 = (byte) 6;
                    functionType = Depuncture.FunctionType.Equals;
                    break;
                  case Depuncture.PunctType.PUNCT_72_162:
                    numPtr6 = numPtr4;
                    num1 = (byte) 9;
                    num2 = (byte) 12;
                    functionType = Depuncture.FunctionType.Equals;
                    break;
                  case Depuncture.PunctType.PUNCT_38_80:
                    numPtr6 = numPtr5;
                    num1 = (byte) 17;
                    num2 = (byte) 24;
                    functionType = Depuncture.FunctionType.Equals;
                    break;
                  default:
                    return;
                }
                for (uint index = 1; (long) index <= (long) sourceLength; ++index)
                {
                  uint num3;
                  switch (functionType)
                  {
                    case Depuncture.FunctionType.Func_292:
                      num3 = index + (index - 1U) / 65U;
                      break;
                    case Depuncture.FunctionType.Func_148:
                      num3 = index + (index - 1U) / 35U;
                      break;
                    default:
                      num3 = index;
                      break;
                  }
                  uint num4 = (uint) num2 * ((num3 - 1U) / (uint) num1) + (uint) numPtr6[num3 - (uint) num1 * ((num3 - 1U) / (uint) num1)];
                  dest[num4 - 1U] = source[index - 1U] == (byte) 0 ? (sbyte) -1 : (sbyte) 1;
                }
              }
    }

    public enum PunctType
    {
      PUNCT_2_3,
      PUNCT_1_3,
      PUNCT_292_432,
      PUNCT_148_432,
      PUNCT_112_168,
      PUNCT_72_162,
      PUNCT_38_80,
    }

    private enum FunctionType
    {
      Equals,
      Func_292,
      Func_148,
    }
  }
}
