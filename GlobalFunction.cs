// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.GlobalFunction
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using System;
using System.Collections.Generic;

namespace SDRSharp.Tetra
{
  internal static class GlobalFunction
  {
    public static bool IgnoreEncryptedSpeech;
    public static List<ReceivedData> NeighbourList = new List<ReceivedData>();
    private static int _currentBand;
    private static int _currentOffset;

    public static unsafe int ParseParams(
      LogicChannel channelData,
      int offset,
      Rules[] rules,
      ReceivedData result)
    {
      int num = 0;
      for (int index = 0; index < rules.Length; ++index)
      {
        if (offset >= channelData.Length)
        {
          if (!result.Contains(GlobalNames.OutOfBuffer))
            result.SetValue(GlobalNames.OutOfBuffer, 1);
          return offset;
        }
        if (num > 0)
        {
          --num;
        }
        else
        {
          Rules rule = rules[index];
          int int32 = TetraUtils.BitsToInt32(channelData.Ptr, offset, rule.Length);
          switch (rule.Type)
          {
            case RulesType.Direct:
              if (rule.GlobalName != GlobalNames.Reserved)
                result.SetValue(rule.GlobalName, int32);
              offset += rule.Length;
              continue;
            case RulesType.Options_bit:
              offset += rule.Length;
              if (int32 == 0)
                return offset;
              continue;
            case RulesType.Presence_bit:
              if (int32 == 0)
                num = rule.Ext1;
              offset += rule.Length;
              continue;
            case RulesType.More_bit:
              offset += rule.Length;
              return offset;
            case RulesType.Switch:
              if (result.Value((GlobalNames) rules[index].Ext1) == rules[index].Ext2)
              {
                if (rule.GlobalName != GlobalNames.Reserved)
                  result.SetValue(rule.GlobalName, int32);
                offset += rule.Length;
                continue;
              }
              continue;
            case RulesType.SwitchNot:
              if (result.Value((GlobalNames) rules[index].Ext1) != rules[index].Ext2)
              {
                if (rule.GlobalName != GlobalNames.Reserved)
                  result.SetValue(rule.GlobalName, int32);
                offset += rule.Length;
                continue;
              }
              continue;
            case RulesType.Reserved:
              offset += rule.Length;
              continue;
            case RulesType.Jamp:
              if (result.Value((GlobalNames) rules[index].Ext1) == rules[index].Ext2)
              {
                num = rule.Ext3;
                continue;
              }
              continue;
            case RulesType.JampNot:
              if (result.Value((GlobalNames) rules[index].Ext1) != rules[index].Ext2)
              {
                num = rule.Ext3;
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      return offset;
    }

    public static long FrequencyCalc(bool isFull, int carrier, int band = 0, int offset = 0)
    {
      if (isFull)
      {
        GlobalFunction._currentBand = band;
        GlobalFunction._currentOffset = offset;
      }
      long num = (long) GlobalFunction._currentBand * 100000000L + (long) carrier * 25000L;
      switch (GlobalFunction._currentOffset)
      {
        case 1:
          num += 6250L;
          break;
        case 2:
          num -= 6250L;
          break;
        case 3:
          num += 12500L;
          break;
      }
      return num;
    }

    public static int CarrierCalc(long frequency)
    {
      switch (GlobalFunction._currentOffset)
      {
        case 1:
          frequency -= 6250L;
          break;
        case 2:
          frequency += 6250L;
          break;
        case 3:
          frequency -= 12500L;
          break;
      }
      return (int) Math.Round((Decimal) (frequency - (long) GlobalFunction._currentBand * 100000000L) / 25000M);
    }
  }
}
