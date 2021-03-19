using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SDRSharp.Tetra
{
    unsafe class MacLevel
    {
        private LlcLevel _llc = new LlcLevel();

        public ChannelType DownLinkChannelType;
        public ChannelType UpLinkChannelType;

        private UnsafeBuffer[] _tempBuffers = new UnsafeBuffer[4];
        private byte*[] _tempBuffersPtr = new byte*[4];

        private int[] _writeAddress = new int[4];
        private int[] _fragmentsEncription = new int[4];
        private ReceivedData[] _fragmentsHeader = new ReceivedData[4];

        public int Field1 { get; private set; }
        public int Field2 { get; private set; }
        public bool HalfSlotStolen { get; private set; }

        public MacLevel()
        {
            for (int i = 0; i < 4; i++)
            {
                _fragmentsHeader[i] = new ReceivedData();
                _tempBuffers[i] = UnsafeBuffer.Create(4096, sizeof(byte));
                _tempBuffersPtr[i] = (byte*)_tempBuffers[i];
            }
        }

        private readonly Rules[] _syncInfoRulesTMO = new Rules[]
        {
            new Rules(GlobalNames.ColorCode, 6 ),
            new Rules(GlobalNames.TimeSlot, 2 ),
            new Rules(GlobalNames.Frame, 5 ),
            new Rules(GlobalNames.MultiFrame, 6 ),
            new Rules(GlobalNames.SharingMode, 2 ),
            new Rules(GlobalNames.TSReservedFrames, 3 ),
            new Rules(GlobalNames.UPlaneDTX, 1 ),
            new Rules(GlobalNames.Frame18Extension, 1 ),
            new Rules(GlobalNames.Reserved, 1 ),
            new Rules(GlobalNames.MCC, 10 ),
            new Rules(GlobalNames.MNC, 14 ),
            new Rules(GlobalNames.NeighbourCellBroadcast, 2 ),
            new Rules(GlobalNames.CellServiceLevel, 2 ),
            new Rules(GlobalNames.LateEntryInfo, 1 )
        };

        private readonly Rules[] _dmacSyncInfoRulesDMO = new Rules[]
        {
            new Rules(GlobalNames.Communication_type, 2 ),
            new Rules(GlobalNames.Master_slave_link_flag, 1 ),
            new Rules(GlobalNames.Gateway_generated_message_flag, 1 ),
            new Rules(GlobalNames.A_B_channel_usage, 2 ),
            new Rules(GlobalNames.TimeSlot, 2 ),
            new Rules(GlobalNames.Frame, 5 ),
            new Rules(GlobalNames.Air_interface_encryption, 2 ),
            new Rules(GlobalNames.Reserved, 39 )
        };

        private readonly Rules[] _dpresSyncInfoRules = new Rules[]
        {
            new Rules(GlobalNames.Repeater_Communication_type, 2 ),
            new Rules(GlobalNames.Repeater_M_DMO_flag, 1 ),
            new Rules(GlobalNames.Reserved, 2, RulesType.Switch, (int)GlobalNames.Repeater_Communication_type, 1),
            new Rules(GlobalNames.Repeater_Two_frequency_flag, 1, RulesType.Switch, (int)GlobalNames.Repeater_Communication_type, 1),
            new Rules(GlobalNames.Repeater_operating_modes, 2, RulesType.Switch, (int)GlobalNames.Repeater_Communication_type, 1),
            new Rules(GlobalNames.Repeater_Spacing_of_uplink, 6, RulesType.Switch, (int)GlobalNames.Repeater_Communication_type, 1),
            new Rules(GlobalNames.Repeater_Master_slave_link_flag, 1 ),
            new Rules(GlobalNames.Repeater_A_B_channel_usage, 2 ),
            new Rules(GlobalNames.Repeater_Channel_state, 2 ),
            new Rules(GlobalNames.Repeater_TimeSlot, 2 ),
            new Rules(GlobalNames.Repeater_Frame, 5 ),
            new Rules(GlobalNames.Repeater_Power_class, 3 ),
            new Rules(GlobalNames.Repeater_Power_control_flag, 1 ),
            new Rules(GlobalNames.Reserved, 1),
            new Rules(GlobalNames.Repeater_Frame_countdown, 2),
            new Rules(GlobalNames.Repeater_Priority_level, 2),
            new Rules(GlobalNames.Reserved, 6, RulesType.Switch, (int)GlobalNames.Repeater_Communication_type, 1),
            new Rules(GlobalNames.Values_of_DN232_DN233, 4, RulesType.Switch, (int)GlobalNames.Repeater_Communication_type, 1),
            new Rules(GlobalNames.Values_of_DT_254, 3, RulesType.Switch, (int)GlobalNames.Repeater_Communication_type, 1),
            new Rules(GlobalNames.Repeater_dual_watch_synchronization_flag, 1),
            new Rules(GlobalNames.Reserved, 5, RulesType.Switch, (int)GlobalNames.Repeater_Communication_type, 1),
        };

        private readonly Rules[] _dmacSyncInfoHalfSlotRules = new Rules[]
        {
            new Rules(GlobalNames.Repeater_address, 10 ),
            new Rules(GlobalNames.Fill_bit, 1 ),
            new Rules(GlobalNames.Fragmentation_flag, 1 ),
            new Rules(GlobalNames.Number_of_slots, 4, RulesType.Switch, (int)GlobalNames.Fragmentation_flag, 1 ),
            new Rules(GlobalNames.Frame_countdown, 2 ),
            new Rules(GlobalNames.Destination_address_type, 2 ),
            new Rules(GlobalNames.Destination_address, 24, RulesType.SwitchNot, (int)GlobalNames.Destination_address_type, 2),
            new Rules(GlobalNames.Source_address_type, 2 ),
            new Rules(GlobalNames.Source_address, 24, RulesType.SwitchNot, (int)GlobalNames.Source_address_type, 2),
            new Rules(GlobalNames.MCC, 10),
            new Rules(GlobalNames.MNC, 14),
            new Rules(GlobalNames.Message_type, 5)

        };

        private readonly Rules[] _dpresSyncInfoHalfSlotRules = new Rules[]
        {
            new Rules(GlobalNames.Repeater_Gateway_address, 10, RulesType.Switch, (int)GlobalNames.Repeater_Communication_type, 1 ),
            new Rules(GlobalNames.Repeater_MCC, 10, RulesType.Switch, (int)GlobalNames.Repeater_Communication_type, 1),
            new Rules(GlobalNames.Repeater_MNC, 14, RulesType.Switch, (int)GlobalNames.Repeater_Communication_type, 1),
            new Rules(GlobalNames.Repeater_Validity_time_unit, 2 ),
            new Rules(GlobalNames.Repeater_Number_of_validity_time_units, 6 ),
            new Rules(GlobalNames.Repeater_Maximum_DM_MS_power_class, 3),
            new Rules(GlobalNames.Reserved, 1),
            new Rules(GlobalNames.Repeater_Usage_restriction_type, 4),

            new Rules(GlobalNames.Repeater_Usage_SCKN, 5, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 8),
            new Rules(GlobalNames.Repeater_Usage_SCKN, 5, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 9),
            new Rules(GlobalNames.Repeater_Usage_SCKN, 5, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 10),
             new Rules(GlobalNames.Repeater_Usage_SCKN, 5, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 11),

             new Rules(GlobalNames.Repeater_Usage_EUIV, 19, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 8),
             new Rules(GlobalNames.Repeater_Usage_EUIV, 19, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 9),
             new Rules(GlobalNames.Repeater_Usage_EUIV, 19, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 10),
             new Rules(GlobalNames.Repeater_Usage_EUIV, 19, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 11),

            new Rules(GlobalNames.Repeater_Usage_MCC, 10, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 2),
            new Rules(GlobalNames.Repeater_Usage_MNC, 14, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 2),

            new Rules(GlobalNames.Repeater_Usage_MCC, 10, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 3),
            new Rules(GlobalNames.Repeater_Usage_MNC, 14, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 3),
            new Rules(GlobalNames.Repeater_Usage_SSI, 24, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 3),

            new Rules(GlobalNames.Repeater_Usage_MCC, 10, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 4),
            new Rules(GlobalNames.Repeater_Usage_MNC, 14, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 4),
            new Rules(GlobalNames.Repeater_Usage_SSI, 24, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 4),
            new Rules(GlobalNames.Repeater_Usage_SSI2, 24, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 4),

            new Rules(GlobalNames.Repeater_Usage_MCC, 10, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 5),
            new Rules(GlobalNames.Repeater_Usage_MNC, 14, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 5),
            new Rules(GlobalNames.Repeater_Usage_SSI, 24, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 5),
            new Rules(GlobalNames.Repeater_Usage_SSI2, 24, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 5),

            new Rules(GlobalNames.Repeater_Usage_SSI, 24, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 6),
            new Rules(GlobalNames.Repeater_Usage_SSI2, 24, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 6),
            new Rules(GlobalNames.Repeater_Usage_SSI3, 24, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 6),

            new Rules(GlobalNames.Repeater_Usage_MCC, 10, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 8),
            new Rules(GlobalNames.Repeater_Usage_MNC, 14, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 8),

            new Rules(GlobalNames.Repeater_Usage_MCC, 10, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 9),
            new Rules(GlobalNames.Repeater_Usage_MNC, 14, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 9),
            new Rules(GlobalNames.Repeater_Usage_SSI, 24, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 9),


            new Rules(GlobalNames.Repeater_Usage_SSI, 24, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 10),

            new Rules(GlobalNames.Repeater_Usage_SSI, 24, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 11),
            new Rules(GlobalNames.Repeater_Usage_SSI2, 24, RulesType.Switch, (int)GlobalNames.Repeater_Usage_restriction_type, 11),

        };


        public enum AccessAssign
        {
            Header = 0,
            Field1,
            Field2,
        }

        private readonly Dictionary<AccessAssign, int> _aachLength = new Dictionary<AccessAssign, int>()
        {
            { AccessAssign.Header, 2 },
            { AccessAssign.Field1, 6 },
            { AccessAssign.Field2, 6 }
        };

        private readonly Rules[] _sysInfoRules = new Rules[]
        {
            new Rules(GlobalNames.Main_Carrier, 12 ),
            new Rules(GlobalNames.Frequency_Band, 4 ),
            new Rules(GlobalNames.Offset, 2 ),
            new Rules(GlobalNames.Duplex_Spacing, 3 ),
            new Rules(GlobalNames.Reverse_Operation, 1 ),
            new Rules(GlobalNames.NumberOfCommon_SC, 2 ),
            new Rules(GlobalNames.MS_TXPwr_Max_Cell, 3 ),
            new Rules(GlobalNames.RXLevel_Access_Min, 4 ),
            new Rules(GlobalNames.Access_Parameter, 4 ),
            new Rules(GlobalNames.Radio_Downlink_Ttimeout, 4 ),
            new Rules(GlobalNames.Hyperframe_or_Cipher_key_flag, 1 ),
            new Rules(GlobalNames.Hyperframe, 16, RulesType.Switch, (int)GlobalNames.Hyperframe_or_Cipher_key_flag, 0 ),
            new Rules(GlobalNames.Cipher_key, 16, RulesType.Switch, (int)GlobalNames.Hyperframe_or_Cipher_key_flag, 1 ),
            new Rules(GlobalNames.Optional_field_flag, 2 ),
            new Rules(GlobalNames.Optional_field_value, 20, RulesType.Switch, (int)GlobalNames.Optional_field_flag, 0 ),
            new Rules(GlobalNames.Optional_field_value, 20, RulesType.Switch, (int)GlobalNames.Optional_field_flag, 1 ),
            new Rules(GlobalNames.Optional_field_value, 20, RulesType.Switch, (int)GlobalNames.Optional_field_flag, 2 ),
            new Rules(GlobalNames.Authentication_required_on_cell, 1, RulesType.Switch, (int)GlobalNames.Optional_field_flag, 3 ),
            new Rules(GlobalNames.Security_Class_1_supported_on_cell, 1, RulesType.Switch, (int)GlobalNames.Optional_field_flag, 3 ),
            new Rules(GlobalNames.Security_Class_3_supported_on_cell, 1, RulesType.Switch, (int)GlobalNames.Optional_field_flag, 3 ),
            new Rules(GlobalNames.Optional_field_value, 17, RulesType.Switch, (int)GlobalNames.Optional_field_flag, 3 ),
            new Rules(GlobalNames.Location_Area, 14 ),
            new Rules(GlobalNames.Subscriber_Class, 16 ),
            new Rules(GlobalNames.Registration_required, 1 ),
            new Rules(GlobalNames.De_registration_required, 1 ),
            new Rules(GlobalNames.Priority_cell, 1 ),
            new Rules(GlobalNames.Cell_never_uses_minimum_mode, 1 ),
            new Rules(GlobalNames.Migration_supported, 1 ),
            new Rules(GlobalNames.System_wide_services, 1 ),
            new Rules(GlobalNames.TETRA_voice_service, 1 ),
            new Rules(GlobalNames.Circuit_mode_data_service, 1 ),
            new Rules(GlobalNames.Reserved, 1),
            new Rules(GlobalNames.SNDCP_Service, 1 ),
            new Rules(GlobalNames.Air_interface_encryption, 1 ),
            new Rules(GlobalNames.Advanced_link_supported, 1 )
        };

        private readonly Rules[] _accessDefineRules = new Rules[]
        {
            new Rules(GlobalNames.Common_or_assigned, 1 ),
            new Rules(GlobalNames.Access_code, 2 ),
            new Rules(GlobalNames.IMM, 4 ),
            new Rules(GlobalNames.WT, 4 ),
            new Rules(GlobalNames.Nu, 4 ),
            new Rules(GlobalNames.Frame_length_factor, 1 ),
            new Rules(GlobalNames.Timeslot_pointer, 4 ),
            new Rules(GlobalNames.Minimum_priority, 3 ),
            new Rules(GlobalNames.Optional_field_flag, 2 ),
            new Rules(GlobalNames.Subscriber_class, 16, RulesType.Switch, (int)GlobalNames.Optional_field_flag, 1),
            new Rules(GlobalNames.GSSI, 24, RulesType.Switch, (int)GlobalNames.Optional_field_flag, 2 ),
            new Rules(GlobalNames.Filler_bits, 3 )
        };


        public void AccessAsignPDU(LogicChannel channelData)
        {
            var offset = 0;

            var length = _aachLength[AccessAssign.Header];
            var header = TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
            offset += length;

            length = _aachLength[AccessAssign.Field1];
            var field1 = TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
            offset += length;

            length = _aachLength[AccessAssign.Field2];
            var field2 = TetraUtils.BitsToInt32(channelData.Ptr, offset, length);

            Field1 = field1;
            Field2 = field2;

            if (channelData.Frame == 18)
            {
                DownLinkChannelType = ChannelType.Common;
                return;
            }

            switch (header)
            {
                case 0:
                    DownLinkChannelType = ChannelType.Common;
                    break;
                case 1:
                case 2:
                case 3:
                    switch (field1)
                    {
                        case 0:
                            DownLinkChannelType = ChannelType.Unalloc;
                            break;

                        case 1:
                            DownLinkChannelType = ChannelType.Assigned;
                            break;

                        case 2:
                            DownLinkChannelType = ChannelType.Common;
                            break;

                        case 3:
                            DownLinkChannelType = ChannelType.Reserved;
                            break;

                        default:
                            DownLinkChannelType = ChannelType.Traffic;
                            break;

                    }
                    break;
            }
        }

        public void ResetAACH()
        {
            Field1 = 0;
            DownLinkChannelType = ChannelType.Common;
            Field2 = 0;
            UpLinkChannelType = ChannelType.Common;
        }

        private readonly Rules[] _macEndRules = new Rules[]
        {
            new Rules(GlobalNames.Fill_bit, 1 ),
            new Rules(GlobalNames.Position_of_grant, 1 ),
            new Rules(GlobalNames.Length_indication, 6 ),
            new Rules(GlobalNames.Slot_granting_flag, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Slot_granting_element, 8 )
        };

        private readonly Rules[] _dmacEndRules = new Rules[]
        {
            new Rules(GlobalNames.Fill_bit, 1 )
        };

        private readonly Rules[] _dmacDataRules = new Rules[]
        {
            new Rules( GlobalNames.Fill_bit, 1 ),
            new Rules(GlobalNames.Second_half_slot_stolen, 1 ),
            new Rules(GlobalNames.Fragmentation_flag, 1 ),
            new Rules(GlobalNames.Null_pdu, 1 ),
            new Rules(GlobalNames.Frame_countdown, 2 ),
            new Rules(GlobalNames.Air_interface_encryption, 2),
            new Rules(GlobalNames.Destination_address_type, 2),
            new Rules(GlobalNames.Destination_address, 24, RulesType.SwitchNot, (int) GlobalNames.Destination_address_type, 2),
            new Rules(GlobalNames.Source_address_type, 2),
            new Rules(GlobalNames.Source_address, 24, RulesType.SwitchNot, (int) GlobalNames.Source_address_type, 2),
            new Rules(GlobalNames.MCC, 10),
            new Rules(GlobalNames.MNC, 14),
            new Rules(GlobalNames.Message_type, 5)
        };

        private readonly Rules[] _macResourceRules = new Rules[]
        {
            new Rules( GlobalNames.Fill_bit, 1 ),
            new Rules(GlobalNames.Position_of_grant, 1 ),
            new Rules(GlobalNames.Encryption_mode, 2 ),
            new Rules(GlobalNames.Random_access_flag, 1 ),
            new Rules(GlobalNames.Length_indication, 6 ),
            new Rules(GlobalNames.Address_type, 3),
            new Rules(GlobalNames.SSI, 24, RulesType.Switch, (int) GlobalNames.Address_type, 1),
            new Rules(GlobalNames.Event_Label, 10, RulesType.Switch, (int) GlobalNames.Address_type, 2),
            new Rules(GlobalNames.USSI, 24, RulesType.Switch, (int) GlobalNames.Address_type, 3),
            new Rules(GlobalNames.SMI, 24, RulesType.Switch, (int) GlobalNames.Address_type, 4),
            new Rules(GlobalNames.SSI, 24, RulesType.Switch, (int) GlobalNames.Address_type, 5),
            new Rules(GlobalNames.Event_Label, 10, RulesType.Switch, (int) GlobalNames.Address_type, 5),
            new Rules(GlobalNames.SSI, 24, RulesType.Switch, (int) GlobalNames.Address_type, 6),
            new Rules(GlobalNames.Usage, 6, RulesType.Switch, (int) GlobalNames.Address_type, 6),
            new Rules(GlobalNames.SMI, 24, RulesType.Switch, (int) GlobalNames.Address_type, 7),
            new Rules(GlobalNames.Event_Label, 10, RulesType.Switch, (int) GlobalNames.Address_type, 7),
            new Rules(GlobalNames.Power_control_flag, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Power_control_element, 4 ),
            new Rules(GlobalNames.Slot_granting_flag, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Slot_granting_element, 8 ),
        };

        private readonly Rules[] _channelAllocationRules = new Rules[]
        {
            new Rules(GlobalNames.Channel_allocation_flag, 1, RulesType.Options_bit),
            new Rules(GlobalNames.Allocation_type, 2 ),
            new Rules(GlobalNames.Timeslot_assigned, 4 ),
            new Rules(GlobalNames.Uplink_downlink_assigned, 2 ),
            new Rules(GlobalNames.CLCH_permission, 1 ),
            new Rules(GlobalNames.Cell_change_flag, 1 ),
            new Rules(GlobalNames.Carrier_number, 12 ),
            new Rules(GlobalNames.Extended_carrier_numbering_flag, 1, RulesType.Presence_bit, 4),
            new Rules(GlobalNames.Extended_frequency_band, 4 ),
            new Rules(GlobalNames.Extended_offset, 2 ),
            new Rules(GlobalNames.Extended_duplex_spacing, 3 ),
            new Rules(GlobalNames.Extended_reverse_operation, 1 ),
            new Rules(GlobalNames.Monitoring_pattern, 2 ),
            new Rules(GlobalNames.Frame_18_monitoring_pattern, 2, RulesType.Switch, (int)GlobalNames.Monitoring_pattern, 0),
            new Rules(GlobalNames.Reserved, 0, RulesType.JampNot, (int)GlobalNames.Uplink_downlink_assigned, 0, 99),
            new Rules(GlobalNames.Up_downlink_assigned_for_augmented_ch_alloc, 2),
            new Rules(GlobalNames.Bandwidth_of_allocated_channel, 3),
            new Rules(GlobalNames.Modulation_mode_of_allocated_channel, 3),
            new Rules(GlobalNames.Maximum_uplink_QAM_modulation_level, 3),
            new Rules(GlobalNames.Conforming_channel_status, 3),
            new Rules(GlobalNames.BS_link_imbalance, 4),
            new Rules(GlobalNames.BS_transmit_power_relative_to_main_carrier, 5),
            new Rules(GlobalNames.Napping_status, 2),
            new Rules(GlobalNames.Napping_information, 11, RulesType.Switch, (int)GlobalNames.Napping_status, 1),
            new Rules(GlobalNames.Reserved, 4),
            new Rules(GlobalNames.Conditional_element_A_flag, 1, RulesType.Presence_bit, 1),
            new Rules(GlobalNames.Conditional_element_A, 16),
            new Rules(GlobalNames.Conditional_element_B_flag, 1, RulesType.Presence_bit, 1),
            new Rules(GlobalNames.Conditional_element_B, 16),
            new Rules(GlobalNames.Further_augmentation_flag, 1)
        };

        private readonly Rules[] _uSignalRules = new Rules[]
        {
            new Rules(GlobalNames.Second_half_slot_stolen, 1 )
        };

        public void UsignalPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            HalfSlotStolen = (TetraUtils.BitsToInt32(channelData.Ptr, offset, 1) != 0);
            offset++;

            //Debug.Write(" Usig_PDU");

            //Possible pdu encrypted        
            //_llc.Parse(channelData, offset, result);
        }

        #region Sync info both system
        public void SyncPDU(LogicChannel channelData, ReceivedData result)
        {
            var length = 4;
            var offset = 0;
            var systemCode = TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
            offset += length;

            result.SetValue(GlobalNames.SystemCode, systemCode);

            switch (systemCode)
            {
                case 12:
                case 13:
                    length = 2;
                    var pduType = TetraUtils.BitsToInt32(channelData.Ptr, offset, length);
                    offset += length;
                    result.SetValue(GlobalNames.SYNC_PDU_type, pduType);

                    switch (pduType)
                    {
                        case 0:
                            //Dmac-Sync
                            Debug.Write(" DMAC-Sync");
                            offset = Global.ParseParams(channelData, offset, _dmacSyncInfoRulesDMO, result);
                            break;

                        case 1:
                            //DPRES-SYNC
                            Debug.Write(" Dpres-Sync");
                            offset = Global.ParseParams(channelData, offset, _dpresSyncInfoRules, result);
                            break;

                        default:
                            //Reserved;
                            Debug.Write(" Unknow_DMO_SyncPdu_type");
                            result.SetValue(GlobalNames.UnknowData, 1);
                            break;
                    }
                    break;

                default:
                    Global.ParseParams(channelData, offset, _syncInfoRulesTMO, result);
                    break;
            }

        }

        public void SyncPDUHalfSlot(LogicChannel channelData, ReceivedData result)
        {
            var pduType = -1;
            var offset = 0;

            if (result.TryGetValue(GlobalNames.SYNC_PDU_type, ref pduType))
            {
                switch (pduType)
                {
                    case 0:
                        offset = Global.ParseParams(channelData, offset, _dmacSyncInfoHalfSlotRules, result);

                        var realLength = channelData.Length - offset;

                        if (result.Value(GlobalNames.Fill_bit) != 0)
                            realLength = CalcRealLength(channelData.Ptr, offset, realLength);


                        if (result.Value(GlobalNames.Fragmentation_flag) == 1)
                        {
                            Debug.Write(" Start_fragmented_PDU");
                            CreateFraqmentsBuffer(channelData, offset, realLength, result);
                        }

                        //Debug.Write(" " + (DMAC_Message_Type)result[GlobalNames.Message_type]);
                        break;

                    case 1:
                        Global.ParseParams(channelData, 0, _dpresSyncInfoHalfSlotRules, result);
                        break;

                    default:
                        Debug.Write(" Unknow_DMO_SyncPdu_type");
                        result.SetValue(GlobalNames.UnknowData, 1);
                        break;
                }
            }
        }

        #endregion

        #region TMO

        public int SysInfoPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            var subType = TetraUtils.BitsToByte(channelData.Ptr, offset, 2);
            result.SetValue(GlobalNames.MAC_Broadcast_Type, subType);
            offset += 2;

            switch (subType)
            {
                case 1:
                    //Debug.Write(" AcсessDef_PDU");
                    offset = Global.ParseParams(channelData, offset, _accessDefineRules, result);
                    break;

                case 0:
                    //Debug.Write(" Broadcast_PDU");
                    offset = Global.ParseParams(channelData, offset, _sysInfoRules, result);
                    break;
            }

            return offset;
        }


        public void TmoParseMacPDU(LogicChannel channelData, List<ReceivedData> result)
        {
            var nullPduLength = 16; // only for receive pi/4 modulation ;
            var blockOffset = 0;
            var nullPdu = false;
            var pduEnd = false;

            for (int i = 0; i < 5; i++)
            {

                var offset = blockOffset;
                var pdu = new ReceivedData();

                pdu.Add(GlobalNames.CurrTimeSlot, channelData.TimeSlot);

                var resourceType = (MAC_PDU_Type)TetraUtils.BitsToInt32(channelData.Ptr, offset, 2);
                offset += 2;
                pdu.Add(GlobalNames.MAC_PDU_Type, (int)resourceType);

                switch (resourceType)
                {
                    case MAC_PDU_Type.Broadcast:
                        blockOffset = SysInfoPDU(channelData, offset, pdu);
                        break;

                    case MAC_PDU_Type.MAC_resource:
                        ResourcePDU(channelData, offset, pdu);
                        blockOffset += (pdu.Value(GlobalNames.Length_indication) * 8);
                        break;

                    case MAC_PDU_Type.MAC_U_Signal:
                        if (channelData.Length == 124)
                        {
                            UsignalPDU(channelData, offset, pdu);
                            pduEnd = true;
                        }
                        else
                        {
                            Debug.Write(" MAC-D-BLCK_PDU");
                            pduEnd = true;
                        }
                        break;

                    case MAC_PDU_Type.MAC_frag:

                        var isTheEnd = TetraUtils.BitsToInt32(channelData.Ptr, offset, 1);
                        offset++;

                        if (isTheEnd == 0)
                        {
                            MacFraqPDU(channelData, offset, pdu);
                            pduEnd = true;
                        }
                        else
                        {
                            MacEndPDU(channelData, offset, pdu);
                            blockOffset += (pdu.Value(GlobalNames.Length_indication) * 8);
                        }
                        break;
                }

                nullPdu = pdu.Contains(GlobalNames.Null_pdu) || pdu.Contains(GlobalNames.OutOfBuffer);

                if (nullPdu) pduEnd = true;

                //if (pdu.ContainsKey(GlobalNames.UnknowData)) nullPdu = true;

                if (!nullPdu) result.Add(pdu);

                if ((blockOffset >= (channelData.Length - nullPduLength)) || pduEnd) break;
            }
        }

        public void ResourcePDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            var startOffset = offset - 2;

            offset = Global.ParseParams(channelData, offset, _macResourceRules, result);

            //Debug.Write(" | Resource_PDU");

            var encr = result.Value(GlobalNames.Encryption_mode);
            var lengthIndication = result.Value(GlobalNames.Length_indication); //Only for pi/4 modulation

            HalfSlotStolen = (lengthIndication == 62) || (lengthIndication == 63);

            Debug.WriteIf(HalfSlotStolen, " Second_slot_stolen");
            Debug.WriteIf(((lengthIndication < 62) && (lengthIndication > 58)), " Incorrect_PDU_length");

            if (((lengthIndication > 58) && (lengthIndication < 62)) || (lengthIndication == 0) || result.Value(GlobalNames.Address_type) == 0)
            {
                result.Add(GlobalNames.Null_pdu, 1);
                //Debug.Write(" Null_PDU");
                return;
            }

            if (encr != 0) // All data before channel allocation flag is clear
            {                   // after is encrypted
                Debug.Write(" Encr");
                return;
            }

            offset = Global.ParseParams(channelData, offset, _channelAllocationRules, result);

            if (lengthIndication != 63)
            {
                _llc.Parse(channelData, offset, result);
                return;
            }

            var realLength = Math.Min(lengthIndication * 8 - (offset - startOffset), channelData.Length - offset);

            if (result.Value(GlobalNames.Fill_bit) != 0)
                realLength = CalcRealLength(channelData.Ptr, offset, realLength);

            if (realLength < 0)
            {
                result.Add(GlobalNames.UnknowData, 1);
                Debug.Write(" frag_length_err");
                return;
            }

            //Debug.Write(" Start_fragmented_PDU");
            CreateFraqmentsBuffer(channelData, offset, realLength, result);
        }

        public void MacEndPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            var startOffset = offset - 3;

            offset = Global.ParseParams(channelData, offset, _macEndRules, result);

            //Debug.Write(" | FragEnd_PDU");

            if (FraqmentsBufferIsEmpty(channelData.TimeSlot))
            {
                Debug.Write(" Buffer_is_empty");
                return;
            }

            offset = Global.ParseParams(channelData, offset, _channelAllocationRules, result);

            var lengthIndication = result.Value(GlobalNames.Length_indication);
            var realLength = Math.Min(lengthIndication * 8 - (offset - startOffset), channelData.Length - offset);

            if (lengthIndication > 58)
            {
                Debug.Write(" Incorrect_PDU_length");
                result.Add(GlobalNames.Null_pdu, 1);
                return;
            }

            if (result.Value(GlobalNames.Fill_bit) != 0)
                realLength = CalcRealLength(channelData.Ptr, offset, realLength);

            if (realLength < 0)
            {
                result.Add(GlobalNames.UnknowData, 1);
                Debug.Write(" frag_length_err");
                return;
            }

            AddFraqmentsToBuffer(channelData, offset, realLength);

            var newBuffer = GetDeFragmentedBuffer(channelData, result);

            if (newBuffer.Length == 0)
            {
                Debug.Write(" Defrag_PDU_length_err");
                result.Add(GlobalNames.Null_pdu, 1);
                return;
            }

            //Debug.Write(" Defrag_PDU_length=" + newBuffer.Length.ToString());
            offset = 0;
            _llc.Parse(newBuffer, offset, result);
        }

        public void MacFraqPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            var startOffset = offset - 3;

            //Debug.Write(" | Frag_PDU");

            if (FraqmentsBufferIsEmpty(channelData.TimeSlot))
            {
                Debug.Write(" Buffer_is_empty!!!");
                return;
            }

            var fill = (TetraUtils.BitsToInt32(channelData.Ptr, offset, 1) != 0);
            offset++;

            var realLength = channelData.Length - offset;

            if (fill)
            {
                realLength = CalcRealLength(channelData.Ptr, offset, realLength);
            }

            if (realLength < 0)
            {
                result.Add(GlobalNames.UnknowData, 1);
                Debug.Write(" frag_length_err");
                return;
            }

            AddFraqmentsToBuffer(channelData, offset, realLength);
        }

        #endregion

        #region DMO

        public void DmoParseMacPDU(LogicChannel channelData, List<ReceivedData> result)
        {
            var offset = 0;
            var pdu = new ReceivedData();

            pdu.Add(GlobalNames.CurrTimeSlot, channelData.TimeSlot);

            var pduType = (MAC_PDU_Type)TetraUtils.BitsToInt32(channelData.Ptr, offset, 2);
            offset += 2;
            pdu.Add(GlobalNames.MAC_PDU_Type, (int)pduType);

            switch (pduType)
            {
                case MAC_PDU_Type.MAC_resource://DMAC-DATA
                    DmacDataPDU(channelData, offset, pdu);
                    break;

                case MAC_PDU_Type.MAC_frag:

                    var isTheEnd = TetraUtils.BitsToInt32(channelData.Ptr, offset, 1);
                    offset++;

                    if (isTheEnd == 0)
                    {
                        Debug.Write(" DMAC-Frag_PDU");
                        DmacFraqPDU(channelData, offset, pdu);
                    }
                    else
                    {
                        Debug.Write(" DMAC-End_PDU");
                        DmacEndPDU(channelData, offset, pdu);
                    }
                    break;

                case MAC_PDU_Type.MAC_U_Signal:
                    Debug.Write(" U-signal_DMO_PDU");
                    //UsignalPDU(channelData, offset, pdu);
                    break;

                case MAC_PDU_Type.Broadcast://Not used for dmo
                    Debug.Write(" Reserved_DMO_PDU");
                    break;

            }
            result.Add(pdu);
        }

        public void DmacDataPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            offset = Global.ParseParams(channelData, offset, _dmacDataRules, result);
            var realLength = channelData.Length - offset;

            if (result.Value(GlobalNames.Fill_bit) != 0)
                realLength = CalcRealLength(channelData.Ptr, offset, realLength);

            HalfSlotStolen = result.Value(GlobalNames.Second_half_slot_stolen) == 1;
            //Debug.WriteIf(HalfSlotStolen, " Second_slot_stolen");

            if (result.Value(GlobalNames.Fragmentation_flag) == 1)
            {
                Debug.Write(" Start_fragmented_PDU");
                CreateFraqmentsBuffer(channelData, offset, realLength, result);
                return;
            }

            if (result.Value(GlobalNames.Null_pdu) == 1)
            {
                Debug.Write(" Null_PDU");
                return;
            }

            //Debug.Write(" DMAC-Data_PDU " + (DMAC_Message_Type)result[GlobalNames.Message_type]);

            //Debug.WriteIf(result[GlobalNames.Air_interface_encryption] != 0, " Encr");

            //_llc.Parse(channelData, offset, result);
        }

        public void DmacEndPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            offset = Global.ParseParams(channelData, offset, _dmacEndRules, result);

            var realLength = channelData.Length - offset;

            if (result.Value(GlobalNames.Fill_bit) != 0)
                realLength = CalcRealLength(channelData.Ptr, offset, realLength);

            Debug.Write(" END_fragmented_PDU");

            AddFraqmentsToBuffer(channelData, offset, realLength);

            var newBuffer = GetDeFragmentedBuffer(channelData, result);
            offset = 0;

            Debug.Write(" Defragmented_PDU_length=" + newBuffer.Length.ToString());

            if (newBuffer.Length == 0) return;

            //_llc.Parse(newBuffer, offset, result);
        }

        public void DmacFraqPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            var fill = (TetraUtils.BitsToInt32(channelData.Ptr, offset, 1) != 0);
            offset++;

            var realLength = channelData.Length - offset;

            if (fill)
            {
                realLength = CalcRealLength(channelData.Ptr, offset, realLength);
            }

            AddFraqmentsToBuffer(channelData, offset, realLength);

            Debug.Write("Fragment_PDU");
        }


        #endregion

        #region Defragmentation

        private int CalcRealLength(byte* buffer, int offset, int currentLength)
        {
            var pos = offset + currentLength - 1;
            while (pos > offset && buffer[pos] == 0) pos--;
            return pos - offset;
        }

        private void CreateFraqmentsBuffer(LogicChannel buffer, int offset, int length, ReceivedData header)
        {
            var ts = buffer.TimeSlot - 1;

            Debug.WriteIf(_writeAddress[ts] != 0, " Warning!!! Fraqments buffer is not empty!");
            if (_writeAddress[ts] + length < _tempBuffers[ts].Length)
            {
                Radio.Utils.Memcpy(_tempBuffersPtr[ts], buffer.Ptr + offset, length * sizeof(byte));
                _writeAddress[ts] = length;
                if (header != null)
                    for (int i = 0; i < header.Data.Length; i++)
                        _fragmentsHeader[ts].Data[i] = header.Data[i];
            }
        }

        private void AddFraqmentsToBuffer(LogicChannel buffer, int offset, int length)
        {
            var ts = buffer.TimeSlot - 1;

            if (_writeAddress[ts] == 0) return;

            if (_writeAddress[ts] + length < _tempBuffers[ts].Length)
            {
                Radio.Utils.Memcpy(_tempBuffersPtr[ts] + _writeAddress[ts], buffer.Ptr + offset, length * sizeof(byte));
                _writeAddress[ts] += length;
            }
        }

        private bool FraqmentsBufferIsEmpty(int timeSlot)
        {
            var ts = timeSlot - 1;
            return _writeAddress[ts] == 0;
        }

        private LogicChannel GetDeFragmentedBuffer(LogicChannel channelData, ReceivedData header)
        {
            var ts = channelData.TimeSlot - 1;

            Debug.WriteIf(_writeAddress[ts] == 0, " Warning!!! Fraqments  buffer is empty!");

            var result = new LogicChannel
            {
                Ptr = _tempBuffersPtr[ts],
                Length = _writeAddress[ts],
                CrcIsOk = true,
                TimeSlot = channelData.TimeSlot,
                Frame = channelData.Frame
            };

            var key = GlobalNames.Encryption_mode;
            if (_fragmentsHeader[ts].Contains(key)) header.Add(key, _fragmentsHeader[ts].Value(key));
            key = GlobalNames.Address_type;
            if (_fragmentsHeader[ts].Contains(key)) header.Add(key, _fragmentsHeader[ts].Value(key));
            key = GlobalNames.SSI;
            if (_fragmentsHeader[ts].Contains(key)) header.Add(key, _fragmentsHeader[ts].Value(key));

            _fragmentsHeader[ts].Clear();
            _writeAddress[ts] = 0;

            return result;
        }

        #endregion
    }
}