// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.MleLevel
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
    internal class MleLevel
    {
        private SdsParser _sds = new SdsParser();
        private readonly Rules[] _d_ReleaseRules = new Rules[6]
        {
      new Rules(GlobalNames.Call_identifier, 14),
      new Rules(GlobalNames.Disconnect_cause, 5),
      new Rules(GlobalNames.Options_bit, 1, RulesType.Options_bit),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Notification_indicator, 6),
      new Rules(GlobalNames.More_bit, 1, RulesType.More_bit)
        };
        private readonly Rules[] _d_SdsDataRules = new Rules[9]
        {
      new Rules(GlobalNames.Calling_party_type_identifier, 2),
      new Rules(GlobalNames.Calling_party_address_SSI, 24),
      new Rules(GlobalNames.Calling_party_extension, 24, RulesType.Switch, 19, 2),
      new Rules(GlobalNames.Short_data_type_identifier, 2),
      new Rules(GlobalNames.User_Defined_Data_16, 16, RulesType.Switch, 137),
      new Rules(GlobalNames.User_Defined_Data_32, 32, RulesType.Switch, 137, 1),
      new Rules(GlobalNames.User_Defined_Data_64_1, 32, RulesType.Switch, 137, 2),
      new Rules(GlobalNames.User_Defined_Data_64_2, 32, RulesType.Switch, 137, 2),
      new Rules(GlobalNames.User_Defined_Data4_Length, 11, RulesType.Switch, 137, 3)
        };
        private readonly Rules[] _d_TX_CeasedRules = new Rules[6]
        {
      new Rules(GlobalNames.Call_identifier, 14),
      new Rules(GlobalNames.Transmission_request_permission, 1),
      new Rules(GlobalNames.Options_bit, 1, RulesType.Options_bit),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Notification_indicator, 6),
      new Rules(GlobalNames.More_bit, 1, RulesType.More_bit)
        };
        private readonly Rules[] _d_TX_GrantedRules = new Rules[13]
        {
      new Rules(GlobalNames.Call_identifier, 14),
      new Rules(GlobalNames.Transmission_grant, 2),
      new Rules(GlobalNames.Transmission_request_permission, 1),
      new Rules(GlobalNames.Encryption_control, 1),
      new Rules(GlobalNames.Reserved, 1),
      new Rules(GlobalNames.Options_bit, 1, RulesType.Options_bit),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Notification_indicator, 6),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 3),
      new Rules(GlobalNames.Transmitting_party_type_identifier, 2),
      new Rules(GlobalNames.Transmitting_party_address_SSI, 24),
      new Rules(GlobalNames.Transmitting_party_extension, 24, RulesType.Switch, 11, 2),
      new Rules(GlobalNames.More_bit, 1, RulesType.More_bit)
        };
        private readonly Rules[] _d_setupRules = new Rules[22]
        {
      new Rules(GlobalNames.Call_identifier, 14),
      new Rules(GlobalNames.Call_time_out, 4),
      new Rules(GlobalNames.Hook_method, 1),
      new Rules(GlobalNames.Simplex_duplex, 1),
      new Rules(GlobalNames.Basic_service_Circuit_mode_type, 3),
      new Rules(GlobalNames.Basic_service_Encryption_flag, 1),
      new Rules(GlobalNames.Basic_service_Communication_type, 2),
      new Rules(GlobalNames.Basic_service_Slots_per_frame, 2, RulesType.SwitchNot, 132),
      new Rules(GlobalNames.Basic_service_Speech_service, 2, RulesType.Switch, 132),
      new Rules(GlobalNames.Transmission_grant, 2),
      new Rules(GlobalNames.Transmission_request_permission, 1),
      new Rules(GlobalNames.Call_priority, 4),
      new Rules(GlobalNames.Options_bit, 1, RulesType.Options_bit),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Notification_indicator, 6),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Temporary_address, 24),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 3),
      new Rules(GlobalNames.Calling_party_type_identifier, 2),
      new Rules(GlobalNames.Calling_party_address_SSI, 24),
      new Rules(GlobalNames.Calling_party_extension, 24, RulesType.Switch, 19, 2),
      new Rules(GlobalNames.More_bit, 1, RulesType.More_bit)
        };
        private readonly Rules[] _d_infoRules = new Rules[25]
        {
      new Rules(GlobalNames.Call_identifier, 14),
      new Rules(GlobalNames.Reset_Call_time_out, 1),
      new Rules(GlobalNames.Poll_request, 1),
      new Rules(GlobalNames.Options_bit, 1, RulesType.Options_bit),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.New_Call_Identifier, 14),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Call_time_out, 4),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Call_time_out_setup_phase, 3),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Call_ownership, 1),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Modify, 9),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Call_status, 3),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Temporary_address, 24),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Notification_indicator, 6),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Poll_response_percentage, 6),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Poll_response_number, 6),
      new Rules(GlobalNames.More_bit, 1, RulesType.More_bit)
        };
        private readonly Rules[] _d_connectRules = new Rules[21]
        {
      new Rules(GlobalNames.Call_identifier, 14),
      new Rules(GlobalNames.Call_time_out, 4),
      new Rules(GlobalNames.Hook_method, 1),
      new Rules(GlobalNames.Simplex_duplex, 1),
      new Rules(GlobalNames.Transmission_grant, 2),
      new Rules(GlobalNames.Transmission_request_permission, 1),
      new Rules(GlobalNames.Call_ownership, 1),
      new Rules(GlobalNames.Options_bit, 1, RulesType.Options_bit),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Call_priority, 4),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 5),
      new Rules(GlobalNames.Basic_service_Circuit_mode_type, 3),
      new Rules(GlobalNames.Basic_service_Encryption_flag, 1),
      new Rules(GlobalNames.Basic_service_Communication_type, 2),
      new Rules(GlobalNames.Basic_service_Slots_per_frame, 2, RulesType.SwitchNot, 132),
      new Rules(GlobalNames.Basic_service_Speech_service, 2, RulesType.Switch, 132),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Temporary_address, 24),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Notification_indicator, 6),
      new Rules(GlobalNames.More_bit, 1, RulesType.More_bit)
        };
        private readonly Rules[] _d_Nwrk_BroadcastRules = new Rules[9]
        {
      new Rules(GlobalNames.Cell_reselect_parameters, 16),
      new Rules(GlobalNames.Cell_service_level, 2),
      new Rules(GlobalNames.Options_bit, 1, RulesType.Options_bit),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 3),
      new Rules(GlobalNames.Network_time, 24),
      new Rules(GlobalNames.Local_time_offset_sign, 1),
      new Rules(GlobalNames.Local_time_offset, 23),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Number_of_Neighbour_cells_element, 3)
        };
        private readonly Rules[] _neighbour_Cell_InfoRules = new Rules[37]
        {
      new Rules(GlobalNames.Cell_identifier, 5),
      new Rules(GlobalNames.Cell_reselection_types_supported, 2),
      new Rules(GlobalNames.Neighbour_cell_synchronised, 1),
      new Rules(GlobalNames.Neighbour_cell_service_level, 2),
      new Rules(GlobalNames.Main_carrier_number, 12),
      new Rules(GlobalNames.Options_bit, 1, RulesType.Options_bit),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Main_carrier_number_extension, 10),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Neighbour_MCC, 10),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Neighbour_MNC, 14),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Neighbour_LA, 14),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Maximum_MS_transmit_power, 3),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Minimum_RX_access_level, 4),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Subscriber_class, 16),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 12),
      new Rules(GlobalNames.Registration_required, 1),
      new Rules(GlobalNames.De_registration_required, 1),
      new Rules(GlobalNames.Priority_cell, 1),
      new Rules(GlobalNames.Cell_never_uses_minimum_mode, 1),
      new Rules(GlobalNames.Migration_supported, 1),
      new Rules(GlobalNames.System_wide_services, 1),
      new Rules(GlobalNames.TETRA_voice_service, 1),
      new Rules(GlobalNames.Circuit_mode_data_service, 1),
      new Rules(GlobalNames.Reserved, 1),
      new Rules(GlobalNames.SNDCP_Service, 1),
      new Rules(GlobalNames.Air_interface_encryption, 1),
      new Rules(GlobalNames.Advanced_link_supported, 1),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Timeshare_cell_and_AI_encryption, 5),
      new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.TDMA_frame_offset, 6)
        };

        public unsafe void Parse(LogicChannel channelData, int offset, ReceivedData result)
        {
            MLEPduType int32 = (MLEPduType)TetraUtils.BitsToInt32(channelData.Ptr, offset, 3);
            offset += 3;
            result.Add(GlobalNames.MLE_PDU_Type, (int)int32);
            if (int32 != MLEPduType.CMCE)
            {
                if (int32 == MLEPduType.MLE)
                    this.ParseMLEPDU(channelData, offset, result);
                else
                    result.Add(GlobalNames.UnknowData, 1);
            }
            else
                this.ParseCMCEPDU(channelData, offset, result);
        }

        private unsafe void ParseCMCEPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            int length = 5;
            CmcePrimitivesType cmceType = (CmcePrimitivesType)TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
            offset += length;
            result.Add(GlobalNames.CMCE_Primitives_Type, (int)cmceType);
            switch (cmceType)
            {
                case CmcePrimitivesType.D_Alert:
                case CmcePrimitivesType.D_Call_Proceeding:
                case CmcePrimitivesType.D_Status:
                case CmcePrimitivesType.D_TX_Continue:
                case CmcePrimitivesType.D_TX_Wait:
                case CmcePrimitivesType.D_TX_Interrupt:
                case CmcePrimitivesType.D_Call_Restore:
                case CmcePrimitivesType.D_Facility:
                case CmcePrimitivesType.Reserved17:
                case CmcePrimitivesType.Reserved18:
                case CmcePrimitivesType.Reserved19:
                case CmcePrimitivesType.Reserved20:
                case CmcePrimitivesType.Reserved21:
                case CmcePrimitivesType.Reserved22:
                case CmcePrimitivesType.Reserved23:
                case CmcePrimitivesType.Reserved24:
                case CmcePrimitivesType.Reserved25:
                case CmcePrimitivesType.Reserved26:
                case CmcePrimitivesType.Reserved27:
                case CmcePrimitivesType.Reserved28:
                case CmcePrimitivesType.Reserved29:
                case CmcePrimitivesType.Reserved30:
                case CmcePrimitivesType.CMCE_Function_Not_Supported:
                    result.Add(GlobalNames.UnknowData, 1);
                    break;
                case CmcePrimitivesType.D_Connect:
                    GlobalFunction.ParseParams(channelData, offset, this._d_connectRules, result);
                    break;
                case CmcePrimitivesType.D_Info:
                    GlobalFunction.ParseParams(channelData, offset, this._d_infoRules, result);
                    break;
                case CmcePrimitivesType.D_Release:
                    GlobalFunction.ParseParams(channelData, offset, this._d_ReleaseRules, result);
                    break;
                case CmcePrimitivesType.D_Setup:
                    GlobalFunction.ParseParams(channelData, offset, this._d_setupRules, result);
                    break;
                case CmcePrimitivesType.D_TX_Ceased:
                    GlobalFunction.ParseParams(channelData, offset, this._d_TX_CeasedRules, result);
                    break;
                case CmcePrimitivesType.D_TX_Granted:
                    GlobalFunction.ParseParams(channelData, offset, this._d_TX_GrantedRules, result);
                    break;
                case CmcePrimitivesType.D_SDS_Data:
                    offset = GlobalFunction.ParseParams(channelData, offset, this._d_SdsDataRules, result);
                    if (!result.Contains(GlobalNames.User_Defined_Data4_Length))
                        break;
                    this._sds.ParseSDS(new LogicChannel()
                    {
                        Ptr = channelData.Ptr + offset,
                        Length = result.Value(GlobalNames.User_Defined_Data4_Length)
                    }, 0, result);
                    break;
                default:
                    TetraPlugin.Logger("Unknow CMCE " + cmceType);
                    break;
            }
        }

        private unsafe void ParseMLEPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            int length = 3;
            MlePrimitivesType int32 = (MlePrimitivesType)TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
            offset += length;
            result.Add(GlobalNames.MLE_Primitives_Type, (int)int32);
            switch (int32)
            {
                case MlePrimitivesType.D_NWRK_BROADCAST:
                    offset = GlobalFunction.ParseParams(channelData, offset, this._d_Nwrk_BroadcastRules, result);
                    int num1 = 0;
                    if (!result.TryGetValue(GlobalNames.Number_of_Neighbour_cells_element, ref num1))
                        break;
                    for (int index = 0; index < num1; ++index)
                    {
                        ReceivedData result1 = new ReceivedData();
                        offset = GlobalFunction.ParseParams(channelData, offset, this._neighbour_Cell_InfoRules, result1);
                        int num2 = 0;
                        if (result1.TryGetValue(GlobalNames.Cell_identifier, ref num2) && GlobalFunction.NeighbourList.Count < 32)
                            GlobalFunction.NeighbourList.Add(result1);
                    }
                    break;
                default:
                    result.Add(GlobalNames.UnknowData, 1);
                    break;
            }
        }
    }
}
