// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.MacLevel
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using SDRSharp.Radio;
using System;
using System.Collections.Generic;

namespace SDRSharp.Tetra
{
    internal class MacLevel
    {
        private LlcLevel _llc = new LlcLevel();
        public ChannelType DownLinkChannelType;
        public ChannelType UpLinkChannelType;
        private UnsafeBuffer[] _tempBuffers = new UnsafeBuffer[4];
        private unsafe byte*[] _tempBuffersPtr = new byte*[4];
        private int[] _writeAddress = new int[4];
        private int[] _fragmentsEncription = new int[4];
        private ReceivedData[] _fragmentsHeader = new ReceivedData[4];
        private readonly Rules[] _syncInfoRulesTMO = new Rules[14]
        {
      new Rules(GlobalNames.ColorCode, 6),
      new Rules(GlobalNames.TimeSlot, 2),
      new Rules(GlobalNames.Frame, 5),
      new Rules(GlobalNames.MultiFrame, 6),
      new Rules(GlobalNames.SharingMode, 2),
      new Rules(GlobalNames.TSReservedFrames, 3),
      new Rules(GlobalNames.UPlaneDTX, 1),
      new Rules(GlobalNames.Frame18Extension, 1),
      new Rules(GlobalNames.Reserved, 1),
      new Rules(GlobalNames.MCC, 10),
      new Rules(GlobalNames.MNC, 14),
      new Rules(GlobalNames.NeighbourCellBroadcast, 2),
      new Rules(GlobalNames.CellServiceLevel, 2),
      new Rules(GlobalNames.LateEntryInfo, 1)
        };
        private readonly Rules[] _dmacSyncInfoRulesDMO = new Rules[8]
        {
      new Rules(GlobalNames.Communication_type, 2),
      new Rules(GlobalNames.Master_slave_link_flag, 1),
      new Rules(GlobalNames.Gateway_generated_message_flag, 1),
      new Rules(GlobalNames.A_B_channel_usage, 2),
      new Rules(GlobalNames.TimeSlot, 2),
      new Rules(GlobalNames.Frame, 5),
      new Rules(GlobalNames.Air_interface_encryption, 2),
      new Rules(GlobalNames.Reserved, 39)
        };
        private readonly Rules[] _dpresSyncInfoRules = new Rules[21]
        {
      new Rules(GlobalNames.Repeater_Communication_type, 2),
      new Rules(GlobalNames.Repeater_M_DMO_flag, 1),
      new Rules(GlobalNames.Reserved, 2, RulesType.Switch, 241, 1),
      new Rules(GlobalNames.Repeater_Two_frequency_flag, 1, RulesType.Switch, 241, 1),
      new Rules(GlobalNames.Repeater_operating_modes, 2, RulesType.Switch, 241, 1),
      new Rules(GlobalNames.Repeater_Spacing_of_uplink, 6, RulesType.Switch, 241, 1),
      new Rules(GlobalNames.Repeater_Master_slave_link_flag, 1),
      new Rules(GlobalNames.Repeater_A_B_channel_usage, 2),
      new Rules(GlobalNames.Repeater_Channel_state, 2),
      new Rules(GlobalNames.Repeater_TimeSlot, 2),
      new Rules(GlobalNames.Repeater_Frame, 5),
      new Rules(GlobalNames.Repeater_Power_class, 3),
      new Rules(GlobalNames.Repeater_Power_control_flag, 1),
      new Rules(GlobalNames.Reserved, 1),
      new Rules(GlobalNames.Repeater_Frame_countdown, 2),
      new Rules(GlobalNames.Repeater_Priority_level, 2),
      new Rules(GlobalNames.Reserved, 6, RulesType.Switch, 241, 1),
      new Rules(GlobalNames.Values_of_DN232_DN233, 4, RulesType.Switch, 241, 1),
      new Rules(GlobalNames.Values_of_DT_254, 3, RulesType.Switch, 241, 1),
      new Rules(GlobalNames.Repeater_dual_watch_synchronization_flag, 1),
      new Rules(GlobalNames.Reserved, 5, RulesType.Switch, 241, 1)
        };
        private readonly Rules[] _dmacSyncInfoHalfSlotRules = new Rules[12]
        {
      new Rules(GlobalNames.Repeater_address, 10),
      new Rules(GlobalNames.Fill_bit, 1),
      new Rules(GlobalNames.Fragmentation_flag, 1),
      new Rules(GlobalNames.Number_of_slots, 4, RulesType.Switch, 211, 1),
      new Rules(GlobalNames.Frame_countdown, 2),
      new Rules(GlobalNames.Destination_address_type, 2),
      new Rules(GlobalNames.Destination_address, 24, RulesType.SwitchNot, 214, 2),
      new Rules(GlobalNames.Source_address_type, 2),
      new Rules(GlobalNames.Source_address, 24, RulesType.SwitchNot, 216, 2),
      new Rules(GlobalNames.MCC, 10),
      new Rules(GlobalNames.MNC, 14),
      new Rules(GlobalNames.Message_type, 5)
        };
        private readonly Rules[] _dpresSyncInfoHalfSlotRules = new Rules[40]
        {
      new Rules(GlobalNames.Repeater_Gateway_address, 10, RulesType.Switch, 241, 1),
      new Rules(GlobalNames.Repeater_MCC, 10, RulesType.Switch, 241, 1),
      new Rules(GlobalNames.Repeater_MNC, 14, RulesType.Switch, 241, 1),
      new Rules(GlobalNames.Repeater_Validity_time_unit, 2),
      new Rules(GlobalNames.Repeater_Number_of_validity_time_units, 6),
      new Rules(GlobalNames.Repeater_Maximum_DM_MS_power_class, 3),
      new Rules(GlobalNames.Reserved, 1),
      new Rules(GlobalNames.Repeater_Usage_restriction_type, 4),
      new Rules(GlobalNames.Repeater_Usage_SCKN, 5, RulesType.Switch, 260, 8),
      new Rules(GlobalNames.Repeater_Usage_SCKN, 5, RulesType.Switch, 260, 9),
      new Rules(GlobalNames.Repeater_Usage_SCKN, 5, RulesType.Switch, 260, 10),
      new Rules(GlobalNames.Repeater_Usage_SCKN, 5, RulesType.Switch, 260, 11),
      new Rules(GlobalNames.Repeater_Usage_EUIV, 19, RulesType.Switch, 260, 8),
      new Rules(GlobalNames.Repeater_Usage_EUIV, 19, RulesType.Switch, 260, 9),
      new Rules(GlobalNames.Repeater_Usage_EUIV, 19, RulesType.Switch, 260, 10),
      new Rules(GlobalNames.Repeater_Usage_EUIV, 19, RulesType.Switch, 260, 11),
      new Rules(GlobalNames.Repeater_Usage_MCC, 10, RulesType.Switch, 260, 2),
      new Rules(GlobalNames.Repeater_Usage_MNC, 14, RulesType.Switch, 260, 2),
      new Rules(GlobalNames.Repeater_Usage_MCC, 10, RulesType.Switch, 260, 3),
      new Rules(GlobalNames.Repeater_Usage_MNC, 14, RulesType.Switch, 260, 3),
      new Rules(GlobalNames.Repeater_Usage_SSI, 24, RulesType.Switch, 260, 3),
      new Rules(GlobalNames.Repeater_Usage_MCC, 10, RulesType.Switch, 260, 4),
      new Rules(GlobalNames.Repeater_Usage_MNC, 14, RulesType.Switch, 260, 4),
      new Rules(GlobalNames.Repeater_Usage_SSI, 24, RulesType.Switch, 260, 4),
      new Rules(GlobalNames.Repeater_Usage_SSI2, 24, RulesType.Switch, 260, 4),
      new Rules(GlobalNames.Repeater_Usage_MCC, 10, RulesType.Switch, 260, 5),
      new Rules(GlobalNames.Repeater_Usage_MNC, 14, RulesType.Switch, 260, 5),
      new Rules(GlobalNames.Repeater_Usage_SSI, 24, RulesType.Switch, 260, 5),
      new Rules(GlobalNames.Repeater_Usage_SSI2, 24, RulesType.Switch, 260, 5),
      new Rules(GlobalNames.Repeater_Usage_SSI, 24, RulesType.Switch, 260, 6),
      new Rules(GlobalNames.Repeater_Usage_SSI2, 24, RulesType.Switch, 260, 6),
      new Rules(GlobalNames.Repeater_Usage_SSI3, 24, RulesType.Switch, 260, 6),
      new Rules(GlobalNames.Repeater_Usage_MCC, 10, RulesType.Switch, 260, 8),
      new Rules(GlobalNames.Repeater_Usage_MNC, 14, RulesType.Switch, 260, 8),
      new Rules(GlobalNames.Repeater_Usage_MCC, 10, RulesType.Switch, 260, 9),
      new Rules(GlobalNames.Repeater_Usage_MNC, 14, RulesType.Switch, 260, 9),
      new Rules(GlobalNames.Repeater_Usage_SSI, 24, RulesType.Switch, 260, 9),
      new Rules(GlobalNames.Repeater_Usage_SSI, 24, RulesType.Switch, 260, 10),
      new Rules(GlobalNames.Repeater_Usage_SSI, 24, RulesType.Switch, 260, 11),
      new Rules(GlobalNames.Repeater_Usage_SSI2, 24, RulesType.Switch, 260, 11)
        };
        private readonly Dictionary<MacLevel.AccessAssign, int> _aachLength = new Dictionary<MacLevel.AccessAssign, int>()
    {
      {
        MacLevel.AccessAssign.Header,
        2
      },
      {
        MacLevel.AccessAssign.Field1,
        6
      },
      {
        MacLevel.AccessAssign.Field2,
        6
      }
    };
        private readonly Rules[] _sysInfoRules = new Rules[35]
        {
      new Rules(GlobalNames.Main_Carrier, 12),
      new Rules(GlobalNames.Frequency_Band, 4),
      new Rules(GlobalNames.Offset, 2),
      new Rules(GlobalNames.Duplex_Spacing, 3),
      new Rules(GlobalNames.Reverse_Operation, 1),
      new Rules(GlobalNames.NumberOfCommon_SC, 2),
      new Rules(GlobalNames.MS_TXPwr_Max_Cell, 3),
      new Rules(GlobalNames.RXLevel_Access_Min, 4),
      new Rules(GlobalNames.Access_Parameter, 4),
      new Rules(GlobalNames.Radio_Downlink_Ttimeout, 4),
      new Rules(GlobalNames.Hyperframe_or_Cipher_key_flag, 1),
      new Rules(GlobalNames.Hyperframe, 16, RulesType.Switch, 35),
      new Rules(GlobalNames.Cipher_key, 16, RulesType.Switch, 35, 1),
      new Rules(GlobalNames.Optional_field_flag, 2),
      new Rules(GlobalNames.Optional_field_value, 20, RulesType.Switch, 38),
      new Rules(GlobalNames.Optional_field_value, 20, RulesType.Switch, 38, 1),
      new Rules(GlobalNames.Optional_field_value, 20, RulesType.Switch, 38, 2),
      new Rules(GlobalNames.Authentication_required_on_cell, 1, RulesType.Switch, 38, 3),
      new Rules(GlobalNames.Security_Class_1_supported_on_cell, 1, RulesType.Switch, 38, 3),
      new Rules(GlobalNames.Security_Class_3_supported_on_cell, 1, RulesType.Switch, 38, 3),
      new Rules(GlobalNames.Optional_field_value, 17, RulesType.Switch, 38, 3),
      new Rules(GlobalNames.Location_Area, 14),
      new Rules(GlobalNames.Subscriber_Class, 16),
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
      new Rules(GlobalNames.Advanced_link_supported, 1)
        };
        private readonly Rules[] _accessDefineRules = new Rules[12]
        {
      new Rules(GlobalNames.Common_or_assigned, 1),
      new Rules(GlobalNames.Access_code, 2),
      new Rules(GlobalNames.IMM, 4),
      new Rules(GlobalNames.WT, 4),
      new Rules(GlobalNames.Nu, 4),
      new Rules(GlobalNames.Frame_length_factor, 1),
      new Rules(GlobalNames.Timeslot_pointer, 4),
      new Rules(GlobalNames.Minimum_priority, 3),
      new Rules(GlobalNames.Optional_field_flag, 2),
      new Rules(GlobalNames.Subscriber_class, 16, RulesType.Switch, 38, 1),
      new Rules(GlobalNames.GSSI, 24, RulesType.Switch, 38, 2),
      new Rules(GlobalNames.Filler_bits, 3)
        };
        private readonly Rules[] _macEndRules = new Rules[5]
        {
      new Rules(GlobalNames.Fill_bit, 1),
      new Rules(GlobalNames.Position_of_grant, 1),
      new Rules(GlobalNames.Length_indication, 6),
      new Rules(GlobalNames.Slot_granting_flag, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Slot_granting_element, 8)
        };
        private readonly Rules[] _dmacEndRules = new Rules[1]
        {
      new Rules(GlobalNames.Fill_bit, 1)
        };
        private readonly Rules[] _dmacDataRules = new Rules[13]
        {
      new Rules(GlobalNames.Fill_bit, 1),
      new Rules(GlobalNames.Second_half_slot_stolen, 1),
      new Rules(GlobalNames.Fragmentation_flag, 1),
      new Rules(GlobalNames.Null_pdu, 1),
      new Rules(GlobalNames.Frame_countdown, 2),
      new Rules(GlobalNames.Air_interface_encryption, 2),
      new Rules(GlobalNames.Destination_address_type, 2),
      new Rules(GlobalNames.Destination_address, 24, RulesType.SwitchNot, 214, 2),
      new Rules(GlobalNames.Source_address_type, 2),
      new Rules(GlobalNames.Source_address, 24, RulesType.SwitchNot, 216, 2),
      new Rules(GlobalNames.MCC, 10),
      new Rules(GlobalNames.MNC, 14),
      new Rules(GlobalNames.Message_type, 5)
        };
        private readonly Rules[] _macResourceRules = new Rules[20]
        {
      new Rules(GlobalNames.Fill_bit, 1),
      new Rules(GlobalNames.Position_of_grant, 1),
      new Rules(GlobalNames.Encryption_mode, 2),
      new Rules(GlobalNames.Random_access_flag, 1),
      new Rules(GlobalNames.Length_indication, 6),
      new Rules(GlobalNames.Address_type, 3),
      new Rules(GlobalNames.SSI, 24, RulesType.Switch, 83, 1),
      new Rules(GlobalNames.Event_Label, 10, RulesType.Switch, 83, 2),
      new Rules(GlobalNames.USSI, 24, RulesType.Switch, 83, 3),
      new Rules(GlobalNames.SMI, 24, RulesType.Switch, 83, 4),
      new Rules(GlobalNames.SSI, 24, RulesType.Switch, 83, 5),
      new Rules(GlobalNames.Event_Label, 10, RulesType.Switch, 83, 5),
      new Rules(GlobalNames.SSI, 24, RulesType.Switch, 83, 6),
      new Rules(GlobalNames.Usage, 6, RulesType.Switch, 83, 6),
      new Rules(GlobalNames.SMI, 24, RulesType.Switch, 83, 7),
      new Rules(GlobalNames.Event_Label, 10, RulesType.Switch, 83, 7),
      new Rules(GlobalNames.Power_control_flag, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Power_control_element, 4),
      new Rules(GlobalNames.Slot_granting_flag, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Slot_granting_element, 8)
        };
        private readonly Rules[] _channelAllocationRules = new Rules[30]
        {
      new Rules(GlobalNames.Channel_allocation_flag, 1, RulesType.Options_bit),
      new Rules(GlobalNames.Allocation_type, 2),
      new Rules(GlobalNames.Timeslot_assigned, 4),
      new Rules(GlobalNames.Uplink_downlink_assigned, 2),
      new Rules(GlobalNames.CLCH_permission, 1),
      new Rules(GlobalNames.Cell_change_flag, 1),
      new Rules(GlobalNames.Carrier_number, 12),
      new Rules(GlobalNames.Extended_carrier_numbering_flag, 1, RulesType.Presence_bit, 4),
      new Rules(GlobalNames.Extended_frequency_band, 4),
      new Rules(GlobalNames.Extended_offset, 2),
      new Rules(GlobalNames.Extended_duplex_spacing, 3),
      new Rules(GlobalNames.Extended_reverse_operation, 1),
      new Rules(GlobalNames.Monitoring_pattern, 2),
      new Rules(GlobalNames.Frame_18_monitoring_pattern, 2, RulesType.Switch, 106),
      new Rules(GlobalNames.Reserved, 0, RulesType.JampNot, 97, ext3: 99),
      new Rules(GlobalNames.Up_downlink_assigned_for_augmented_ch_alloc, 2),
      new Rules(GlobalNames.Bandwidth_of_allocated_channel, 3),
      new Rules(GlobalNames.Modulation_mode_of_allocated_channel, 3),
      new Rules(GlobalNames.Maximum_uplink_QAM_modulation_level, 3),
      new Rules(GlobalNames.Conforming_channel_status, 3),
      new Rules(GlobalNames.BS_link_imbalance, 4),
      new Rules(GlobalNames.BS_transmit_power_relative_to_main_carrier, 5),
      new Rules(GlobalNames.Napping_status, 2),
      new Rules(GlobalNames.Napping_information, 11, RulesType.Switch, 189, 1),
      new Rules(GlobalNames.Reserved, 4),
      new Rules(GlobalNames.Conditional_element_A_flag, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Conditional_element_A, 16),
      new Rules(GlobalNames.Conditional_element_B_flag, 1, RulesType.Presence_bit, 1),
      new Rules(GlobalNames.Conditional_element_B, 16),
      new Rules(GlobalNames.Further_augmentation_flag, 1)
        };
        private readonly Rules[] _uSignalRules = new Rules[1]
        {
      new Rules(GlobalNames.Second_half_slot_stolen, 1)
        };

