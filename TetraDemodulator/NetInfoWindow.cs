using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

        private Dictionary<int, Dictionary<GlobalNames, int>> _neighbourList = new Dictionary<int, Dictionary<GlobalNames, int>>();
        private Dictionary<GlobalNames, int> _currentCell = new Dictionary<GlobalNames, int>();

        private readonly SortableBindingList<NeighbourDisplay> _neighbourEntries = new SortableBindingList<NeighbourDisplay>();
        private readonly SortableBindingList<CallsDisplay> _callsEntries = new SortableBindingList<CallsDisplay>();
        private readonly SortableBindingList<CellDisplay> _cellEntries = new SortableBindingList<CellDisplay>();
        private readonly SortableBindingList<GroupDisplay> _groupEntries = new SortableBindingList<GroupDisplay>();

        public bool GroupsChanged { get; set; }

        public NetInfoWindow()
        {
            InitializeComponent();

            this.VisibleChanged += NetInfoWindow_VisibleChanged;

            neighbourBindingSource.DataSource = _neighbourEntries;
            callsBindingSource.DataSource = _callsEntries;
            cellBindingSource.DataSource = _cellEntries;
            groupBindingSource.DataSource = _groupEntries;

            groupsDataGridView.CellEndEdit += groupsDataGridView_CellEndEdit;
            groupsDataGridView.Sorted += groupsDataGridView_Sorted;

        }

        void groupsDataGridView_Sorted(object sender, EventArgs e)
        {
            GroupsChanged = true;
        }

        void groupsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
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

        public void UpdateSysInfo(Dictionary<GlobalNames, int> syncInfo, Dictionary<GlobalNames, int> sysInfo)
        {
            if (tabControl1.SelectedTab.Name != "cellTabPage") return;

            sysInfo.Remove(GlobalNames.MAC_PDU_Type);
            sysInfo.Remove(GlobalNames.MAC_Broadcast_Type);
            syncInfo.Remove(GlobalNames.TimeSlot);
            syncInfo.Remove(GlobalNames.Frame);
            syncInfo.Remove(GlobalNames.MultiFrame);

            foreach (var entry in syncInfo)
            {
                var needUpdate = true;

                for (int i = 0; i < _cellEntries.Count; i++)
                {
                    if (_cellEntries[i].Parameter == entry.Key)
                    {
                        needUpdate = false;

                        if (_cellEntries[i].Value != entry.Value)
                        {
                            _cellEntries[i].Value = entry.Value;
                            cellBindingSource.ResetItem(i);
                            break;
                        }
                    }
                }

                if (needUpdate)
                {
                    var newEwntry = new CellDisplay();

                    newEwntry.Parameter = (GlobalNames)entry.Key;
                    newEwntry.Value = entry.Value;
                    newEwntry.Comment = string.Empty;

                    _cellEntries.Add(newEwntry);
                }
            }

            foreach (var entry in sysInfo)
            {
                var needUpdate = true;

                for (int i = 0; i < _cellEntries.Count; i++)
                {
                    if (_cellEntries[i].Parameter == entry.Key)
                    {
                        needUpdate = false;

                        if (_cellEntries[i].Value != entry.Value)
                        {
                            _cellEntries[i].Value = entry.Value;
                            cellBindingSource.ResetItem(i);
                            break;
                        }
                    }
                }

                if (needUpdate)
                {
                    var newEwntry = new CellDisplay();

                    newEwntry.Parameter = (GlobalNames)entry.Key;
                    newEwntry.Value = entry.Value;
                    newEwntry.Comment = string.Empty;

                    _cellEntries.Add(newEwntry);
                }
            }
        }

        public void UpdateNeighBour()
        {
            if (tabControl1.SelectedTab.Name != "neighbourTabPage") return;

            while (GlobalFunction.NeighbourList.Count > 0)
            {
                var cell = GlobalFunction.NeighbourList[0];

                if (!_neighbourList.ContainsKey(cell[GlobalNames.Cell_identifier]))
                {
                    _neighbourList.Add(cell[GlobalNames.Cell_identifier], cell);

                    foreach (var entry in cell)
                    {
                        var newEntry = new NeighbourDisplay()
                        {
                            Parameter = entry.Key,
                            Data = entry.Value.ToString(),
                            Comment = string.Empty
                        };

                        _neighbourEntries.Add(newEntry);
                    }

                   
                    _neighbourEntries.Add(new NeighbourDisplay()
                    {
                        Parameter = GlobalNames.Data_Separator,
                        Data = "---------",
                        Comment = "-----------"
                    }
                        );


                }
                GlobalFunction.NeighbourList.RemoveAt(0);

            }
        }

        public void UpdateTextBox(List<Dictionary<GlobalNames, int>> rawData)
        {
            var text = string.Empty;
            var value = 0;

            while (rawData.Count > 0)
            {
                var data = rawData[0];

                if (!data.ContainsKey(GlobalNames.CMCE_Primitives_Type))
                {
                    rawData.RemoveAt(0);
                    continue;
                }
              
                text = string.Empty;

                if (data.TryGetValue(GlobalNames.Encryption_mode, out value))
                {
                    if (value != 0)
                    {
                        text += "PDU encrypted:" + value.ToString() + " Data incorrect!";
                    }
                }

                if (data.TryGetValue(GlobalNames.Carrier_number, out value))
                {
                    text += " Carrier:" + value.ToString();
                }

                if (data.TryGetValue(GlobalNames.Timeslot_assigned, out value))
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

                if (data.TryGetValue(GlobalNames.SSI, out value))
                {
                    text += " SSI:" + value.ToString();
                }

                if (data.TryGetValue(GlobalNames.Call_identifier, out value))
                {
                    text += " Call ID:" + value.ToString();
                }

                if (data.TryGetValue(GlobalNames.Encryption_control, out value))
                {
                    text += " Encrypt:" + (value == 0 ? "Clear" : "E2EE");
                }

                if (data.TryGetValue(GlobalNames.CMCE_Primitives_Type, out value))
                {
                    text += " " + ((CmcePrimitivesType)value).ToString();
                }

                if (data.TryGetValue(GlobalNames.Transmission_grant, out value))
                {
                    text += " Transmission " + ((TransmissionGranted)value).ToString();
                }

                if (data.TryGetValue(GlobalNames.Calling_party_address_SSI, out value))
                {
                    text += " Party_SSI:" + (value).ToString();
                }

                if (data.TryGetValue(GlobalNames.Transmitting_party_address_SSI, out value))
                {
                    text += " Party_SSI:" + (value).ToString();
                }

                if (data.TryGetValue(GlobalNames.Basic_service_Communication_type, out value))
                {
                    text += " Basic_service:" + ((CommunicationType)value).ToString();
                }

                if (data.TryGetValue(GlobalNames.Basic_service_Encryption_flag, out value))
                {
                    text += value == 0 ? " Clear" : " E2EE";
                }

                if (data.TryGetValue(GlobalNames.Basic_service_Circuit_mode_type, out value))
                {
                    text += " " + ((CircuitModeType)value).ToString();
                }

                //if (data.TryGetValue(GlobalNames.Slot_granting_element, out value))
                //{
                //    text += " Slot granting:" + (value).ToString();
                //}

                //if (data.TryGetValue(GlobalNames.Reset_Call_time_out, out value))
                //{
                //    text += " Reset call:" + (value).ToString();
                //}

                if (data.TryGetValue(GlobalNames.Short_data_type_identifier, out value))
                {
                    text += " Type:" + (value).ToString();
                    
                    var length = 0;
                    var sds = string.Empty;

                    switch (value)
                    {
                        case 0:
                            length = 16;
                            if (data.TryGetValue(GlobalNames.User_Defined_Data_16, out value))
                            {
                                sds = value.ToString();
                            }
               
                            text += " Length:" + length.ToString();
                            text += " Data:" + sds;

                            break;

                        case 1:
                            length = 32;
                            if (data.TryGetValue(GlobalNames.User_Defined_Data_32, out value))
                            {
                                sds = ((uint)value).ToString();
                            }

                            text += " Length:" + length.ToString();
                            text += " Data:" + sds;

                            break;

                        case 2:
                            length = 64;
                            var tempValue = (ulong)0;

                            if (data.TryGetValue(GlobalNames.User_Defined_Data_64_1, out value))
                            {
                                tempValue = (ulong)value << 32;
                                sds = tempValue.ToString();
                            }

                            if (data.TryGetValue(GlobalNames.User_Defined_Data_64_2, out value))
                            {
                                tempValue = (ulong)value;
                                sds = tempValue.ToString();
                            }
        
                            text += " Length:" + length.ToString();
                            text += " Data:" + sds;

                            break;
                    }
                    
                    if (data.TryGetValue(GlobalNames.Protocol_identifier, out value))
                    {
                        text += " Protocol:" + ((SdsProtocolIdent)value).ToString();
                    }

                    if (data.TryGetValue(GlobalNames.Location_PDU_type_extension, out value))
                    {
                        text += " SubType:" + ((LocationTypeExtension)value).ToString();
                    }

                    if (data.TryGetValue(GlobalNames.Latitude, out value))
                    {
                        var latitude = value * LatitudeToDegrees;
                        if (latitude >= 90) latitude -= 180;
                        text += string.Format(" Lat:{0:0.000000}°", latitude);
                    }

                    if (data.TryGetValue(GlobalNames.Longitude, out value))
                    {
                        var longitude = value * LongitudeToDegrees;
                        if (longitude >= 180) longitude -= 360;

                        text += string.Format(" Long:{0:0.000000}°", longitude);
                    }

                    if (data.TryGetValue(GlobalNames.Position_error, out value))
                    {
                        text += " Accuracy:" + ((value * 10) * 2).ToString() + "m";
                    }

                    if (data.TryGetValue(GlobalNames.Horizontal_velocity, out value))
                    {
                        var velocity = 0.0d;

                        if (value > 28) velocity = 16 * Math.Pow(1.038, value - 13);
                        else velocity = value;

                        text += string.Format(" Velocity:{0:0.0}km/h", velocity);
                    }

                    if (data.TryGetValue(GlobalNames.Direction_of_travel, out value))
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
            _currentCell.Clear();
            _groupEntries.Clear();

            _neighbourEntries.Clear();
            _cellEntries.Clear();
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
                var ssiValue = String.Empty;

                var newEntry = new CallsDisplay();

                foreach (int ssi in entry.Value.Users)
                {
                    ssiValue += ssi.ToString() + ", ";
                }

                var text = string.Empty;
                text += (entry.Value.AssignedSlot & 0x8) != 0 ? " 1" : "";
                text += (entry.Value.AssignedSlot & 0x4) != 0 ? " 2" : "";
                text += (entry.Value.AssignedSlot & 0x2) != 0 ? " 3" : "";
                text += (entry.Value.AssignedSlot & 0x1) != 0 ? " 4" : "";

                newEntry.AssignedSlot = text;
                newEntry.CallID = entry.Value.CallID;
                newEntry.Carrier = entry.Value.Carrier;

                newEntry.Group = entry.Value.Group;
                newEntry.Users = ssiValue;
                newEntry.Encrypted = entry.Value.IsClear == 1 ? " Clear" : " Encrypted";
                newEntry.Duplex = entry.Value.Duplex == 0 ? " Simplex" : " Duplex"; ;
                newEntry.TXer = entry.Value.TXer;

                _callsEntries.Add(newEntry);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = checkBox1.Checked;
        }

    }

    public class CallsEntry
    {
        public int Carrier { get; set; }
        public int CallID { get; set; }
        public int SSI { get; set; }
        public int Group { get; set; }
        public int TXer { get; set; }
        public List<int> Users { get; set; }
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
        public int Group { get; set; }
        public int TXer { get; set; }
        public string Users { get; set; }
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
        public string Data { get; set; }
        public string Comment { get; set; }
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
