using System.Collections.Generic;
using System.Diagnostics;

namespace SDRSharp.Tetra
{
    unsafe class MleLevel
    {
        private SdsParser _sds = new SdsParser();

        public void Parse(LogicChannel channelData, int offset, ReceivedData result)
        {
            var protokol = (MLEPduType)TetraUtils.BitsToInt32(channelData.Ptr, offset, 3);
            offset += 3;
            result.Add(GlobalNames.MLE_PDU_Type, (int)protokol);

            //Debug.Write(" " + protokol.ToString());

            switch (protokol)
            {
                case MLEPduType.CMCE:
                    ParseCMCEPDU(channelData, offset, result);
                    break;

                case MLEPduType.MLE:
                    ParseMLEPDU(channelData, offset, result);
                    break;

                default:
                    //Debug.Write(" Unknow_PDU_Type");
                    result.Add(GlobalNames.UnknowData, 1);
                    break;

            }
        }

        private void ParseCMCEPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            var length = 5;
            var type = (CmcePrimitivesType)TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
            offset += length;
            result.Add(GlobalNames.CMCE_Primitives_Type, (int)type);

            //Debug.Write(" " + type.ToString());

            switch (type)
            {
                case CmcePrimitivesType.D_Connect:
                    Global.ParseParams(channelData, offset, _d_connectRules, result);
                    break;

                case CmcePrimitivesType.D_Setup:
                    Global.ParseParams(channelData, offset, _d_setupRules, result);
                    break;

                case CmcePrimitivesType.D_TX_Granted:
                    Global.ParseParams(channelData, offset, _d_TX_GrantedRules, result);
                    break;

                case CmcePrimitivesType.D_TX_Ceased:
                    Global.ParseParams(channelData, offset, _d_TX_CeasedRules, result);
                    break;

                case CmcePrimitivesType.D_Release:
                    Global.ParseParams(channelData, offset, _d_ReleaseRules, result);
                    break;

                case CmcePrimitivesType.D_Disconnect:

                    break;
                case CmcePrimitivesType.D_Connect_Acknowledgea:

                    break;
                case CmcePrimitivesType.D_Info:
                    Global.ParseParams(channelData, offset, _d_infoRules, result);
                    break;

                case CmcePrimitivesType.D_SDS_Data:
                    offset = Global.ParseParams(channelData, offset, _d_SdsDataRules, result);

                    if (result.Contains(GlobalNames.User_Defined_Data4_Length))
                    {
                        var sdsData = new LogicChannel
                        {
                            Ptr = channelData.Ptr + offset,
                            Length = result.Value(GlobalNames.User_Defined_Data4_Length),
                        };

                        _sds.ParseSDS(sdsData, 0, result);
                    }
                    break;

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

                    //Debug.Write(" Unknow_CMCE_SDU");
                    result.Add(GlobalNames.UnknowData, 1);

                    break;
            }
        }

        private readonly Rules[] _d_ReleaseRules = new Rules[]
        {
            new Rules(GlobalNames.Call_identifier, 14 ),
            new Rules(GlobalNames.Disconnect_cause, 5 ),
            new Rules(GlobalNames.Options_bit, 1 , RulesType.Options_bit),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
            new Rules(GlobalNames.Notification_indicator, 6 ),
            new Rules(GlobalNames.More_bit, 1, RulesType.More_bit)
        };

        private readonly Rules[] _d_SdsDataRules = new Rules[]
        {
            new Rules(GlobalNames.Calling_party_type_identifier, 2 ),
            new Rules(GlobalNames.Calling_party_address_SSI, 24 ),
            new Rules(GlobalNames.Calling_party_extension, 24, RulesType.Switch, (int)GlobalNames.Calling_party_type_identifier, 2),
            new Rules(GlobalNames.Short_data_type_identifier, 2 ),
            new Rules(GlobalNames.User_Defined_Data_16, 16, RulesType.Switch, (int)GlobalNames.Short_data_type_identifier, 0),
            new Rules(GlobalNames.User_Defined_Data_32, 32, RulesType.Switch, (int)GlobalNames.Short_data_type_identifier, 1),
            new Rules(GlobalNames.User_Defined_Data_64_1, 32, RulesType.Switch, (int)GlobalNames.Short_data_type_identifier, 2),
            new Rules(GlobalNames.User_Defined_Data_64_2, 32, RulesType.Switch, (int)GlobalNames.Short_data_type_identifier, 2),
            new Rules(GlobalNames.User_Defined_Data4_Length, 11, RulesType.Switch, (int)GlobalNames.Short_data_type_identifier, 3)
        };