        public int Field1 { get; private set; }

        public int Field2 { get; private set; }

        public bool HalfSlotStolen { get; private set; }

        public unsafe MacLevel()
        {
            for (int index = 0; index < 4; ++index)
            {
                this._fragmentsHeader[index] = new ReceivedData();
                this._tempBuffers[index] = UnsafeBuffer.Create(4096, 1);
                this._tempBuffersPtr[index] = (byte*)(void*)this._tempBuffers[index];
            }
        }

        public unsafe void AccessAsignPDU(LogicChannel channelData)
        {
            int offset1 = 0;
            int length1 = this._aachLength[MacLevel.AccessAssign.Header];
            int int32_1 = TetraUtils.BitsToInt32(channelData.Ptr, offset1, length1);
            int offset2 = offset1 + length1;
            int length2 = this._aachLength[MacLevel.AccessAssign.Field1];
            int int32_2 = TetraUtils.BitsToInt32(channelData.Ptr, offset2, length2);
            int offset3 = offset2 + length2;
            int length3 = this._aachLength[MacLevel.AccessAssign.Field2];
            int int32_3 = TetraUtils.BitsToInt32(channelData.Ptr, offset3, length3);
            this.Field1 = int32_2;
            this.Field2 = int32_3;
            if (channelData.Frame == 18)
            {
                this.DownLinkChannelType = ChannelType.Common;
            }
            else
            {
                switch (int32_1)
                {
                    case 0:
                        this.DownLinkChannelType = ChannelType.Common;
                        break;
                    case 1:
                    case 2:
                    case 3:
                        switch (int32_2)
                        {
                            case 0:
                                this.DownLinkChannelType = ChannelType.Unalloc;
                                return;
                            case 1:
                                this.DownLinkChannelType = ChannelType.Assigned;
                                return;
                            case 2:
                                this.DownLinkChannelType = ChannelType.Common;
                                return;
                            case 3:
                                this.DownLinkChannelType = ChannelType.Reserved;
                                return;
                            default:
                                this.DownLinkChannelType = ChannelType.Traffic;
                                return;
                        }
                }
            }
        }

