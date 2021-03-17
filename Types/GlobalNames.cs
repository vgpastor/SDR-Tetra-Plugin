﻿// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.GlobalNames
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
    public enum GlobalNames
    {
        Data_Separator,
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
        CurrTimeSlot,
        A_B_channel_usage,
        Gateway_generated_message_flag,
        Master_slave_link_flag,
        Communication_type,
        SYNC_PDU_type,
        Repeater_Gateway_address,
        Fragmentation_flag,
        Number_of_slots,
        Frame_countdown,
        Destination_address_type,
        Destination_address,
        Source_address_type,
        Source_address,
        Mobile_Network_Identity,
        M_DMO_flag,
        Two_frequency_repeater_flag,
        Repeater_operating_modes,
        Spacing_of_uplink,
        Channel_state,
        Power_class,
        Priority_level,
        Values_of_DN232_DN233,
        Values_of_DT_254,
        Presence_signal_dual_watch_synchronization_flag,
        Validity_time_unit,
        Number_of_validity_time_units,
        Maximum_DM_MS_power_class,
        Usage_restriction_type,
        SCKN,
        EUIV,
        MCC_Rep,
        MNC_Rep,
        SSI3,
        SSI2,
        Null_pdu,
        Repeater_address,
        Repeater_Communication_type,
        Repeater_M_DMO_flag,
        Repeater_Two_frequency_flag,
        Repeater_Spacing_of_uplink,
        Repeater_Master_slave_link_flag,
        Repeater_A_B_channel_usage,
        Repeater_Channel_state,
        Repeater_TimeSlot,
        Repeater_Frame,
        Repeater_Power_class,
        Repeater_Power_control_flag,
        Repeater_Frame_countdown,
        Repeater_Priority_level,
        Repeater_dual_watch_synchronization_flag,
        Repeater_MCC,
        Repeater_MNC,
        Repeater_Validity_time_unit,
        Repeater_Number_of_validity_time_units,
        Repeater_Maximum_DM_MS_power_class,
        Repeater_Usage_restriction_type,
        Repeater_Usage_SCKN,
        Repeater_Usage_EUIV,
        Repeater_Usage_MCC,
        Repeater_Usage_MNC,
        Repeater_Usage_SSI,
        Repeater_Usage_SSI2,
        Repeater_Usage_SSI3,
        OutOfBuffer,
        UnknowData,
        SDS_TL_MesaggeType,
        Service_form_report,
        Acknowledgement_required,
        Delivery_status,
        Location_System_Coding,
        Request_response,
        Report_type,
        T5_el_ident,
        T5_el_length,
        T5_el_length_ex,
        End,
    }
}