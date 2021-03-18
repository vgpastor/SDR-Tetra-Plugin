using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace SDRSharp.Tetra
{

    [DesignTimeVisible(true)]
    public partial class NetInfoWindow : Form
    {
        const double LatitudeToDegrees = (double)180 / 16777216;
        const double LatitudeCenter = (double)16777216 / 2;
        
        const double LongitudeToDegrees = (double)360 / 33554432;
        const double LongitudeCenter = 33554432 / 2;

        private Dictionary<int, ReceivedData> _neighbourList = new Dictionary<int, ReceivedData>();

        private readonly SortableBindingList<NeighbourDisplay> _neighbourEntries = new SortableBindingList<NeighbourDisplay>();
        private readonly SortableBindingList<CallsDisplay> _callsEntries = new SortableBindingList<CallsDisplay>();
        private readonly SortableBindingList<GroupDisplay> _groupEntries = new SortableBindingList<GroupDisplay>();
        private readonly SortableBindingList<CellDisplay> _cellEntries = new SortableBindingList<CellDisplay>();

        public bool GroupsChanged { get; set; }

        public NetInfoWindow()
        {
            InitializeComponent();

            this.VisibleChanged += NetInfoWindow_VisibleChanged;

            neighbourBindingSource.DataSource = _neighbourEntries;
            callsBindingSource.DataSource = _callsEntries;
            groupBindingSource.DataSource = _groupEntries;
            cellBindingSource.DataSource = _cellEntries;

            groupDataGridView.CellEndEdit += GroupsDataGridView_CellEndEdit;
            groupDataGridView.Sorted += GroupsDataGridView_Sorted;

        }

        void GroupsDataGridView_Sorted(object sender, EventArgs e)
        {
            GroupsChanged = true;
        }

        void GroupsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            GroupsChanged = true;
        }


        void NetInfoWindow_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                checkBox1.Checked = this.TopMost;
            }
        }

        public int TimeOut { get; set; }

        public void UpdateCalls(SortedDictionary<int, CallsEntry> calls)
        {
            if (tabControl1.SelectedTab.Name != "callsTabPage") return;

            var fixedCalls = calls.ToDictionary(entry => entry.Key, entry => entry.Value);

            UpdateCallsGrid(fixedCalls);

        }

        public void UpdateSysInfo(ReceivedData syncInfo, ReceivedData sysInfo)
        {

            if (tabControl1.SelectedTab.Name != "cellTabPage") return;

            syncInfo.SetValue(GlobalNames.Repeater_Master_slave_link_flag, -1);
            syncInfo.SetValue(GlobalNames.Repeater_Frame, -1);
            syncInfo.SetValue(GlobalNames.Repeater_TimeSlot, -1);
            syncInfo.SetValue(GlobalNames.Repeater_Frame_countdown, -1);

            syncInfo.SetValue(GlobalNames.SYNC_PDU_type, -1);
            syncInfo.SetValue(GlobalNames.Master_slave_link_flag, -1);
            syncInfo.SetValue(GlobalNames.Fill_bit, -1);
            syncInfo.SetValue(GlobalNames.Fragmentation_flag, -1);
            syncInfo.SetValue(GlobalNames.Frame_countdown, -1);
            syncInfo.SetValue(GlobalNames.Gateway_generated_message_flag, -1);
            syncInfo.SetValue(GlobalNames.Message_type, -1);

            syncInfo.SetValue(GlobalNames.Frame, -1);
            syncInfo.SetValue(GlobalNames.MultiFrame, -1);
            syncInfo.SetValue(GlobalNames.TimeSlot, -1);

            sysInfo.SetValue(GlobalNames.Frame, -1);
            sysInfo.SetValue(GlobalNames.MultiFrame, -1);
            sysInfo.SetValue(GlobalNames.TimeSlot, -1);

            syncInfo.SetValue(GlobalNames.MAC_PDU_Type, -1);
            syncInfo.SetValue(GlobalNames.MAC_Broadcast_Type, -1);
            sysInfo.SetValue(GlobalNames.MAC_PDU_Type, -1);
            sysInfo.SetValue(GlobalNames.MAC_Broadcast_Type, -1);

            sysInfo.SetValue(GlobalNames.CurrTimeSlot, -1);
            syncInfo.SetValue(GlobalNames.CurrTimeSlot, -1);

            for (var i = 0; i < syncInfo.Data.Length; i++)
            {
                var parameter = (GlobalNames)i;
                if (!syncInfo.Contains(parameter)) continue;

                var value = syncInfo.Value(parameter);
                var knowValues = false;

                foreach (var entry in _cellEntries)
                {
                    if (entry.Parameter == parameter)
                    {
                        knowValues = true;
                        if (entry.Value != value)
                        {
                            entry.Value = value;
                            cellBindingSource.ResetItem(_cellEntries.IndexOf(entry));
                        }
                        break;
                    }
                }

                if (!knowValues)
                {
                    _cellEntries.Add(new CellDisplay()
                    {
                        Parameter = parameter,
                        Value = value,
                        Comment = string.Empty
                    });
                }
            }

            for (var i = 0; i < sysInfo.Data.Length; i++)
            {
                var parameter = (GlobalNames)i;
                if (!sysInfo.Contains(parameter)) continue;

                var value = sysInfo.Value(parameter);
                var knowValues = false;

                foreach (var entry in _cellEntries)
                {
                    if (entry.Parameter == parameter)
                    {
                        knowValues = true;
                        if (entry.Value != value)
                        {
                            entry.Value = value;
                            cellBindingSource.ResetItem(_cellEntries.IndexOf(entry));
                        }
                        break;
                    }
                }

                if (!knowValues)
                {
                    _cellEntries.Add(new CellDisplay()
                    {
                        Parameter = parameter,
                        Value = value,
                        Comment = string.Empty
                    });
                }
            }         
        }


        public void UpdateNeighBour()
        {
            if (tabControl1.SelectedTab.Name != "neighbourTabPage") return;

            if (GlobalFunction.NeighbourList.Count == 0) return;

            while (GlobalFunction.NeighbourList.Count > 0)
            {
                var cell = GlobalFunction.NeighbourList[0];

                cell.SetValue(GlobalNames.OutOfBuffer, -1);

                var cellId = cell.Value(GlobalNames.Cell_identifier);

                for (int i = 0; i < cell.Data.Length; i++)
                {
                    var parameter = (GlobalNames)i;
                    if (!cell.Contains(parameter)) continue;

                    var value = cell.Value(parameter);
                    var knowValues = false;

                    foreach (var entry in _neighbourEntries)
                    {
                        if (entry.Parameter == parameter)
                        {
                            knowValues = true;

                            var curValue = GetCellValue(entry, cellId);

                            if (curValue == null || (int)curValue != value)
                            {
                                SetCellValue(entry, cellId, value);
                            }
                            break;
                        }
                    }

                    if (!knowValues)
                    {
                        var newLine = new NeighbourDisplay()
                        {
                            Parameter = parameter,
                        };
                        SetCellValue(newLine, cellId, value);
                        _neighbourEntries.Add(newLine);
                    }
                }

                GlobalFunction.NeighbourList.RemoveAt(0);
            }

            _neighbourEntries.ResetBindings();
        }

        private void SetCellValue(NeighbourDisplay entry, int cellId, object value)
        {
            switch (cellId)
            {
                case 1: entry.Cell1 = value; break;
                case 2: entry.Cell2 = value; break;
                case 3: entry.Cell3 = value; break;
                case 4: entry.Cell4 = value; break;
                case 5: entry.Cell5 = value; break;
                case 6: entry.Cell6 = value; break;
                case 7: entry.Cell7 = value; break;
                case 8: entry.Cell8 = value; break;
                case 9: entry.Cell9 = value; break;
                case 10: entry.Cell10 = value; break;
                case 11: entry.Cell11 = value; break;
                case 12: entry.Cell12 = value; break;
                case 13: entry.Cell13 = value; break;
                case 14: entry.Cell14 = value; break;
                case 15: entry.Cell15 = value; break;
                case 16: entry.Cell16 = value; break;
                case 17: entry.Cell17 = value; break;
                case 18: entry.Cell18 = value; break;
                case 19: entry.Cell19 = value; break;
                case 20: entry.Cell20 = value; break;
                case 21: entry.Cell21 = value; break;
                case 22: entry.Cell22 = value; break;
                case 23: entry.Cell23 = value; break;
                case 24: entry.Cell24 = value; break;
                case 25: entry.Cell25 = value; break;
                case 26: entry.Cell26 = value; break;
                case 27: entry.Cell27 = value; break;
                case 28: entry.Cell28 = value; break;
                case 29: entry.Cell29 = value; break;
                case 30: entry.Cell30 = value; break;
                case 31: entry.Cell31 = value; break;

                default: entry.Cell32 = value; break;

            }
        }

        private object GetCellValue(NeighbourDisplay parameter, int cellId)
        {
            switch (cellId)
            {
                case 1: return parameter.Cell1;
                case 2: return parameter.Cell2;
                case 3: return parameter.Cell3;
                case 4: return parameter.Cell4;
                case 5: return parameter.Cell5;
                case 6: return parameter.Cell6;
                case 7: return parameter.Cell7;
                case 8: return parameter.Cell8;
                case 9: return parameter.Cell9;
                case 10: return parameter.Cell10;
                case 11: return parameter.Cell11;
                case 12: return parameter.Cell12;
                case 13: return parameter.Cell13;
                case 14: return parameter.Cell14;
                case 15: return parameter.Cell15;
                case 16: return parameter.Cell16;
                case 17: return parameter.Cell17;
                case 18: return parameter.Cell18;
                case 19: return parameter.Cell19;
                case 20: return parameter.Cell20;
                case 21: return parameter.Cell21;
                case 22: return parameter.Cell22;
                case 23: return parameter.Cell23;
                case 24: return parameter.Cell24;
                case 25: return parameter.Cell25;
                case 26: return parameter.Cell26;
                case 27: return parameter.Cell27;
                case 28: return parameter.Cell28;
                case 29: return parameter.Cell29;
                case 30: return parameter.Cell30;
                case 31: return parameter.Cell31;

                default: return parameter.Cell32;
            }
        }

        public void UpdateTextBox(List<ReceivedData> rawData)
        {
            var text = string.Empty;
            var value = 0;

            while (rawData.Count > 0)
            {
                var data = rawData[0];

                if (!data.Contains(GlobalNames.CMCE_Primitives_Type))
                {
                    rawData.RemoveAt(0);
                    continue;
                }
              
                text = string.Empty;

                if (data.TryGetValue(GlobalNames.Encryption_mode, ref value))
                {
                    if (value != 0)
                    {
                        text += "PDU encrypted:" + value.ToString() + " Data incorrect!";
                    }
                }

                if (data.TryGetValue(GlobalNames.Carrier_number, ref value))
                {
                    text += " Carrier:" + value.ToString();
                }

                if (data.TryGetValue(GlobalNames.Timeslot_assigned, ref value))
                {
                    text += " TimeSlot:";
                    if (value == 0)
                    {
                        text += "0";
                    }
                    else
                    {
                        text += (value & 0x8) != 0 ? "1" : "";
                        text += (value & 0x4) != 0 ? "2" : "";
                        text += (value & 0x2) != 0 ? "3" : "";
                        text += (value & 0x1) != 0 ? "4" : "";
                    }
                }

                if (data.TryGetValue(GlobalNames.SSI, ref value))
                {
                    text += " SSI:" + value.ToString();
                }

                if (data.TryGetValue(GlobalNames.Call_identifier, ref value))
                {
                    text += " Call ID:" + value.ToString();
                }

                if (data.TryGetValue(GlobalNames.Encryption_control, ref value))
                {
                    text += " Encrypt:" + (value == 0 ? "Clear" : "E2EE");
                }

                if (data.TryGetValue(GlobalNames.CMCE_Primitives_Type, ref value))
                {
                    text += " " + ((CmcePrimitivesType)value).ToString();
                }

                if (data.TryGetValue(GlobalNames.Transmission_grant, ref value))
                {
                    text += " Transmission " + ((TransmissionGranted)value).ToString();
                }

                if (data.TryGetValue(GlobalNames.Calling_party_address_SSI, ref value))
                {
                    text += " Party_SSI:" + (value).ToString();
                }

                if (data.TryGetValue(GlobalNames.Transmitting_party_address_SSI, ref value))
                {
                    text += " Party_SSI:" + (value).ToString();
                }

                if (data.TryGetValue(GlobalNames.Basic_service_Communication_type, ref value))
                {
                    text += " Basic_service:" + ((CommunicationType)value).ToString();
                }

                if (data.TryGetValue(GlobalNames.Basic_service_Encryption_flag, ref value))
                {
                    text += value == 0 ? " Clear" : " E2EE";
                }

                if (data.TryGetValue(GlobalNames.Basic_service_Circuit_mode_type, ref value))
                {
                    text += " " + ((CircuitModeType)value).ToString();
                }

                //if (data.TryGetValue(GlobalNames.Slot_granting_element, ref value))
                //{
                //    text += " Slot granting:" + (value).ToString();
                //}

                //if (data.TryGetValue(GlobalNames.Reset_Call_time_out, ref value))
                //{
                //    text += " Reset call:" + (value).ToString();
                //}

                if (data.TryGetValue(GlobalNames.Short_data_type_identifier, ref value))
                {
                    text += " Type:" + (value).ToString();
                    
                    var length = 0;
                    var sds = string.Empty;

                    switch (value)
                    {
                        case 0:
                            length = 16;
                            if (data.TryGetValue(GlobalNames.User_Defined_Data_16, ref value))
                            {
                                sds = value.ToString();
                            }
               
                            text += " Length:" + length.ToString();
                            text += " Data:" + sds;

                            break;

                        case 1:
                            length = 32;
                            if (data.TryGetValue(GlobalNames.User_Defined_Data_32, ref value))
                            {
                                sds = ((uint)value).ToString();
                            }

                            text += " Length:" + length.ToString();
                            text += " Data:" + sds;

                            break;

                        case 2:
                            length = 64;
                            var tempValue = (ulong)0;

                            if (data.TryGetValue(GlobalNames.User_Defined_Data_64_1, ref value))
                            {
                                tempValue = (ulong)value << 32;
                                sds = tempValue.ToString();
                            }

                            if (data.TryGetValue(GlobalNames.User_Defined_Data_64_2, ref value))
                            {
                                tempValue = (ulong)value;
                                sds = tempValue.ToString();
                            }
        
                            text += " Length:" + length.ToString();
                            text += " Data:" + sds;

                            break;
                    }
                    
                    if (data.TryGetValue(GlobalNames.Protocol_identifier, ref value))
                    {
                        text += " Protocol:" + ((SdsProtocolIdent)value).ToString();
                    }

                    if (data.TryGetValue(GlobalNames.Location_PDU_type_extension, ref value))
                    {
                        text += " SubType:" + ((LocationTypeExtension)value).ToString();
                    }

                    if (data.TryGetValue(GlobalNames.Latitude, ref value))
                    {
                        var latitude = value * LatitudeToDegrees;
                        if (latitude >= 90) latitude -= 180;
                        text += string.Format(" Lat:{0:0.000000}°", latitude);
                    }

                    if (data.TryGetValue(GlobalNames.Longitude, ref value))
                    {
                        var longitude = value * LongitudeToDegrees;
                        if (longitude >= 180) longitude -= 360;

                        text += string.Format(" Long:{0:0.000000}°", longitude);
                    }

                    if (data.TryGetValue(GlobalNames.Position_error, ref value))
                    {
                        text += " Accuracy:" + ((value * 10) * 2).ToString() + "m";
                    }

                    if (data.TryGetValue(GlobalNames.Horizontal_velocity, ref value))
                    {
                        var velocity = 0.0d;

                        if (value > 28) velocity = 16 * Math.Pow(1.038, value - 13);
                        else velocity = value;

                        text += string.Format(" Velocity:{0:0.0}km/h", velocity);
                    }

                    if (data.TryGetValue(GlobalNames.Direction_of_travel, ref value))
                    {
                        text += " Dir:" + (value * 22.5).ToString() + "°";
                    }
                }

                callsTextBox.AppendText(text + Environment.NewLine);
                rawData.RemoveAt(0);

            }
        }

        public void UpdateGroupGrid(List<GroupDisplay> groups)
        {
            _groupEntries.Clear();

            foreach (var entry in groups)
            {
                _groupEntries.Add(entry);          
            }
        }

        public List<GroupDisplay> GetUpdatedGroups()
        {
            return _groupEntries.ToList();
        }

        public void ResetInfo()
        {
            _neighbourList.Clear();
            _cellEntries.Clear();
            _groupEntries.Clear();

            _neighbourEntries.Clear();
            _callsEntries.Clear();
            callsTextBox.Clear();

            GlobalFunction.NeighbourList.Clear();
        }

        private void UpdateCallsGrid(Dictionary<int, CallsEntry> calls)
        {

            _callsEntries.Clear();
            
            var keys = calls.Keys;

            foreach (var entry in calls)
            {
                var newEntry = new CallsDisplay();

                var to = entry.Value.To;

                var text = string.Empty;
                text += (entry.Value.AssignedSlot & 0x8) != 0 ? " 1" : "";
                text += (entry.Value.AssignedSlot & 0x4) != 0 ? " 2" : "";
                text += (entry.Value.AssignedSlot & 0x2) != 0 ? " 3" : "";
                text += (entry.Value.AssignedSlot & 0x1) != 0 ? " 4" : "";

                newEntry.AssignedSlot = text;
                newEntry.CallID = entry.Value.CallID;
                newEntry.Carrier = entry.Value.Carrier;

                newEntry.Type = ((CommunicationType)entry.Value.Type).ToString();
                newEntry.Encrypted = entry.Value.IsClear == 1 ? " Clear" : " Encrypted";
                newEntry.Duplex = entry.Value.Duplex == 0 ? " Simplex" : " Duplex"; ;
                newEntry.From = entry.Value.From;

                newEntry.To = to.ToString();
                if (_groupEntries != null && _groupEntries.Count > 0)
                {
                    foreach (var gEntry in _groupEntries)
                    {
                        if (gEntry.GSSI == to && gEntry.Name != "")
                        {
                            newEntry.To = gEntry.Name;
                            break;
                        }
                    }
                }

                _callsEntries.Add(newEntry);
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = checkBox1.Checked;
        }
    }

    public class CallsEntry
    {
        public int Carrier { get; set; }
        public int CallID { get; set; }
        public int Type { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public int IsClear { get; set; }
        public int Duplex { get; set; }
        public int WatchDog { get; set; }
        public int AssignedSlot { get; set; }
    }

    public class CallsDisplay
    {
        public int Carrier { get; set; }
        public string AssignedSlot { get; set; }
        public int CallID { get; set; }
        public string Type { get; set; }
        public int From { get; set; }
        public string To { get; set; }
        public string Encrypted { get; set; }
        public string Duplex { get; set; }
    }

    public class CellDisplay
    {
        public GlobalNames Parameter { get; set; }
        public int Value { get; set; }
        public string Comment { get; set; }
    }

    public class NeighbourDisplay
    {
        public GlobalNames Parameter { get; set; }
        public object Cell1 { get; set; }
        public object Cell2 { get; set; }
        public object Cell3 { get; set; }
        public object Cell4 { get; set; }
        public object Cell5 { get; set; }
        public object Cell6 { get; set; }
        public object Cell7 { get; set; }
        public object Cell8 { get; set; }
        public object Cell9 { get; set; }
        public object Cell10 { get; set; }
        public object Cell11 { get; set; }
        public object Cell12 { get; set; }
        public object Cell13 { get; set; }
        public object Cell14 { get; set; }
        public object Cell15 { get; set; }
        public object Cell16 { get; set; }
        public object Cell17 { get; set; }
        public object Cell18 { get; set; }
        public object Cell19 { get; set; }
        public object Cell20 { get; set; }
        public object Cell21 { get; set; }
        public object Cell22 { get; set; }
        public object Cell23 { get; set; }
        public object Cell24 { get; set; }
        public object Cell25 { get; set; }
        public object Cell26 { get; set; }
        public object Cell27 { get; set; }
        public object Cell28 { get; set; }
        public object Cell29 { get; set; }
        public object Cell30 { get; set; }
        public object Cell31 { get; set; }
        public object Cell32 { get; set; }
    }
    public class GroupDisplay
    {
        public int GSSI { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
    }

    public class DataGridViewEx : DataGridView
    {
        public DataGridViewEx()
            : base()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }
    }
}
