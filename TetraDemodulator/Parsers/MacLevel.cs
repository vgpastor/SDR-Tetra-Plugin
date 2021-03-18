using SDRSharp.Radio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SDRSharp.Tetra
{
    unsafe class MacLevel
    {
        private LlcLevel _llc = new LlcLevel();

        public ChannelType DownLinkChannelType;
        public ChannelType UpLinkChannelType;

        private UnsafeBuffer[] _tempBuffers = new UnsafeBuffer[4];
        private byte*[] _tempBuffersPtr = new byte*[4];


        public int Field1 { get; private set; }
        public int Field2 { get; private set; }
        public bool HalfSlotStolen { get; private set; }

        public MacLevel()
        {
            for (int i = 0; i < 4; i++ )
            {
                _tempBuffers[i] = UnsafeBuffer.Create(4096, sizeof(byte));
                _tempBuffersPtr[i] = (byte*)_tempBuffers[i];
            }
        }

        private readonly Rules[] _syncInfoRules = new Rules[]
        {
            new Rules(GlobalNames.SystemCode, 4 ),
            new Rules(GlobalNames.ColorCode, 6 ),
            new Rules(GlobalNames.TimeSlot, 2 ),
            new Rules(GlobalNames.Frame, 5 ),
            new Rules(GlobalNames.MultiFrame, 6 ),
            new Rules(GlobalNames.SharingMode, 2 ),
            new Rules(GlobalNames.TSReservedFrames, 3 ),
            new Rules(GlobalNames.UPlaneDTX, 1 ),
            new Rules(GlobalNames.Frame18Extension, 1 ),
            new Rules(GlobalNames.Reserved, 1, RulesType.Reserved ),
            new Rules(GlobalNames.MCC, 10 ),
            new Rules(GlobalNames.MNC, 14 ),
            new Rules(GlobalNames.NeighbourCellBroadcast, 2 ),
            new Rules(GlobalNames.CellServiceLevel, 2 ),
            new Rules(GlobalNames.LateEntryInfo, 1 )
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
            new Rules(GlobalNames.Reserved, 1, RulesType.Reserved ),
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

        public void SyncPDU(LogicChannel channelData, Dictionary<GlobalNames, int> result)
        {
            GlobalFunction.ParseParams(channelData, 0, _syncInfoRules, result);
        }

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

        public int SysInfoPDU(LogicChannel channelData, int offset, Dictionary<GlobalNames, int> result)
        {
            var subType = TetraUtils.BitsToByte(channelData.Ptr, offset, 2);
            result.Add(GlobalNames.MAC_Broadcast_Type, (int)subType);
            offset += 2;

            switch (subType)
            {
                case 1:
                    Debug.Write(" AcсessDef PDU");
                    offset = GlobalFunction.ParseParams(channelData, offset, _accessDefineRules, result);
                    break;

                case 0:
                    Debug.Write(" Broadcast PDU");
                    offset = GlobalFunction.ParseParams(channelData, offset, _sysInfoRules, result);
                    break;
            }

            return offset;
        }


        public void ParseMacPDU(LogicChannel channelData, List<Dictionary<GlobalNames, int>> result)
        {
            var nullPduLength = 16; // only for receive pi/4 modulation ;
            var blockOffset = 0;
            var nullPdu = false;
            var pduEnd = false;

            while ((blockOffset < (channelData.Length - nullPduLength)) && !pduEnd)
            {
                var offset = blockOffset;

                var pdu = new Dictionary<GlobalNames, int>();

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
                        blockOffset += (pdu[GlobalNames.Length_indication] * 8);
                        nullPdu = (pdu[GlobalNames.Address_type] == 0);
                        pduEnd = nullPdu;
                        break;

                    case MAC_PDU_Type.MAC_U_Signal:
                        if (channelData.Length == 124)
                        {
                            UsignalPDU(channelData, offset, pdu);
                            blockOffset += channelData.Length;
                            pduEnd = true;
                        }
                        else
                        {
                            Debug.Write(" MAC-D-BLCK PDU");
                            pduEnd = true;
                        }
                        break;

                    case MAC_PDU_Type.MAC_frag:

                        var isTheEnd = TetraUtils.BitsToInt32(channelData.Ptr, offset, 1);
                        offset++;

                        if (isTheEnd == 0)
                        {
                            MacFraqPDU(channelData, offset, pdu);
                            blockOffset += channelData.Length;
                            pduEnd = true;
                        }
                        else
                        {
                            MacEndPDU(channelData, offset, pdu);
                            blockOffset += (pdu[GlobalNames.Length_indication] * 8);
                        }
                        break;
                }

                if (!nullPdu) result.Add(pdu);
            }
        }
    

        private readonly Rules[] _macEndRules = new Rules[]
        {
            new Rules(GlobalNames.Fill_bit, 1 ),
            new Rules(GlobalNames.Position_of_grant, 1 ),
            new Rules(GlobalNames.Length_indication, 6 ),
            new Rules(GlobalNames.Slot_granting_flag, 1, RulesType.Presence_bit, 1 ),
            new Rules(GlobalNames.Slot_granting_element, 8 )   
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
            
            new Rules(GlobalNames.Reserved, 0, RulesType.JampNot, (int)GlobalNames.Uplink_downlink_assigned, 0, 18), 
            //Перескакиваем в конец если не 0
            
            new Rules(GlobalNames.Up_downlink_assigned_for_augmented_ch_alloc, 2),
            new Rules(GlobalNames.Bandwidth_of_allocated_channel, 3),
            new Rules(GlobalNames.Modulation_mode_of_allocated_channel, 3),
            new Rules(GlobalNames.Maximum_uplink_QAM_modulation_level, 3),
            new Rules(GlobalNames.Conforming_channel_status, 3),
            new Rules(GlobalNames.BS_link_imbalance, 4),
            new Rules(GlobalNames.BS_transmit_power_relative_to_main_carrier, 5),
            new Rules(GlobalNames.Napping_status, 2),
            new Rules(GlobalNames.Napping_information, 11, RulesType.Switch, (int)GlobalNames.Napping_status, 1),
            new Rules(GlobalNames.Reserved, 4, RulesType.Reserved),
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

        private int[] _writeAddress = new int[4];
        private int[] _fragmentsEncription = new int[4];

        public void UsignalPDU(LogicChannel channelData, int offset, Dictionary<GlobalNames, int> result)
        {
            HalfSlotStolen = (TetraUtils.BitsToInt32(channelData.Ptr, offset, 1) != 0);
            offset++;

            Debug.Write(" Usig PDU");
            
            //Possible pdu encrypted        
            if (!GlobalFunction.IgnoreEncryptedData) _llc.Parse(channelData, offset, result);
        }

        public void ResourcePDU(LogicChannel channelData, int offset, Dictionary<GlobalNames, int> result)
        {
            var startOffset = offset - 2;

            offset = GlobalFunction.ParseParams(channelData, offset, _macResourceRules, result);

            offset = GlobalFunction.ParseParams(channelData, offset, _channelAllocationRules, result);

            var length = result[GlobalNames.Length_indication]; //Only for pi/4 modulation
            var realLength = length * 8;
            var fill = result[GlobalNames.Fill_bit] != 0;
            var encr = 0;

            if (fill)
            {
                var endPos = Math.Min(startOffset + realLength, channelData.Length);
                realLength = CalcRealLength(channelData.Ptr, endPos, offset);
            }

            result.TryGetValue(GlobalNames.Encryption_mode, out encr);

            Debug.WriteIf(encr != 0, " Encrypted");

            HalfSlotStolen = (length == 62) || (length == 63);
                
            Debug.WriteIf(((length < 62) && (length > 58)), " Incorrect PDU length");
           
            if (length == 62)
            {
                Debug.Write(" Second slot stolen");
            }
            else if (length == 63)
            {
                Debug.Write(" Start fragmented PDU");
                CreateFraqmentsBuffer(channelData, offset, realLength, encr);

                return;
            }

            if (result[GlobalNames.Address_type] == 0)
            {
                Debug.Write(" Null PDU");
                return;
            }

            Debug.Write(" Resource pdu");

            if (GlobalFunction.IgnoreEncryptedData && (encr != 0)) return;

            _llc.Parse(channelData, offset, result);
        }

        private int CalcRealLength(byte* buffer, int length, int startPos)
        {
            var pos = length - 1;
            while (pos > startPos && buffer[pos] == 0) pos--;
            return pos - startPos;
        }

        public void MacEndPDU(LogicChannel channelData, int offset, Dictionary<GlobalNames, int> result)
        {
            var startOffset = offset - 3;

            offset = GlobalFunction.ParseParams(channelData, offset, _macEndRules, result);
            offset = GlobalFunction.ParseParams(channelData, offset, _channelAllocationRules, result);

            var length = result[GlobalNames.Length_indication];
            var realLength = length * 8;
            var fill = result[GlobalNames.Fill_bit] != 0;

            Debug.WriteIf(length > 58," Incorrect PDU length");
            
            if (fill)
            {
                var endPos = Math.Min(startOffset + realLength, channelData.Length);
                realLength = CalcRealLength(channelData.Ptr, endPos, offset);
            }
            
            Debug.Write(" END fragmented PDU");

            AddFraqmentsToBuffer(channelData, offset, length);

            var newBuffer = GetDeFragmentedBuffer(channelData);
            offset = 0;

            Debug.Write(" Defragmented PDU length=" + newBuffer.Length.ToString());

            if (newBuffer.Length == 0) return;

            _llc.Parse(newBuffer, offset, result);
        }

        public void MacFraqPDU(LogicChannel channelData, int offset, Dictionary<GlobalNames, int> result)
        {
            var fill = (TetraUtils.BitsToInt32(channelData.Ptr, offset, 1) != 0);
            offset++;

            var length = channelData.Length - offset;
            
            if (fill)
            {
                length = CalcRealLength(channelData.Ptr, channelData.Length, offset);
            }

            AddFraqmentsToBuffer(channelData, offset, length);

            Debug.Write("Fragment PDU");
        }

        private void CreateFraqmentsBuffer(LogicChannel buffer, int offset, int length, int encr)
        {
            var ts = buffer.TimeSlot - 1;

            Debug.WriteIf(_writeAddress[ts] != 0, " Warning!!! Fraqments buffer is not empty!");
            if (_writeAddress[ts] + length < _tempBuffers[ts].Length)
            {
                Utils.Memcpy(_tempBuffersPtr[ts], buffer.Ptr + offset, length * sizeof(byte));
                _writeAddress[ts] = length;
                _fragmentsEncription[ts] = encr;
            }
        }
        private void AddFraqmentsToBuffer(LogicChannel buffer, int offset, int length)
        {
            var ts = buffer.TimeSlot - 1;

            Debug.WriteIf(_writeAddress[ts] == 0," Warning!!! Add fraqments in empty buffer!");
            if (_writeAddress[ts] + length < _tempBuffers[ts].Length)
            {
                Utils.Memcpy(_tempBuffersPtr[ts] + _writeAddress[ts], buffer.Ptr + offset, length * sizeof(byte));
                _writeAddress[ts] += length;
            }
        }

        private LogicChannel GetDeFragmentedBuffer(LogicChannel channelData)
        {
            var ts = channelData.TimeSlot - 1;           
            
            Debug.WriteIf(_writeAddress[ts] == 0," Warning!!! Fraqments  buffer is empty!");
            
            var result = new LogicChannel
            {
                Ptr = _tempBuffersPtr[ts],
                Length = ((GlobalFunction.IgnoreEncryptedData && (_fragmentsEncription[ts] != 0)) ? 0 : _writeAddress[ts]),
                CrcIsOk = true,
                TimeSlot = channelData.TimeSlot,
                Frame = channelData.Frame
            };

            _writeAddress[ts] = 0;

            return result;
        }
    }
}