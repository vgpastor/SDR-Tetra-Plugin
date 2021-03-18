using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDRSharp.Tetra
{
    unsafe class SdsParser
    {
        private readonly Rules[] _d_SdsDataRules = new Rules[]
        {
            new Rules(GlobalNames.Message_type, 4 ),
            new Rules(GlobalNames.Delivery_report_request, 2),
            new Rules(GlobalNames.Service_selection, 1 ),
            new Rules(GlobalNames.Storage_forward_control, 1),
            new Rules(GlobalNames.Message_reference, 8),
            new Rules(GlobalNames.Presence_bit, 0, RulesType.Jamp, (int)GlobalNames.Storage_forward_control, 0, 9),
            new Rules(GlobalNames.Validity_period, 5),
            new Rules(GlobalNames.Forward_address_type, 3),
            new Rules(GlobalNames.Forward_short_address, 8, RulesType.Switch, (int)GlobalNames.Forward_address_type, 0),
            new Rules(GlobalNames.Forward_address_SSI, 24, RulesType.Switch, (int)GlobalNames.Forward_address_type, 1),
            new Rules(GlobalNames.Forward_address_SSI, 24, RulesType.Switch, (int)GlobalNames.Forward_address_type, 2),
            new Rules(GlobalNames.Forward_address_extension, 24, RulesType.Switch, (int)GlobalNames.Forward_address_type, 2),
            new Rules(GlobalNames.Number_subscriber_number_digits, 8, RulesType.Switch, (int)GlobalNames.Forward_address_type, 3),
            new Rules(GlobalNames.External_subscriber_number_digit, 4, RulesType.Switch, (int)GlobalNames.Forward_address_type, 3),
            new Rules(GlobalNames.Dummy_digit, 4, RulesType.Switch, (int)GlobalNames.Forward_address_type, 3),
            new Rules(GlobalNames.User_data, 32)
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

        private readonly Rules[] _sds_SimpleTextRules = new Rules[]
        {
            new Rules(GlobalNames.Time_stamp_used, 1),
            new Rules(GlobalNames.Text_coding_scheme, 7),
            new Rules(GlobalNames.Timeframe_type, 2 ,RulesType.Switch, (int)GlobalNames.Time_stamp_used, 1),
            new Rules(GlobalNames.Reserved, 2 ,RulesType.Reserved),
            new Rules(GlobalNames.Month, 4 ,RulesType.Switch, (int)GlobalNames.Time_stamp_used, 1),
            new Rules(GlobalNames.Day, 5 ,RulesType.Switch, (int)GlobalNames.Time_stamp_used, 1),
            new Rules(GlobalNames.Hour, 5 ,RulesType.Switch, (int)GlobalNames.Time_stamp_used, 1),
            new Rules(GlobalNames.Minute, 6 ,RulesType.Switch, (int)GlobalNames.Time_stamp_used, 1),
            
        };

        public void ParseSDS(LogicChannel channelData, int offset, Dictionary<GlobalNames, int> result)
        {
            var length = 8;
            var type = (SdsProtocolIdent)TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
            offset += length;
            result.Add(GlobalNames.Protocol_identifier, (int)type);

            Debug.Write(" " + type.ToString());

            switch (type)
            {
                case SdsProtocolIdent.Simple_immediate_text:
                case SdsProtocolIdent.Immediate_text_messaging_TL:
                case SdsProtocolIdent.Text_Messaging_TL:
                case SdsProtocolIdent.Simple_text_msg:

                    offset = GlobalFunction.ParseParams(channelData, offset, _sds_SimpleTextRules, result);
                    ParseTextMessage(channelData, offset, result);
                    break;

                case SdsProtocolIdent.Location_System_TL:
                    ParseLocationInformation(channelData, offset, result);
                    break;

                case SdsProtocolIdent.Simple_location_system:
                    offset += 8;//Location system coding scheme
                    ParseLocationInformation(channelData, offset, result);
                    break;

                case SdsProtocolIdent.Location_information:
                    ParseLocationInformation(channelData, offset, result);                    
                    break;

                default:
                    GlobalFunction.ParseParams(channelData, offset, _d_SdsDataRules, result);
                    break;

            }
        }

        private void ParseLocationInformation(LogicChannel channelData, int offset, Dictionary<GlobalNames, int> result)
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
                    var pduSubType = TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
                    offset += length;
                    result.Add(GlobalNames.Location_PDU_type_extension, pduSubType);
                    switch (pduSubType)
                    {
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private unsafe void ParseTextMessage(LogicChannel channelData, int offset, Dictionary<GlobalNames, int> result)
        {
            var codingSheme = result[GlobalNames.Text_coding_scheme];

            while (offset < channelData.Length)
            {
                switch(codingSheme)
                {
                    default:
                        break;
                }

                offset++;
            }
        }

        private unsafe void ParseLongSDS(LogicChannel channelData, int offset, Dictionary<GlobalNames, int> result)
        {
            throw new NotImplementedException();
        }
    }
}
