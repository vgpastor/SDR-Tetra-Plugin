using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDRSharp.Tetra
{
    public enum GlobalNames
    {
        Data_Separator = 0,
        Call_identifier,
        Disconnect_cause,
        Options_bit,
        Presence_bit,
        Notification_indicator,
        More_bit,
        Transmission_request_permission,
        Transmission_grant,
        Encryption_control,
        Reserved,
        Transmitting_party_type_identifier,
        Transmitting_party_address_SSI,
        Transmitting_party_extension,
        Call_time_out,
        Hook_method,
        Simplex_duplex,
        Call_priority,
        Temporary_address,
        Calling_party_type_identifier,
        Calling_party_address_SSI,
        Calling_party_extension,
        Call_ownership,
        MAC_PDU_Type,
        MAC_Broadcast_Type,
        Main_Carrier,
        Frequency_Band,
        Offset,
        Duplex_Spacing,
        Reverse_Operation,
        NumberOfCommon_SC,
        MS_TXPwr_Max_Cell,
        RXLevel_Access_Min,
        Access_Parameter,
        Radio_Downlink_Ttimeout,
        Hyperframe_or_Cipher_key_flag,
        Hyperframe,
        Cipher_key,
        Optional_field_flag,
        Optional_field_value,
        Location_Area,
        Subscriber_Class,
        Registration_required,
        De_registration_required,
        Priority_cell,
        Cell_never_uses_minimum_mode,
        Migration_supported,
        System_wide_services,
        TETRA_voice_service,
        Circuit_mode_data_service,
        SNDCP_Service,
        Air_interface_encryption,
        Advanced_link_supported,
        Common_or_assigned,
        Access_code,
        IMM,
        WT,
        Nu,
        Frame_length_factor,
        Timeslot_pointer,
        Minimum_priority,
        Subscriber_class,
        GSSI,
        Filler_bits,
        SystemCode,
        ColorCode,
        TimeSlot,
        Frame,
        MultiFrame,
        SharingMode,
        TSReservedFrames,
        UPlaneDTX,
        Frame18Extension,
        MCC,
        MNC,
        NeighbourCellBroadcast,
        CellServiceLevel,
        LateEntryInfo,
        Fill_bit,
        Position_of_grant,
        Encryption_mode,
        Random_access_flag,
        Length_indication,
        Address_type,
        SSI,
        USSI,
        SMI,
        Usage,
        Event_Label,
        Power_control_flag,
        Power_control_element,
        Slot_granting_flag,
        Slot_granting_element,
        Channel_allocation_flag,
        Channel_allocation_element,
        Allocation_type,
        Timeslot_assigned,
        Uplink_downlink_assigned,
        CLCH_permission,
        Cell_change_flag,
        Carrier_number,
        Extended_carrier_numbering_flag,
        Extended_frequency_band,
        Extended_offset,
        Extended_duplex_spacing,
        Extended_reverse_operation,
        Monitoring_pattern,
        Frame_18_monitoring_pattern,
        MLE_PDU_Type,
        LLC_Pdu_Type,
        CMCE_Primitives_Type,
        MLE_Primitives_Type,
        Cell_reselect_parameters,
        Cell_service_level,
        Network_time,
        Local_time_offset_sign,
        Local_time_offset,
        Number_of_Neighbour_cells_element,
        Cell_identifier,
        Cell_reselection_types_supported,
        Neighbour_cell_synchronised,
        Main_carrier_number,
        Neighbour_cell_service_level,
        Main_carrier_number_extension,
        Neighbour_MCC,
        Neighbour_MNC,
        Neighbour_LA,
        EndOfData,
        TDMA_frame_offset,
        Timeshare_cell_and_AI_encryption,
        Minimum_RX_access_level,
        Maximum_MS_transmit_power,
        Basic_service_Circuit_mode_type,
        Basic_service_Encryption_flag,
        Basic_service_Communication_type,
        Basic_service_Slots_per_frame,
        Basic_service_Speech_service,
        Short_data_type_identifier,
        User_Defined_Data,
        User_Defined_Data4_Length,
        Reset_Call_time_out,
        Poll_request,
        New_Call_Identifier,
        Call_time_out_setup_phase,
        Modify,
        Call_status,
        Poll_response_percentage,
        Poll_response_number,
        User_Defined_Data_16,
        User_Defined_Data_32,
        User_Defined_Data_64_1,
        User_Defined_Data_64_2,
        Protocol_identifier,
        Message_type,
        Delivery_report_request,
        Service_selection,
        Storage_forward_control,
        Message_reference,
        Validity_period,
        Forward_address_type,
        Forward_short_address,
        Forward_address_SSI,
        Forward_address_extension,
        Number_subscriber_number_digits,
        External_subscriber_number_digit,
        Dummy_digit,
        User_data,
        Location_PDU_type,
        Time_elapsed,
        Longitude,
        Latitude,
        Position_error,
        Horizontal_velocity,
        Direction_of_travel,
        Reason_for_sending,
        User_defined_data,
        Time_data,
        Location_PDU_type_extension,
        Second_half_slot_stolen,
        Authentication_required_on_cell,
        Security_Class_1_supported_on_cell,
        Security_Class_3_supported_on_cell,
        Up_downlink_assigned_for_augmented_ch_alloc,
        Bandwidth_of_allocated_channel,
        Modulation_mode_of_allocated,
        Modulation_mode_of_allocated_channel,
        Conforming_channel_status,
        BS_link_imbalance,
        BS_transmit_power_relative_to_main_carrier,
        Napping_status,
        Maximum_uplink_QAM_modulation_level,
        Napping_information,
        Conditional_element_A_flag,
        Conditional_element_A,
        Conditional_element_B,
        Conditional_element_B_flag,
        Further_augmentation_flag,
        Text_coding_scheme,
        Time_stamp_used,
        Timeframe_type,
        Month,
        Day,
        Hour,
        Minute,
        CurrTimeSlot
    }

    public enum EndOfData
    {
        NullPDU = 0,
        FraqStart,
        FraqEnd,
        Unknow,
        NoMoreData,
        NeedFix
    }

    public enum MlePrimitivesType
    {
        D_NEW_CELL = 0,
        D_PREPARE,
        D_NWRK_BROADCAST,
        Reserved1,
        D_RESTORE_ACK,
        D_RESTORE_FAIL,
        Reserved2,
        Reserved3
    }

    public enum CmcePrimitivesType
    {
        D_Alert = 0,
        D_Call_Proceeding,
        D_Connect,
        D_Connect_Acknowledgea,
        D_Disconnect,
        D_Info,
        D_Release,
        D_Setup,
        D_Status,
        D_TX_Ceased,
        D_TX_Continue,
        D_TX_Granted,
        D_TX_Wait,
        D_TX_Interrupt,
        D_Call_Restore,
        D_SDS_Data,
        D_Facility,
        Reserved17,
        Reserved18,
        Reserved19,
        Reserved20,
        Reserved21,
        Reserved22,
        Reserved23,
        Reserved24,
        Reserved25,
        Reserved26,
        Reserved27,
        Reserved28,
        Reserved29,
        Reserved30,
        CMCE_Function_Not_Supported
    }

    public enum CircuitModeType
    {
        Speech_TCH_S = 0,
        Unprotect_72,
        Low_Protect_48_1,
        Low_Protect_48_4,
        Low_Protect_48_8,
        High_Protect_24_1,
        High_Protect_24_4,
        High_Protect_24_8
    }

    public enum CommunicationType
    {
        Individual = 0,
        Group_call,
        Point_to_multipoint_Acknowledged,
        Broadcast
    }

    public enum TransmissionGranted
    {
        Granted = 0,
        Not_granted,
        Request_queued,
        Granted_to_another_user
    }

    public enum MLEPduType
    {
        Reserved1 = 0,
        MM,
        CMCE,
        Reserved2,
        SNDCP,
        MLE,
        TETRA_management_entity,
        Testing
    }

    public enum LLCPduType
    {
        BL_ADATA = 0,
        BL_DATA,
        BL_UDATA,
        BL_ACK,
        BL_ADATA_FCS,
        BL_DATA_FCS,
        BL_UDATA_FCS,
        BL_ACK_FCS,
        AL_SETUP,
        AL_DATA_AR_FINAL,
        AL_UDATA_UFINAL,
        AL_ACK_RNR,
        AL_RECONNECT,
        Reserved1,
        Reserved2,
        AL_DISC
    }

    public enum MAC_PDU_Type
    {
        MAC_resource = 0,
        MAC_frag,
        Broadcast,
        MAC_U_Signal
    };

    public enum ChannelType
    {
        Unalloc = 0,
        Common,
        Assigned,
        Reserved,
        Traffic
    }

    public enum SdsProtocolIdent
    {
        Reserved_0 = 0,
        OTAK = 1,
        Simple_text_msg = 2,
        Simple_location_system = 3,
        Wireless_datagram = 4,
        Wireless_control_msg = 5,
        Managed_DMO = 6,
        PIN_authentication = 7,
        End_to_end_encrypted_msg = 8,
        Simple_immediate_text = 9,
        Location_information = 10,
        Net_Assist_2 = 11,
        Concatenated_SDS_msg = 12,
        DOTAM = 13,
        Simple_AGNSS_service = 14,
        Reserved_15,
        Reserved_16,
        Reserved_17,
        Reserved_18,
        Reserved_19,
        Reserved_20,
        Reserved_21,
        Reserved_22,
        Reserved_23,
        Reserved_24,
        Reserved_25,
        Reserved_26,
        Reserved_27,
        Reserved_28,
        Reserved_29,
        Reserved_30,
        Reserved_31,
        Reserved_32,
        Reserved_33,
        Reserved_34,
        Reserved_35,
        Reserved_36,
        Reserved_37,
        Reserved_38,
        Reserved_39,
        Reserved_40,
        Reserved_41,
        Reserved_42,
        Reserved_43,
        Reserved_44,
        Reserved_45,
        Reserved_46,
        Reserved_47,
        Reserved_48,
        Reserved_49,
        Reserved_50,
        Reserved_51,
        Reserved_52,
        Reserved_53,
        Reserved_54,
        Reserved_55,
        Reserved_56,
        Reserved_57,
        Reserved_58,
        Reserved_59,
        Reserved_60,
        Reserved_61,
        Reserved_62,
        Reserved_63,
        User_Defined_64,
        User_Defined_65,
        User_Defined_66,
        User_Defined_67,
        User_Defined_68,
        User_Defined_69,
        User_Defined_70,
        User_Defined_71,
        User_Defined_72,
        User_Defined_73,
        User_Defined_74,
        User_Defined_75,
        User_Defined_76,
        User_Defined_77,
        User_Defined_78,
        User_Defined_79,
        User_Defined_80,
        User_Defined_81,
        User_Defined_82,
        User_Defined_83,
        User_Defined_84,
        User_Defined_85,
        User_Defined_86,
        User_Defined_87,
        User_Defined_88,
        User_Defined_89,
        User_Defined_90,
        User_Defined_91,
        User_Defined_92,
        User_Defined_93,
        User_Defined_94,
        User_Defined_95,
        User_Defined_96,
        User_Defined_97,
        User_Defined_98,
        User_Defined_99,
        User_Defined_100,
        User_Defined_101,
        User_Defined_102,
        User_Defined_103,
        User_Defined_104,
        User_Defined_105,
        User_Defined_106,
        User_Defined_107,
        User_Defined_108,
        User_Defined_109,
        User_Defined_110,
        User_Defined_111,
        User_Defined_112,
        User_Defined_113,
        User_Defined_114,
        User_Defined_115,
        User_Defined_116,
        User_Defined_117,
        User_Defined_118,
        User_Defined_119,
        User_Defined_120,
        User_Defined_121,
        User_Defined_122,
        User_Defined_123,
        User_Defined_124,
        User_Defined_125,
        User_Defined_126,
        Reserved_127,
        Reserved_128,
        Reserved_129,
        Text_Messaging_TL = 130,
        Location_System_TL = 131,
        Wireless_Datagram_TL = 132,
        Wireless_Control_Message_TL = 133,
        Managed_DMO_Message_TL = 134,
        Reserved_135 = 135,
        End_to_end_encrypted_message_TL = 136,
        Immediate_text_messaging_TL = 137,
        Message_with_User_Data_Header_TL = 138,
        Reserved_139 = 139,
        Concatenated_SDS_message_TL = 140,
        AGNSS_service_TL = 141,
        Reserved_142,
        Reserved_143,
        Reserved_144,
        Reserved_145,
        Reserved_146,
        Reserved_147,
        Reserved_148,
        Reserved_149,
        Reserved_150,
        Reserved_151,
        Reserved_152,
        Reserved_153,
        Reserved_154,
        Reserved_155,
        Reserved_156,
        Reserved_157,
        Reserved_158,
        Reserved_159,
        Reserved_160,
        Reserved_161,
        Reserved_162,
        Reserved_163,
        Reserved_164,
        Reserved_165,
        Reserved_166,
        Reserved_167,
        Reserved_168,
        Reserved_169,
        Reserved_170,
        Reserved_171,
        Reserved_172,
        Reserved_173,
        Reserved_174,
        Reserved_175,
        Reserved_176,
        Reserved_177,
        Reserved_178,
        Reserved_179,
        Reserved_180,
        Reserved_181,
        Reserved_182,
        Reserved_183,
        Reserved_184,
        Reserved_185,
        Reserved_186,
        Reserved_187,
        Reserved_188,
        Reserved_189,
        Reserved_190,
        Reserved_191,
        User_Defined_192,
        User_Defined_193,
        User_Defined_194,
        User_Defined_195,
        User_Defined_196,
        User_Defined_197,
        User_Defined_198,
        User_Defined_199,
        User_Defined_200,
        User_Defined_201,
        User_Defined_202,
        User_Defined_203,
        User_Defined_204,
        User_Defined_205,
        User_Defined_206,
        User_Defined_207,
        User_Defined_208,
        User_Defined_209,
        User_Defined_210,
        User_Defined_211,
        User_Defined_212,
        User_Defined_213,
        User_Defined_214,
        User_Defined_215,
        User_Defined_216,
        User_Defined_217,
        User_Defined_218,
        User_Defined_219,
        User_Defined_220,
        User_Defined_221,
        User_Defined_222,
        User_Defined_223,
        User_Defined_224,
        User_Defined_225,
        User_Defined_226,
        User_Defined_227,
        User_Defined_228,
        User_Defined_229,
        User_Defined_230,
        User_Defined_231,
        User_Defined_232,
        User_Defined_233,
        User_Defined_234,
        User_Defined_235,
        User_Defined_236,
        User_Defined_237,
        User_Defined_238,
        User_Defined_239,
        User_Defined_240,
        User_Defined_241,
        User_Defined_242,
        User_Defined_243,
        User_Defined_244,
        User_Defined_245,
        User_Defined_246,
        User_Defined_247,
        User_Defined_248,
        User_Defined_249,
        User_Defined_250,
        User_Defined_251,
        User_Defined_252,
        User_Defined_253,
        User_Defined_254,
        Reserved_255
    }

    public enum LocationTypeExtension
    {
        Reserved_0 = 0,
        Immediate_location_report_request,
        Reserved_2,
        Long_location_report,
        Location_report_acknowledgement,
        Basic_location_parameters_request_response,
        Add_modify_trigger_request_response,
        Remove_trigger_request_response,
        Report_trigger_request_response,
        Report_basic_location_parameters_request_response,
        Location_reporting_enable_disable_request_response,
        Location_reporting_temporary_control_request_response,
        Backlog_request_response,
        Reserved_13,
        Reserved_14,
        Reserved_15
    }

    public class NetworkEntry
    {
        public Dictionary<int, GroupsEntry> KnowGroups { get; set; }
    }

    public struct GroupsEntry
    {
        public string Name { get; set; }
        public int Priority{ get; set; }
    }

    unsafe static class GlobalFunction
    {
        public static bool IgnoreEncryptedData;
        public static bool IgnoreEncryptedSpeech;

        public static List<Dictionary<GlobalNames, int>> NeighbourList = new List<Dictionary<GlobalNames, int>>();
        private static int _currentBand;
        private static int _currentOffset;
        
        public static int ParseParams(LogicChannel channelData, int offset, Rules[] rules, Dictionary<GlobalNames, int> result)
        {

            var skipRules = 0;

            for (int i = 0; i < rules.Length; i++)
            {
                if (offset >= channelData.Length)
                {
                    return offset;
                }

                if (skipRules > 0)
                {
                    skipRules--;
                    continue;
                }

                var param = rules[i];
                var value = TetraUtils.BitsToInt32(channelData.Ptr, offset, param.Length);

                switch (param.Type)
                {
                    case RulesType.Direct:
                        result.Add(param.GlobalName, value);
                        offset += param.Length;
                        continue;

                    case RulesType.Options_bit:
                        offset += param.Length;
                        if (value == 0)
                        {
                            return offset;
                        }
                        break;

                    case RulesType.Presence_bit:
                        if (value == 0)
                        {
                            skipRules = param.Ext1;
                        }
                        offset += param.Length;
                        break;

                    case RulesType.More_bit:
                        offset += param.Length;
                        return offset;

                    case RulesType.Switch:
                        if (result[(GlobalNames)rules[i].Ext1] == rules[i].Ext2)
                        {
                            result.Add(param.GlobalName, value);
                            offset += param.Length;
                        }
                        break;

                    case RulesType.SwitchNot:
                        if (result[(GlobalNames)rules[i].Ext1] != rules[i].Ext2)
                        {
                            result.Add(param.GlobalName, value);
                            offset += param.Length;
                        }
                        break;

                    case RulesType.Jamp:
                        if (result[(GlobalNames)rules[i].Ext1] == rules[i].Ext2)
                        {
                            skipRules = param.Ext3;
                        }
                        break;

                    case RulesType.JampNot:
                        if (result[(GlobalNames)rules[i].Ext1] != rules[i].Ext2)
                        {
                            skipRules = param.Ext3;
                        }
                        break;

                    case RulesType.Reserved:
                        {
                            offset += param.Length;
                        }
                        break;
                }
            }

            return offset;
        }

        public static long FrequencyCalc(bool isFull, int carrier, int band = 0, int offset = 0)
        {
            if (isFull)
            {
                _currentBand = band;
                _currentOffset = offset;
            }

            var freq = _currentBand * 100000000L + carrier * 25000L;
            switch (_currentOffset)
            {
                case 0:
                    break;
                case 1:
                    freq += 6250;
                    break;

                case 2:
                    freq -= 6250;
                    break;

                case 3:
                    freq += 12500;
                    break;
            }

            return freq;

        }

        public static int CarrierCalc(long frequency)
        {
            switch (_currentOffset)
            {
                case 0:
                    break;
                case 1:
                    frequency -= 6250;
                    break;

                case 2:
                    frequency += 6250;
                    break;

                case 3:
                    frequency -= 12500;
                    break;
            } 

            return (int)Math.Round((frequency - _currentBand * 100000000L) / 25000m);
        }
    }
}
