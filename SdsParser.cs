// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.SdsParser
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using System;
using System.Linq;
using System.Text;

namespace SDRSharp.Tetra
{
    internal class SdsParser
    {
        private readonly Rules[] _d_Sds_TL_ForwardRules = new Rules[37]
        {
      new Rules(GlobalNames.Delivery_report_request, 2),
      new Rules(GlobalNames.Service_selection, 1),
      new Rules(GlobalNames.Storage_forward_control, 1),
      new Rules(GlobalNames.Message_reference, 8),
      new Rules(GlobalNames.Reserved, 0, RulesType.Jamp, 156, ext3: 99),
      new Rules(GlobalNames.Validity_period, 5),
      new Rules(GlobalNames.Forward_address_type, 3),
      new Rules(GlobalNames.Forward_short_address, 8, RulesType.Switch, 159),
      new Rules(GlobalNames.Forward_address_SSI, 24, RulesType.Switch, 159, 1),
      new Rules(GlobalNames.Forward_address_SSI, 24, RulesType.Switch, 159, 2),
      new Rules(GlobalNames.Forward_address_extension, 24, RulesType.Switch, 159, 2),
      new Rules(GlobalNames.Reserved, 0, RulesType.JampNot, 159, 3, 99),
      new Rules(GlobalNames.Number_subscriber_number_digits, 8),
      new Rules(GlobalNames.Reserved, 8, RulesType.Switch, 163, 1),
      new Rules(GlobalNames.Reserved, 8, RulesType.Switch, 163, 2),
      new Rules(GlobalNames.Reserved, 16, RulesType.Switch, 163, 3),
      new Rules(GlobalNames.Reserved, 16, RulesType.Switch, 163, 4),
      new Rules(GlobalNames.Reserved, 24, RulesType.Switch, 163, 5),
      new Rules(GlobalNames.Reserved, 24, RulesType.Switch, 163, 6),
      new Rules(GlobalNames.Reserved, 32, RulesType.Switch, 163, 7),
      new Rules(GlobalNames.Reserved, 32, RulesType.Switch, 163, 8),
      new Rules(GlobalNames.Reserved, 40, RulesType.Switch, 163, 9),
      new Rules(GlobalNames.Reserved, 40, RulesType.Switch, 163, 10),
      new Rules(GlobalNames.Reserved, 48, RulesType.Switch, 163, 11),
      new Rules(GlobalNames.Reserved, 48, RulesType.Switch, 163, 12),
      new Rules(GlobalNames.Reserved, 56, RulesType.Switch, 163, 13),
      new Rules(GlobalNames.Reserved, 56, RulesType.Switch, 163, 14),
      new Rules(GlobalNames.Reserved, 64, RulesType.Switch, 163, 15),
      new Rules(GlobalNames.Reserved, 64, RulesType.Switch, 163, 16),
      new Rules(GlobalNames.Reserved, 72, RulesType.Switch, 163, 17),
      new Rules(GlobalNames.Reserved, 72, RulesType.Switch, 163, 18),
      new Rules(GlobalNames.Reserved, 80, RulesType.Switch, 163, 19),
      new Rules(GlobalNames.Reserved, 80, RulesType.Switch, 163, 20),
      new Rules(GlobalNames.Reserved, 88, RulesType.Switch, 163, 21),
      new Rules(GlobalNames.Reserved, 88, RulesType.Switch, 163, 22),
      new Rules(GlobalNames.Reserved, 96, RulesType.Switch, 163, 23),
      new Rules(GlobalNames.Reserved, 96, RulesType.Switch, 163, 24)
        };
        private readonly Rules[] _d_Sds_TL_ReportRules = new Rules[38]
        {
      new Rules(GlobalNames.Acknowledgement_required, 1),
      new Rules(GlobalNames.Reserved, 2),
      new Rules(GlobalNames.Storage_forward_control, 1),
      new Rules(GlobalNames.Delivery_status, 8),
      new Rules(GlobalNames.Message_reference, 8),
      new Rules(GlobalNames.Presence_bit, 0, RulesType.Jamp, 156, ext3: 99),
      new Rules(GlobalNames.Validity_period, 5),
      new Rules(GlobalNames.Forward_address_type, 3),
      new Rules(GlobalNames.Forward_short_address, 8, RulesType.Switch, 159),
      new Rules(GlobalNames.Forward_address_SSI, 24, RulesType.Switch, 159, 1),
      new Rules(GlobalNames.Forward_address_SSI, 24, RulesType.Switch, 159, 2),
      new Rules(GlobalNames.Forward_address_extension, 24, RulesType.Switch, 159, 2),
      new Rules(GlobalNames.Reserved, 0, RulesType.JampNot, 159, 3, 99),
      new Rules(GlobalNames.Number_subscriber_number_digits, 8),
      new Rules(GlobalNames.Reserved, 8, RulesType.Switch, 163, 1),
      new Rules(GlobalNames.Reserved, 8, RulesType.Switch, 163, 2),
      new Rules(GlobalNames.Reserved, 16, RulesType.Switch, 163, 3),
      new Rules(GlobalNames.Reserved, 16, RulesType.Switch, 163, 4),
      new Rules(GlobalNames.Reserved, 24, RulesType.Switch, 163, 5),
      new Rules(GlobalNames.Reserved, 24, RulesType.Switch, 163, 6),
      new Rules(GlobalNames.Reserved, 32, RulesType.Switch, 163, 7),
      new Rules(GlobalNames.Reserved, 32, RulesType.Switch, 163, 8),
      new Rules(GlobalNames.Reserved, 40, RulesType.Switch, 163, 9),
      new Rules(GlobalNames.Reserved, 40, RulesType.Switch, 163, 10),
      new Rules(GlobalNames.Reserved, 48, RulesType.Switch, 163, 11),
      new Rules(GlobalNames.Reserved, 48, RulesType.Switch, 163, 12),
      new Rules(GlobalNames.Reserved, 56, RulesType.Switch, 163, 13),
      new Rules(GlobalNames.Reserved, 56, RulesType.Switch, 163, 14),
      new Rules(GlobalNames.Reserved, 64, RulesType.Switch, 163, 15),
      new Rules(GlobalNames.Reserved, 64, RulesType.Switch, 163, 16),
      new Rules(GlobalNames.Reserved, 72, RulesType.Switch, 163, 17),
      new Rules(GlobalNames.Reserved, 72, RulesType.Switch, 163, 18),
      new Rules(GlobalNames.Reserved, 80, RulesType.Switch, 163, 19),
      new Rules(GlobalNames.Reserved, 80, RulesType.Switch, 163, 20),
      new Rules(GlobalNames.Reserved, 88, RulesType.Switch, 163, 21),
      new Rules(GlobalNames.Reserved, 88, RulesType.Switch, 163, 22),
      new Rules(GlobalNames.Reserved, 96, RulesType.Switch, 163, 23),
      new Rules(GlobalNames.Reserved, 96, RulesType.Switch, 163, 24)
        };
        private readonly Rules[] _sds_LocationShortRules = new Rules[9]
        {
      new Rules(GlobalNames.Time_elapsed, 2),
      new Rules(GlobalNames.Longitude, 25),
      new Rules(GlobalNames.Latitude, 24),
      new Rules(GlobalNames.Position_error, 3),
      new Rules(GlobalNames.Horizontal_velocity, 7),
      new Rules(GlobalNames.Direction_of_travel, 4),
      new Rules(GlobalNames.Options_bit, 1, RulesType.Options_bit),
      new Rules(GlobalNames.Reason_for_sending, 8),
      new Rules(GlobalNames.User_defined_data, 8)
        };
        private readonly Rules[] _sds_ImmediateLocRepRules = new Rules[5]
        {
      new Rules(GlobalNames.Request_response, 1),
      new Rules(GlobalNames.Report_type, 2),
      new Rules(GlobalNames.T5_el_ident, 5),
      new Rules(GlobalNames.T5_el_length, 6),
      new Rules(GlobalNames.T5_el_length_ex, 7, RulesType.Switch, 278)
        };
        private readonly Rules[] _sds_SimpleTextRules = new Rules[8]
        {
      new Rules(GlobalNames.Time_stamp_used, 1),
      new Rules(GlobalNames.Text_coding_scheme, 7),
      new Rules(GlobalNames.Timeframe_type, 2, RulesType.Switch, 198, 1),
      new Rules(GlobalNames.Reserved, 2, RulesType.Switch, 198, 1),
      new Rules(GlobalNames.Month, 4, RulesType.Switch, 198, 1),
      new Rules(GlobalNames.Day, 5, RulesType.Switch, 198, 1),
      new Rules(GlobalNames.Hour, 5, RulesType.Switch, 198, 1),
      new Rules(GlobalNames.Minute, 6, RulesType.Switch, 198, 1)
        };

