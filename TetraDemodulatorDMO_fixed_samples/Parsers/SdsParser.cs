using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SDRSharp.Tetra
{
    unsafe class SdsParser
    {
        private readonly Rules[] _d_Sds_TL_ForwardRules = new Rules[]
        {
            new Rules(GlobalNames.Delivery_report_request, 2),
            new Rules(GlobalNames.Service_selection, 1 ),
            new Rules(GlobalNames.Storage_forward_control, 1),
            new Rules(GlobalNames.Message_reference, 8),
            new Rules(GlobalNames.Reserved, 0, RulesType.Jamp, (int)GlobalNames.Storage_forward_control, 0, 99),
            new Rules(GlobalNames.Validity_period, 5),
            new Rules(GlobalNames.Forward_address_type, 3),
            new Rules(GlobalNames.Forward_short_address, 8, RulesType.Switch, (int)GlobalNames.Forward_address_type, 0),
            new Rules(GlobalNames.Forward_address_SSI, 24, RulesType.Switch, (int)GlobalNames.Forward_address_type, 1),
            new Rules(GlobalNames.Forward_address_SSI, 24, RulesType.Switch, (int)GlobalNames.Forward_address_type, 2),
            new Rules(GlobalNames.Forward_address_extension, 24, RulesType.Switch, (int)GlobalNames.Forward_address_type, 2),
            new Rules(GlobalNames.Reserved, 0, RulesType.JampNot, (int)GlobalNames.Forward_address_type, 3, 99),
            new Rules(GlobalNames.Number_subscriber_number_digits, 8),
            new Rules(GlobalNames.Reserved, 8, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 1),
            new Rules(GlobalNames.Reserved, 8, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 2),
            new Rules(GlobalNames.Reserved, 16, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 3),
            new Rules(GlobalNames.Reserved, 16, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 4),
            new Rules(GlobalNames.Reserved, 24, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 5),
            new Rules(GlobalNames.Reserved, 24, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 6),
            new Rules(GlobalNames.Reserved, 32, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 7),
            new Rules(GlobalNames.Reserved, 32, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 8),
            new Rules(GlobalNames.Reserved, 40, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 9),
            new Rules(GlobalNames.Reserved, 40, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 10),
            new Rules(GlobalNames.Reserved, 48, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 11),
            new Rules(GlobalNames.Reserved, 48, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 12),
            new Rules(GlobalNames.Reserved, 56, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 13),
            new Rules(GlobalNames.Reserved, 56, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 14),
            new Rules(GlobalNames.Reserved, 64, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 15),
            new Rules(GlobalNames.Reserved, 64, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 16),
            new Rules(GlobalNames.Reserved, 72, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 17),
            new Rules(GlobalNames.Reserved, 72, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 18),
            new Rules(GlobalNames.Reserved, 80, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 19),
            new Rules(GlobalNames.Reserved, 80, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 20),
            new Rules(GlobalNames.Reserved, 88, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 21),
            new Rules(GlobalNames.Reserved, 88, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 22),
            new Rules(GlobalNames.Reserved, 96, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 23),
            new Rules(GlobalNames.Reserved, 96, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 24),
        };

        private readonly Rules[] _d_Sds_TL_ReportRules = new Rules[]
        {
            new Rules(GlobalNames.Acknowledgement_required, 1), 
            new Rules(GlobalNames.Reserved, 2),
            new Rules(GlobalNames.Storage_forward_control, 1),
            new Rules(GlobalNames.Delivery_status, 8),
            new Rules(GlobalNames.Message_reference, 8),
            new Rules(GlobalNames.Presence_bit, 0, RulesType.Jamp, (int)GlobalNames.Storage_forward_control, 0, 99),
            new Rules(GlobalNames.Validity_period, 5),
            new Rules(GlobalNames.Forward_address_type, 3),
            new Rules(GlobalNames.Forward_short_address, 8, RulesType.Switch, (int)GlobalNames.Forward_address_type, 0),
            new Rules(GlobalNames.Forward_address_SSI, 24, RulesType.Switch, (int)GlobalNames.Forward_address_type, 1),
            new Rules(GlobalNames.Forward_address_SSI, 24, RulesType.Switch, (int)GlobalNames.Forward_address_type, 2),
            new Rules(GlobalNames.Forward_address_extension, 24, RulesType.Switch, (int)GlobalNames.Forward_address_type, 2),
            new Rules(GlobalNames.Reserved, 0, RulesType.JampNot, (int)GlobalNames.Forward_address_type, 3, 99),
            new Rules(GlobalNames.Number_subscriber_number_digits, 8),
            new Rules(GlobalNames.Reserved, 8, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 1),
            new Rules(GlobalNames.Reserved, 8, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 2),
            new Rules(GlobalNames.Reserved, 16, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 3),
            new Rules(GlobalNames.Reserved, 16, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 4),
            new Rules(GlobalNames.Reserved, 24, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 5),
            new Rules(GlobalNames.Reserved, 24, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 6),
            new Rules(GlobalNames.Reserved, 32, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 7),
            new Rules(GlobalNames.Reserved, 32, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 8),
            new Rules(GlobalNames.Reserved, 40, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 9),
            new Rules(GlobalNames.Reserved, 40, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 10),
            new Rules(GlobalNames.Reserved, 48, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 11),
            new Rules(GlobalNames.Reserved, 48, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 12),
            new Rules(GlobalNames.Reserved, 56, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 13),
            new Rules(GlobalNames.Reserved, 56, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 14),
            new Rules(GlobalNames.Reserved, 64, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 15),
            new Rules(GlobalNames.Reserved, 64, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 16),
            new Rules(GlobalNames.Reserved, 72, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 17),
            new Rules(GlobalNames.Reserved, 72, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 18),
            new Rules(GlobalNames.Reserved, 80, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 19),
            new Rules(GlobalNames.Reserved, 80, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 20),
            new Rules(GlobalNames.Reserved, 88, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 21),
            new Rules(GlobalNames.Reserved, 88, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 22),
            new Rules(GlobalNames.Reserved, 96, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 23),
            new Rules(GlobalNames.Reserved, 96, RulesType.Switch, (int)GlobalNames.Number_subscriber_number_digits, 24),
        };
       
        private readonly Rules[] _sds_LocationShortRules = new Rules[]
        {
            new Rules(GlobalNames.Time_elapsed, 2),
            new Rules(GlobalNames.Longitude, 25),
            new Rules(GlobalNames.Latitude, 24),
            new Rules(GlobalNames.Position_error, 3),
            new Rules(GlobalNames.Horizontal_velocity, 7),
            new Rules(GlobalNames.Direction_of_travel, 4),
            new Rules(GlobalNames.Options_bit, 1 , RulesType.Options_bit),
            new Rules(GlobalNames.Reason_for_sending, 8),
            new Rules(GlobalNames.User_defined_data, 8)
        };

        private readonly Rules[] _sds_ImmediateLocRepRules = new Rules[]
        {
            new Rules(GlobalNames.Request_response, 1),
            new Rules(GlobalNames.Report_type, 2),
            new Rules(GlobalNames.T5_el_ident, 5),
            new Rules(GlobalNames.T5_el_length, 6),
            new Rules(GlobalNames.T5_el_length_ex, 7, RulesType.Switch, (int)GlobalNames.T5_el_length, 0),
        };
        private readonly Rules[] _sds_SimpleTextRules = new Rules[]
        {
            new Rules(GlobalNames.Time_stamp_used, 1),
            new Rules(GlobalNames.Text_coding_scheme, 7),
            new Rules(GlobalNames.Timeframe_type, 2 ,RulesType.Switch, (int)GlobalNames.Time_stamp_used, 1),
            new Rules(GlobalNames.Reserved, 2, RulesType.Switch, (int)GlobalNames.Time_stamp_used, 1),
            new Rules(GlobalNames.Month, 4 ,RulesType.Switch, (int)GlobalNames.Time_stamp_used, 1),
            new Rules(GlobalNames.Day, 5 ,RulesType.Switch, (int)GlobalNames.Time_stamp_used, 1),
            new Rules(GlobalNames.Hour, 5 ,RulesType.Switch, (int)GlobalNames.Time_stamp_used, 1),
            new Rules(GlobalNames.Minute, 6 ,RulesType.Switch, (int)GlobalNames.Time_stamp_used, 1),
            
        };

        public int ParseTLService(LogicChannel channelData, int offset, ReceivedData result)
        {
            var length = 4;
            var messageType = TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
            offset += length;
            result.Add(GlobalNames.SDS_TL_MesaggeType, messageType);
            switch (messageType)
            {
                case 0://Forward
                    offset = GlobalFunction.ParseParams(channelData, offset, _d_Sds_TL_ForwardRules, result);
                    break;
                case 1://Report
                    offset = GlobalFunction.ParseParams(channelData, offset, _d_Sds_TL_ReportRules, result);
                    break;
                case 2:
                case 3:
                    // 
                    break;

            }

            return offset;
        }

        public void ParseSDS(LogicChannel channelData, int offset, ReceivedData result)
        {
            var length = 8;
            var type = (SdsProtocolIdent)TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
            offset += length;
            result.Add(GlobalNames.Protocol_identifier, (int)type);

            //Debug.Write(" " + type.ToString());

            if ((int)type > 127) offset = ParseTLService(channelData, offset, result);

            switch (type)
            {
                case SdsProtocolIdent.Simple_immediate_text:
                case SdsProtocolIdent.Simple_text_msg:

                    offset = GlobalFunction.ParseParams(channelData, offset, _sds_SimpleTextRules, result);
                    ParseTextMessage(channelData, offset, result);
                    break;

                case SdsProtocolIdent.Immediate_text_messaging_TL:
                case SdsProtocolIdent.Text_Messaging_TL:
                    offset = GlobalFunction.ParseParams(channelData, offset, _sds_SimpleTextRules, result);
                    ParseTextMessage(channelData, offset, result);
                    break;

                case SdsProtocolIdent.Location_System_TL:
                case SdsProtocolIdent.Simple_location_system:
                    length = 8;
                    var locationCodingSheme = TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
                    offset += length;
                    result.Add(GlobalNames.Location_System_Coding, locationCodingSheme);
                    if (locationCodingSheme == 0)
                    {
                        result.Add(GlobalNames.Text_coding_scheme, 1);
                        ParseTextMessage(channelData, offset, result);
                    }
                    else
                    {
                        result.Add(GlobalNames.UnknowData, 1);
                    }
                break;

                case SdsProtocolIdent.Location_information_protokol:
                    ParseLocationInformationProtokol(channelData, offset, result);
                    break;

                default:
                    offset = GlobalFunction.ParseParams(channelData, offset, _sds_SimpleTextRules, result);
                    ParseTextMessage(channelData, offset, result);
                    result.Add(GlobalNames.UnknowData, 1);
                    break;


            }
        }


        private void ParseLocationInformationProtokol(LogicChannel channelData, int offset, ReceivedData result)
        {
            var length = 2;
            var pduType = TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
            offset += length;
            result.Add(GlobalNames.Location_PDU_type, pduType);

            switch (pduType)
            {
                case 0://Short
                    GlobalFunction.ParseParams(channelData, offset, _sds_LocationShortRules, result);
                    break;

                case 1://Long
                    length = 4;
                    var pduSubType = (LocationTypeExtension)TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
                    offset += length;
                    result.Add(GlobalNames.Location_PDU_type_extension, (int)pduSubType);
                    switch (pduSubType)
                    {
                        case LocationTypeExtension.Immediate_location_report:
                            GlobalFunction.ParseParams(channelData, offset, _sds_ImmediateLocRepRules, result);
                            break;

                        case LocationTypeExtension.Long_location_report:
                        case LocationTypeExtension.Remove_trigger:
                        case LocationTypeExtension.Location_report_acknowledgement:
                        case LocationTypeExtension.Add_modify_trigger:
                        case LocationTypeExtension.Backlog:
                        case LocationTypeExtension.Basic_location_parameters:
                        case LocationTypeExtension.Location_reporting_enable_disable:
                        case LocationTypeExtension.Location_reporting_temporary_control:
                        case LocationTypeExtension.Report_basic_location_parameters:
                        case LocationTypeExtension.Report_trigger:
                        default:
                            result.Add(GlobalNames.UnknowData, 1);
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private unsafe void ParseTextMessage(LogicChannel channelData, int offset, ReceivedData result)
        {
            if (result.Contains(GlobalNames.OutOfBuffer)) return;

            var codingSheme = result.Value(GlobalNames.Text_coding_scheme);

            Encoding encTable = Encoding.GetEncoding("iso-8859-1");
            int symbLength = 8;
            
            switch (codingSheme)
            {
                case 1: 
                    encTable = Encoding.GetEncoding("iso-8859-1");
                    break;
                case 2:
                    encTable = Encoding.GetEncoding("iso-8859-2");
                    break;
                case 3:
                    encTable = Encoding.GetEncoding("iso-8859-3");
                    break;
                case 4:
                    encTable = Encoding.GetEncoding("iso-8859-4");
                    break;
                case 5:
                    encTable = Encoding.GetEncoding("iso-8859-5");
                    break;
                case 6:
                    encTable = Encoding.GetEncoding("iso-8859-6");
                    break;
                case 7:
                    encTable = Encoding.GetEncoding("iso-8859-7");
                    break;
                case 8:
                    encTable = Encoding.GetEncoding("iso-8859-8");
                    break;
                case 9:
                    encTable = Encoding.GetEncoding("iso-8859-9");
                    break;
                case 10:
                    encTable = Encoding.GetEncoding("iso-8859-10");
                    break;
                case 11:
                    encTable = Encoding.GetEncoding("iso-8859-13");
                    break;
                case 12:
                    encTable = Encoding.GetEncoding("iso-8859-14");
                    break;
                case 13:
                    encTable = Encoding.GetEncoding("iso-8859-15");
                    break;
                case 14:
                    encTable = Encoding.GetEncoding(437);
                    break;
                case 15:
                    encTable = Encoding.GetEncoding(737);
                    break;
                case 16:
                    encTable = Encoding.GetEncoding(850);
                    break;
                case 17:
                    encTable = Encoding.GetEncoding(852);
                    break;
                case 18:
                    encTable = Encoding.GetEncoding(855);
                    break;
                case 19:
                    encTable = Encoding.GetEncoding(857);
                    break;
                case 20:
                    encTable = Encoding.GetEncoding(860);
                    break;
                case 21:
                    encTable = Encoding.GetEncoding(861);
                    break;
                case 22:
                    encTable = Encoding.GetEncoding(863);
                    break;
                case 23:
                    encTable = Encoding.GetEncoding(865);
                    break;
                case 24:
                    encTable = Encoding.GetEncoding(866);
                    break;
                case 25:
                    encTable = Encoding.GetEncoding(869);
                    break;
                
                default:
                    break;
                    result.Add(GlobalNames.UnknowData, 1);
                    return;
            }

            Decoder dec = encTable.GetDecoder();
            var messageLength = (channelData.Length - offset) / symbLength;

            if (messageLength < 0) return;

            string message;
            byte[] symbolsArray = new byte[messageLength];
            int index = 0;

            while (messageLength > 0)
            {
                symbolsArray[index++] = TetraUtils.BitsToByte(channelData.Ptr, offset, symbLength);
                offset += symbLength;
                messageLength--;
            }

            message = encTable.GetString(symbolsArray);
           
            //Debug.Write(" data:" + message);
        }

        private unsafe void ParseLongSDS(LogicChannel channelData, int offset, ReceivedData result)
        {
            throw new NotImplementedException();
        }
    }
}