        public void ResetAACH()
        {
            this.Field1 = 0;
            this.DownLinkChannelType = ChannelType.Common;
            this.Field2 = 0;
            this.UpLinkChannelType = ChannelType.Common;
        }

        public unsafe void UsignalPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            this.HalfSlotStolen = (uint)TetraUtils.BitsToInt32(channelData.Ptr, offset, 1) > 0U;
            ++offset;
        }

        public unsafe void SyncPDU(LogicChannel channelData, ReceivedData result)
        {
            int length1 = 4;
            int offset1 = 0;
            int int32_1 = TetraUtils.BitsToInt32(channelData.Ptr, offset1, length1);
            int offset2 = offset1 + length1;
            result.SetValue(GlobalNames.SystemCode, int32_1);
            switch (int32_1)
            {
                case 12:
                case 13:
                    int length2 = 2;
                    int int32_2 = TetraUtils.BitsToInt32(channelData.Ptr, offset2, length2);
                    int offset3 = offset2 + length2;
                    result.SetValue(GlobalNames.SYNC_PDU_type, int32_2);
                    int num;
                    if (int32_2 != 0)
                    {
                        if (int32_2 == 1)
                        {
                            num = GlobalFunction.ParseParams(channelData, offset3, this._dpresSyncInfoRules, result);
                            break;
                        }
                        result.SetValue(GlobalNames.UnknowData, 1);
                        break;
                    }
                    num = GlobalFunction.ParseParams(channelData, offset3, this._dmacSyncInfoRulesDMO, result);
                    break;
                default:
                    GlobalFunction.ParseParams(channelData, offset2, this._syncInfoRulesTMO, result);
                    break;
            }
        }