        public unsafe int ParseTLService(LogicChannel channelData, int offset, ReceivedData result)
        {
            int length = 4;
            int int32 = TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
            offset += length;
            result.Add(GlobalNames.SDS_TL_MesaggeType, int32);
            switch (int32)
            {
                case 0:
                    offset = GlobalFunction.ParseParams(channelData, offset, this._d_Sds_TL_ForwardRules, result);
                    break;
                case 1:
                    offset = GlobalFunction.ParseParams(channelData, offset, this._d_Sds_TL_ReportRules, result);
                    break;
            }
            return offset;
        }

        public unsafe void ParseSDS(LogicChannel channelData, int offset, ReceivedData result)
        {
            int length1 = 8;
            SdsProtocolIdent SdsProtocol = (SdsProtocolIdent)TetraUtils.BitsToInt32(channelData.Ptr, offset, length1);

            offset += length1;
            result.Add(GlobalNames.Protocol_identifier, (int)SdsProtocol);
            string str;

            if (SdsProtocol > SdsProtocolIdent.Reserved_127)
            {
                offset = this.ParseTLService(channelData, offset, result);
            }
            if (SdsProtocol <= SdsProtocolIdent.Simple_immediate_text)
            {
                if (SdsProtocol != SdsProtocolIdent.Simple_text_msg)
                {
                    if (SdsProtocol != SdsProtocolIdent.Simple_location_system)
                    {
                        if (SdsProtocol != SdsProtocolIdent.Simple_immediate_text)
                        {
                            offset = GlobalFunction.ParseParams(channelData, offset, this._sds_SimpleTextRules, result);
                            str = this.ParseTextMessage(channelData, offset, result);
                            result.Add(GlobalNames.UnknowData, 1);
                            TetraPlugin.Logger("SDS_PROTOCOLO" + SdsProtocol + " -> " + str);
                        }
                    }
                    else
                    {
                        int length2 = 8;
                        int int32_2 = TetraUtils.BitsToInt32(channelData.Ptr, offset, length2);
                        offset += length2;
                        result.Add(GlobalNames.Location_System_Coding, int32_2);
                        if (int32_2 == 0)
                        {
                            result.Add(GlobalNames.Text_coding_scheme, 1);
                            str = this.ParseTextMessage(channelData, offset, result);
                            TetraPlugin.Logger("SDS_PROTOCOLO" + SdsProtocol + " -> " + str);
                            return;
                        }
                        result.Add(GlobalNames.UnknowData, 1);
                        return;
                    }
                }
                offset = GlobalFunction.ParseParams(channelData, offset, this._sds_SimpleTextRules, result);
                str = this.ParseTextMessage(channelData, offset, result);
                TetraPlugin.Logger("SDS_PROTOCOLO" + SdsProtocol + " -> " + str);
                return;
            }
            if (SdsProtocol <= SdsProtocolIdent.Text_Messaging_TL)
            {
                if (SdsProtocol != SdsProtocolIdent.Location_information_protokol)
                {
                    if (SdsProtocol != SdsProtocolIdent.Text_Messaging_TL)
                    {
                        offset = GlobalFunction.ParseParams(channelData, offset, this._sds_SimpleTextRules, result);
                        str = this.ParseTextMessage(channelData, offset, result);
                        TetraPlugin.Logger("SDS_PROTOCOLO" + SdsProtocol + " -> " + str);
                        result.Add(GlobalNames.UnknowData, 1);
                    }
                }
                else
                {
                    this.ParseLocationInformationProtokol(channelData, offset, result);
                    return;
                }
            }
            else if (SdsProtocol != SdsProtocolIdent.Location_System_TL)
            {
                if (SdsProtocol != SdsProtocolIdent.Immediate_text_messaging_TL)
                {
                    offset = GlobalFunction.ParseParams(channelData, offset, this._sds_SimpleTextRules, result);
                    str = this.ParseTextMessage(channelData, offset, result);
                    TetraPlugin.Logger("SDS_PROTOCOLO" + SdsProtocol + " -> " + str);
                    result.Add(GlobalNames.UnknowData, 1);
                }
            }
            else
            {
                int length2 = 8;
                int int32_2 = TetraUtils.BitsToInt32(channelData.Ptr, offset, length2);
                offset += length2;
                result.Add(GlobalNames.Location_System_Coding, int32_2);
                if (int32_2 == 0)
                {
                    result.Add(GlobalNames.Text_coding_scheme, 1);
                    str = this.ParseTextMessage(channelData, offset, result);
                    TetraPlugin.Logger("SDS_PROTOCOLO" + SdsProtocol + " -> " + str);
                    return;
                }
                result.Add(GlobalNames.UnknowData, 1);
                return;
            }
            offset = GlobalFunction.ParseParams(channelData, offset, this._sds_SimpleTextRules, result);
            str = this.ParseTextMessage(channelData, offset, result);
            TetraPlugin.Logger("SDS_PROTOCOLO" + SdsProtocol + " -> " + str);
            return;
        }