        private readonly Rules[] _d_TX_CeasedRules = new Rules[]
        {
            new Rules(GlobalNames.Call_identifier, 14 ),
            new Rules(GlobalNames.Transmission_request_permission, 1 ),
            new Rules(GlobalNames.Options_bit, 1 , RulesType.Options_bit),
            new Rules(GlobalNames.Presence_bit, 1 , RulesType.Presence_bit, 1),
            new Rules(GlobalNames.Notification_indicator, 6 ),
            new Rules(GlobalNames.More_bit, 1, RulesType.More_bit)
        };

        private readonly Rules[] _d_TX_GrantedRules = new Rules[]
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
            new Rules(GlobalNames.Transmitting_party_extension, 24, RulesType.Switch, (int)GlobalNames.Transmitting_party_type_identifier, 2),
            new Rules(GlobalNames.More_bit, 1, RulesType.More_bit)
        };

        private readonly Rules[] _d_setupRules = new Rules[]
        {
            new Rules(GlobalNames.Call_identifier, 14),
            new Rules(GlobalNames.Call_time_out, 4 ),
            new Rules(GlobalNames.Hook_method, 1 ),
            new Rules(GlobalNames.Simplex_duplex, 1 ),
            new Rules(GlobalNames.Basic_service_Circuit_mode_type, 3 ),
            new Rules(GlobalNames.Basic_service_Encryption_flag, 1 ),
            new Rules(GlobalNames.Basic_service_Communication_type, 2 ),
            new Rules(GlobalNames.Basic_service_Slots_per_frame, 2, RulesType.SwitchNot, (int)GlobalNames.Basic_service_Circuit_mode_type, 0),
            new Rules(GlobalNames.Basic_service_Speech_service, 2, RulesType.Switch, (int)GlobalNames.Basic_service_Circuit_mode_type, 0),
            new Rules(GlobalNames.Transmission_grant, 2 ),
            new Rules(GlobalNames.Transmission_request_permission, 1 ),
            new Rules(GlobalNames.Call_priority, 4 ),
            new Rules(GlobalNames.Options_bit, 1, RulesType.Options_bit ),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Notification_indicator, 6 ),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Temporary_address, 24 ),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 3 ),
            new Rules(GlobalNames.Calling_party_type_identifier, 2 ),
            new Rules(GlobalNames.Calling_party_address_SSI, 24 ),
            new Rules(GlobalNames.Calling_party_extension, 24, RulesType.Switch, (int)GlobalNames.Calling_party_type_identifier, 2),
            new Rules(GlobalNames.More_bit, 1 , RulesType.More_bit)
        };

        private readonly Rules[] _d_infoRules = new Rules[]
        {
            new Rules(GlobalNames.Call_identifier, 14),
            new Rules(GlobalNames.Reset_Call_time_out, 1 ),
            new Rules(GlobalNames.Poll_request, 1 ),
            new Rules(GlobalNames.Options_bit, 1, RulesType.Options_bit ),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.New_Call_Identifier, 14 ),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Call_time_out, 4 ),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Call_time_out_setup_phase, 3 ),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Call_ownership, 1 ),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Modify, 9 ),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Call_status, 3 ),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Temporary_address, 24 ),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Notification_indicator, 6 ),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Poll_response_percentage, 6 ),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Poll_response_number, 6 ),
            new Rules(GlobalNames.More_bit, 1 , RulesType.More_bit)
        };

        private readonly Rules[] _d_connectRules = new Rules[]
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
            new Rules(GlobalNames.Basic_service_Circuit_mode_type, 3 ),
            new Rules(GlobalNames.Basic_service_Encryption_flag, 1 ),
            new Rules(GlobalNames.Basic_service_Communication_type, 2 ),
            new Rules(GlobalNames.Basic_service_Slots_per_frame, 2, RulesType.SwitchNot, (int)GlobalNames.Basic_service_Circuit_mode_type, 0),
            new Rules(GlobalNames.Basic_service_Speech_service, 2, RulesType.Switch, (int)GlobalNames.Basic_service_Circuit_mode_type, 0),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
            new Rules(GlobalNames.Temporary_address, 24),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
            new Rules(GlobalNames.Notification_indicator, 6),
            new Rules(GlobalNames.More_bit, 1, RulesType.More_bit)
        };

        private readonly Rules[] _d_Nwrk_BroadcastRules = new Rules[]
        {
            new Rules(GlobalNames.Cell_reselect_parameters, 16),
            new Rules(GlobalNames.Cell_service_level, 2),
            new Rules(GlobalNames.Options_bit, 1, RulesType.Options_bit),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 3),
            new Rules(GlobalNames.Network_time, 24),
            new Rules(GlobalNames.Local_time_offset_sign, 1),
            new Rules(GlobalNames.Local_time_offset, 23),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
            new Rules(GlobalNames.Number_of_Neighbour_cells_element, 3),
        };

        private readonly Rules[] _neighbour_Cell_InfoRules = new Rules[]
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
            new Rules(GlobalNames.Registration_required, 1 ),
            new Rules(GlobalNames.De_registration_required, 1 ),
            new Rules(GlobalNames.Priority_cell, 1 ),
            new Rules(GlobalNames.Cell_never_uses_minimum_mode, 1 ),
            new Rules(GlobalNames.Migration_supported, 1 ),
            new Rules(GlobalNames.System_wide_services, 1 ),
            new Rules(GlobalNames.TETRA_voice_service, 1 ),
            new Rules(GlobalNames.Circuit_mode_data_service, 1 ),
            new Rules(GlobalNames.Reserved, 1 ),
            new Rules(GlobalNames.SNDCP_Service, 1 ),
            new Rules(GlobalNames.Air_interface_encryption, 1 ),
            new Rules(GlobalNames.Advanced_link_supported, 1 ),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
            new Rules(GlobalNames.Timeshare_cell_and_AI_encryption, 5),
            new Rules(GlobalNames.Presence_bit, 1, RulesType.Presence_bit, 1),
            new Rules(GlobalNames.TDMA_frame_offset, 6),
        };

        private void ParseMLEPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            var length = 3;
            var type = (MlePrimitivesType)TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
            offset += length;
            result.Add(GlobalNames.MLE_Primitives_Type, (int)type);

            //Debug.Write(" " + type.ToString());

            switch (type)
            {
                case MlePrimitivesType.D_NWRK_BROADCAST:
                    offset = Global.ParseParams(channelData, offset, _d_Nwrk_BroadcastRules, result);

                    var count = 0;

                    if (result.TryGetValue(GlobalNames.Number_of_Neighbour_cells_element, ref count))
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var neighbourData = new ReceivedData();
                            offset = Global.ParseParams(channelData, offset, _neighbour_Cell_InfoRules, neighbourData);

                            var cellID = 0;
                            if (neighbourData.TryGetValue(GlobalNames.Cell_identifier, ref cellID))
                            {
                                //////Debug.WriteLine("Neighbour " + Thread.CurrentThread.ManagedThreadId.ToString());

                                if (Global.NeighbourList.Count < 32)
                                {
                                    Global.NeighbourList.Add(neighbourData);
                                }
                            }
                        }
                    }

                    break;

                case MlePrimitivesType.D_NEW_CELL:
                case MlePrimitivesType.D_PREPARE:
                case MlePrimitivesType.D_RESTORE_ACK:
                case MlePrimitivesType.D_RESTORE_FAIL:

                default:
                    //Debug.Write(" Unknow_MLE_SDU");
                    result.Add(GlobalNames.UnknowData, 1);
                    break;
            }
        }
    }
}