        public unsafe void SyncPDUHalfSlot(LogicChannel channelData, ReceivedData result)
        {
            int num1 = -1;
            int offset1 = 0;
            if (!result.TryGetValue(GlobalNames.SYNC_PDU_type, ref num1))
                return;
            switch (num1)
            {
                case 0:
                    int offset2 = GlobalFunction.ParseParams(channelData, offset1, this._dmacSyncInfoHalfSlotRules, result);
                    int num2 = channelData.Length - offset2;
                    if (result.Value(GlobalNames.Fill_bit) != 0)
                        num2 = this.CalcRealLength(channelData.Ptr, offset2, num2);
                    if (result.Value(GlobalNames.Fragmentation_flag) != 1)
                        break;
                    this.CreateFraqmentsBuffer(channelData, offset2, num2, result);
                    break;
                case 1:
                    GlobalFunction.ParseParams(channelData, 0, this._dpresSyncInfoHalfSlotRules, result);
                    break;
                default:
                    result.SetValue(GlobalNames.UnknowData, 1);
                    break;
            }
        }

        public unsafe int SysInfoPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            byte num = TetraUtils.BitsToByte(channelData.Ptr, offset, 2);
            result.SetValue(GlobalNames.MAC_Broadcast_Type, (int)num);
            offset += 2;
            switch (num)
            {
                case 0:
                    offset = GlobalFunction.ParseParams(channelData, offset, this._sysInfoRules, result);
                    break;
                case 1:
                    offset = GlobalFunction.ParseParams(channelData, offset, this._accessDefineRules, result);
                    break;
            }
            return offset;
        }

        public unsafe void TmoParseMacPDU(LogicChannel channelData, List<ReceivedData> result)
        {
            int num1 = 16;
            int num2 = 0;
            bool flag = false;
            for (int index = 0; index < 5; ++index)
            {
                int offset1 = num2;
                ReceivedData result1 = new ReceivedData();
                result1.Add(GlobalNames.CurrTimeSlot, channelData.TimeSlot);
                MAC_PDU_Type int32_1 = (MAC_PDU_Type)TetraUtils.BitsToInt32(channelData.Ptr, offset1, 2);
                int offset2 = offset1 + 2;
                result1.Add(GlobalNames.MAC_PDU_Type, (int)int32_1);
                switch (int32_1)
                {
                    case MAC_PDU_Type.MAC_resource:
                        this.ResourcePDU(channelData, offset2, result1);
                        num2 += result1.Value(GlobalNames.Length_indication) * 8;
                        break;
                    case MAC_PDU_Type.MAC_frag:
                        int int32_2 = TetraUtils.BitsToInt32(channelData.Ptr, offset2, 1);
                        int offset3 = offset2 + 1;
                        if (int32_2 == 0)
                        {
                            this.MacFraqPDU(channelData, offset3, result1);
                            flag = true;
                            break;
                        }
                        this.MacEndPDU(channelData, offset3, result1);
                        num2 += result1.Value(GlobalNames.Length_indication) * 8;
                        break;
                    case MAC_PDU_Type.Broadcast:
                        num2 = this.SysInfoPDU(channelData, offset2, result1);
                        break;
                    case MAC_PDU_Type.MAC_U_Signal:
                        if (channelData.Length == 124)
                        {
                            this.UsignalPDU(channelData, offset2, result1);
                            flag = true;
                            break;
                        }
                        flag = true;
                        break;
                }
                int num3 = result1.Contains(GlobalNames.Null_pdu) ? 1 : (result1.Contains(GlobalNames.OutOfBuffer) ? 1 : 0);
                if (num3 != 0)
                    flag = true;
                if (num3 == 0)
                    result.Add(result1);
                if (num2 >= channelData.Length - num1 | flag)
                    break;
            }
        }

        public unsafe void ResourcePDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            int num1 = offset - 2;
            offset = GlobalFunction.ParseParams(channelData, offset, this._macResourceRules, result);
            int num2 = result.Value(GlobalNames.Encryption_mode);
            int num3 = result.Value(GlobalNames.Length_indication);
            this.HalfSlotStolen = num3 == 62 || num3 == 63;
            if (num3 > 58 && num3 < 62 || (num3 == 0 || result.Value(GlobalNames.Address_type) == 0))
            {
                result.Add(GlobalNames.Null_pdu, 1);
            }
            else
            {
                if (num2 != 0)
                    return;
                offset = GlobalFunction.ParseParams(channelData, offset, this._channelAllocationRules, result);
                if (num3 != 63)
                {
                    this._llc.Parse(channelData, offset, result);
                }
                else
                {
                    int num4 = Math.Min(num3 * 8 - (offset - num1), channelData.Length - offset);
                    if (result.Value(GlobalNames.Fill_bit) != 0)
                        num4 = this.CalcRealLength(channelData.Ptr, offset, num4);
                    if (num4 < 0)
                        result.Add(GlobalNames.UnknowData, 1);
                    else
                        this.CreateFraqmentsBuffer(channelData, offset, num4, result);
                }
            }
        }

        public unsafe void MacEndPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            int num1 = offset - 3;
            offset = GlobalFunction.ParseParams(channelData, offset, this._macEndRules, result);
            if (this.FraqmentsBufferIsEmpty(channelData.TimeSlot))
                return;
            offset = GlobalFunction.ParseParams(channelData, offset, this._channelAllocationRules, result);
            int num2 = result.Value(GlobalNames.Length_indication);
            int num3 = Math.Min(num2 * 8 - (offset - num1), channelData.Length - offset);
            if (num2 > 58)
            {
                result.Add(GlobalNames.Null_pdu, 1);
            }
            else
            {
                if (result.Value(GlobalNames.Fill_bit) != 0)
                    num3 = this.CalcRealLength(channelData.Ptr, offset, num3);
                if (num3 < 0)
                {
                    result.Add(GlobalNames.UnknowData, 1);
                }
                else
                {
                    this.AddFraqmentsToBuffer(channelData, offset, num3);
                    LogicChannel fragmentedBuffer = this.GetDeFragmentedBuffer(channelData, result);
                    if (fragmentedBuffer.Length == 0)
                    {
                        result.Add(GlobalNames.Null_pdu, 1);
                    }
                    else
                    {
                        offset = 0;
                        this._llc.Parse(fragmentedBuffer, offset, result);
                    }
                }
            }
        }

        public unsafe void MacFraqPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            if (this.FraqmentsBufferIsEmpty(channelData.TimeSlot))
                return;
            int num1 = (uint)TetraUtils.BitsToInt32(channelData.Ptr, offset, 1) > 0U ? 1 : 0;
            ++offset;
            int num2 = channelData.Length - offset;
            if (num1 != 0)
                num2 = this.CalcRealLength(channelData.Ptr, offset, num2);
            if (num2 < 0)
                result.Add(GlobalNames.UnknowData, 1);
            else
                this.AddFraqmentsToBuffer(channelData, offset, num2);
        }

        public unsafe void DmoParseMacPDU(LogicChannel channelData, List<ReceivedData> result)
        {
            int offset1 = 0;
            ReceivedData result1 = new ReceivedData();
            result1.Add(GlobalNames.CurrTimeSlot, channelData.TimeSlot);
            MAC_PDU_Type int32_1 = (MAC_PDU_Type)TetraUtils.BitsToInt32(channelData.Ptr, offset1, 2);
            int offset2 = offset1 + 2;
            result1.Add(GlobalNames.MAC_PDU_Type, (int)int32_1);
            switch (int32_1)
            {
                case MAC_PDU_Type.MAC_resource:
                    this.DmacDataPDU(channelData, offset2, result1);
                    break;
                case MAC_PDU_Type.MAC_frag:
                    int int32_2 = TetraUtils.BitsToInt32(channelData.Ptr, offset2, 1);
                    int offset3 = offset2 + 1;
                    if (int32_2 == 0)
                    {
                        this.DmacFraqPDU(channelData, offset3, result1);
                        break;
                    }
                    this.DmacEndPDU(channelData, offset3, result1);
                    break;
            }
            result.Add(result1);
        }

        public unsafe void DmacDataPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            offset = GlobalFunction.ParseParams(channelData, offset, this._dmacDataRules, result);
            int num = channelData.Length - offset;
            if (result.Value(GlobalNames.Fill_bit) != 0)
                num = this.CalcRealLength(channelData.Ptr, offset, num);
            this.HalfSlotStolen = result.Value(GlobalNames.Second_half_slot_stolen) == 1;
            if (result.Value(GlobalNames.Fragmentation_flag) == 1)
                this.CreateFraqmentsBuffer(channelData, offset, num, result);
            else
                result.Value(GlobalNames.Null_pdu);
        }

        public unsafe void DmacEndPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            offset = GlobalFunction.ParseParams(channelData, offset, this._dmacEndRules, result);
            int num = channelData.Length - offset;
            if (result.Value(GlobalNames.Fill_bit) != 0)
                num = this.CalcRealLength(channelData.Ptr, offset, num);
            this.AddFraqmentsToBuffer(channelData, offset, num);
            LogicChannel fragmentedBuffer = this.GetDeFragmentedBuffer(channelData, result);
            offset = 0;
            int length = fragmentedBuffer.Length;
        }

        public unsafe void DmacFraqPDU(LogicChannel channelData, int offset, ReceivedData result)
        {
            int num1 = (uint)TetraUtils.BitsToInt32(channelData.Ptr, offset, 1) > 0U ? 1 : 0;
            ++offset;
            int num2 = channelData.Length - offset;
            if (num1 != 0)
                num2 = this.CalcRealLength(channelData.Ptr, offset, num2);
            this.AddFraqmentsToBuffer(channelData, offset, num2);
        }

        private unsafe int CalcRealLength(byte* buffer, int offset, int currentLength)
        {
            int index = offset + currentLength - 1;
            while (index > offset && buffer[index] == (byte)0)
                --index;
            return index - offset;
        }

        private unsafe void CreateFraqmentsBuffer(
          LogicChannel buffer,
          int offset,
          int length,
          ReceivedData header)
        {
            int index1 = buffer.TimeSlot - 1;
            if (this._writeAddress[index1] + length >= this._tempBuffers[index1].Length)
                return;
            Utils.Memcpy((void*)this._tempBuffersPtr[index1], (void*)(buffer.Ptr + offset), length);
            this._writeAddress[index1] = length;
            if (header == null)
                return;
            for (int index2 = 0; index2 < header.Data.Length; ++index2)
                this._fragmentsHeader[index1].Data[index2] = header.Data[index2];
        }

        private unsafe void AddFraqmentsToBuffer(LogicChannel buffer, int offset, int length)
        {
            int index = buffer.TimeSlot - 1;
            if (this._writeAddress[index] == 0 || this._writeAddress[index] + length >= this._tempBuffers[index].Length)
                return;
            Utils.Memcpy((void*)(this._tempBuffersPtr[index] + this._writeAddress[index]), (void*)(buffer.Ptr + offset), length);
            this._writeAddress[index] += length;
        }

        private bool FraqmentsBufferIsEmpty(int timeSlot) => this._writeAddress[timeSlot - 1] == 0;

        private unsafe LogicChannel GetDeFragmentedBuffer(
          LogicChannel channelData,
          ReceivedData header)
        {
            int index = channelData.TimeSlot - 1;
            LogicChannel logicChannel = new LogicChannel();
            logicChannel.Ptr = this._tempBuffersPtr[index];
            logicChannel.Length = this._writeAddress[index];
            logicChannel.CrcIsOk = true;
            logicChannel.TimeSlot = channelData.TimeSlot;
            logicChannel.Frame = channelData.Frame;
            GlobalNames name1 = GlobalNames.Encryption_mode;
            if (this._fragmentsHeader[index].Contains(name1))
                header.Add(name1, this._fragmentsHeader[index].Value(name1));
            GlobalNames name2 = GlobalNames.Address_type;
            if (this._fragmentsHeader[index].Contains(name2))
                header.Add(name2, this._fragmentsHeader[index].Value(name2));
            GlobalNames name3 = GlobalNames.SSI;
            if (this._fragmentsHeader[index].Contains(name3))
                header.Add(name3, this._fragmentsHeader[index].Value(name3));
            this._fragmentsHeader[index].Clear();
            this._writeAddress[index] = 0;
            return logicChannel;
        }

        public enum AccessAssign
        {
            Header,
            Field1,
            Field2,
        }
    }
}