        private unsafe void ParseLocationInformationProtokol(
          LogicChannel channelData,
          int offset,
          ReceivedData result)
        {
            int length1 = 2;
            int int32_1 = TetraUtils.BitsToInt32(channelData.Ptr, offset, length1);
            offset += length1;
            result.Add(GlobalNames.Location_PDU_type, int32_1);
            switch (int32_1)
            {
                case 0:
                    GlobalFunction.ParseParams(channelData, offset, this._sds_LocationShortRules, result);
                    break;
                case 1:
                    int length2 = 4;
                    LocationTypeExtension int32_2 = (LocationTypeExtension)TetraUtils.BitsToInt32(channelData.Ptr, offset, length2);
                    offset += length2;
                    result.Add(GlobalNames.Location_PDU_type_extension, (int)int32_2);
                    switch (int32_2)
                    {
                        case LocationTypeExtension.Immediate_location_report:
                            GlobalFunction.ParseParams(channelData, offset, this._sds_ImmediateLocRepRules, result);
                            return;
                        default:
                            result.Add(GlobalNames.UnknowData, 1);
                            return;
                    }
            }
        }

        private unsafe string ParseTextMessage(LogicChannel channelData, int offset, ReceivedData result)
        {
            if (result.Contains(GlobalNames.OutOfBuffer))
                return null;
            int textEncoding = result.Value(GlobalNames.Text_coding_scheme);
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            int length1 = 8;
            //switch (textEncoding)
            //{
            //    case 1:
            //        encoding = Encoding.GetEncoding("iso-8859-1");
            //        break;
            //    case 2:
            //        encoding = Encoding.GetEncoding("iso-8859-2");
            //        break;
            //    case 3:
            //        encoding = Encoding.GetEncoding("iso-8859-3");
            //        break;
            //    case 4:
            //        encoding = Encoding.GetEncoding("iso-8859-4");
            //        break;
            //    case 5:
            //        encoding = Encoding.GetEncoding("iso-8859-5");
            //        break;
            //    case 6:
            //        encoding = Encoding.GetEncoding("iso-8859-6");
            //        break;
            //    case 7:
            //        encoding = Encoding.GetEncoding("iso-8859-7");
            //        break;
            //    case 8:
            //        encoding = Encoding.GetEncoding("iso-8859-8");
            //        break;
            //    case 9:
            //        encoding = Encoding.GetEncoding("iso-8859-9");
            //        break;
            //    case 10:
            //        encoding = Encoding.GetEncoding("iso-8859-10");
            //        break;
            //    case 11:
            //        encoding = Encoding.GetEncoding("iso-8859-13");
            //        break;
            //    case 12:
            //        encoding = Encoding.GetEncoding("iso-8859-14");
            //        break;
            //    case 13:
            //        encoding = Encoding.GetEncoding("iso-8859-15");
            //        break;
            //    case 14:
            //        encoding = Encoding.GetEncoding(437);
            //        break;
            //    case 15:
            //        encoding = Encoding.GetEncoding(737);
            //        break;
            //    case 16:
            //        encoding = Encoding.GetEncoding(850);
            //        break;
            //    case 17:
            //        encoding = Encoding.GetEncoding(852);
            //        break;
            //    case 18:
            //        encoding = Encoding.GetEncoding(855);
            //        break;
            //    case 19:
            //        encoding = Encoding.GetEncoding(857);
            //        break;
            //    case 20:
            //        encoding = Encoding.GetEncoding(860);
            //        break;
            //    case 21:
            //        encoding = Encoding.GetEncoding(861);
            //        break;
            //    case 22:
            //        encoding = Encoding.GetEncoding(863);
            //        break;
            //    case 23:
            //        encoding = Encoding.GetEncoding(865);
            //        break;
            //    case 24:
            //        encoding = Encoding.GetEncoding(866);
            //        break;
            //    case 25:
            //        encoding = Encoding.GetEncoding(869);
            //        break;
            //}
            encoding.GetDecoder();
            int length2 = (channelData.Length - offset) / length1;

            TetraPlugin.Logger(channelData.Ptr, offset, length2*length1);

            if (length2 < 0)
                return null;
            byte[] bytes = new byte[length2];
            int num2 = 0;
            for (; length2 > 0; --length2)
            {
                bytes[num2++] = TetraUtils.BitsToByte(channelData.Ptr, offset, length1);
                offset += length1;
            }
            string bitString = BitConverter.ToString(bytes).Replace("-", string.Empty);
            TetraPlugin.Logger("Origin " + bitString);
            return encoding.GetString(bytes);
        }

        private void ParseLongSDS(LogicChannel channelData, int offset, ReceivedData result) => throw new NotImplementedException();
    }
}
