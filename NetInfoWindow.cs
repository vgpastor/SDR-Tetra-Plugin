// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.NetInfoWindow
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SDRSharp.Tetra
{
    [DesignTimeVisible(true)]
    public class NetInfoWindow : Form
    {
        private const double LatitudeToDegrees = 1.07288360595703E-05;
        private const double LatitudeCenter = 8388608.0;
        private const double LongitudeToDegrees = 1.07288360595703E-05;
        private const double LongitudeCenter = 16777216.0;
        private Dictionary<int, ReceivedData> _neighbourList = new Dictionary<int, ReceivedData>();
        private readonly SortableBindingList<NeighbourDisplay> _neighbourEntries = new SortableBindingList<NeighbourDisplay>();
        private readonly SortableBindingList<CallsDisplay> _callsEntries = new SortableBindingList<CallsDisplay>();
        private readonly SortableBindingList<GroupDisplay> _groupEntries = new SortableBindingList<GroupDisplay>();
        private readonly SortableBindingList<CellDisplay> _cellEntries = new SortableBindingList<CellDisplay>();
        private IContainer components;
        private Timer timeOutTimer;
        private TabControl tabControl1;
        private TabPage callsTabPage;
        private TabPage neighbourTabPage;
        private TabPage cellTabPage;
        private CheckBox checkBox1;
        private BindingSource callsBindingSource;
        private BindingSource neighbourBindingSource;
        private BindingSource cellBindingSource;
        private TabPage groupTabPage;
        private BindingSource groupBindingSource;
        private SplitContainer splitContainer2;
        private DataGridViewEx callsDataGridView;
        private TextBox callsTextBox;
        private DataGridViewTextBoxColumn callIDDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn Type;
        private DataGridViewTextBoxColumn From;
        private DataGridViewTextBoxColumn To;
        private DataGridViewTextBoxColumn Carrier;
        private DataGridViewTextBoxColumn assignedSlotDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn encryptedDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn duplexDataGridViewTextBoxColumn;
        private DataGridView cellDataGridView;
        private DataGridViewTextBoxColumn parameterDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn valueDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn commentDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn parameterDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn Cell1;
        private DataGridViewTextBoxColumn Cell2;
        private DataGridViewTextBoxColumn Cell3;
        private DataGridViewTextBoxColumn Cell4;
        private DataGridViewTextBoxColumn Cell5;
        private DataGridViewTextBoxColumn Cell6;
        private DataGridViewTextBoxColumn Cell7;
        private DataGridViewTextBoxColumn Cell8;
        private DataGridViewTextBoxColumn Cell9;
        private DataGridViewTextBoxColumn Cell10;
        private DataGridViewTextBoxColumn Cell11;
        private DataGridViewTextBoxColumn Cell12;
        private DataGridViewTextBoxColumn Cell13;
        private DataGridViewTextBoxColumn Cell14;
        private DataGridViewTextBoxColumn Cell15;
        private DataGridViewTextBoxColumn Cell16;
        private DataGridViewTextBoxColumn Cell17;
        private DataGridViewTextBoxColumn Cell18;
        private DataGridViewTextBoxColumn Cell19;
        private DataGridViewTextBoxColumn Cell20;
        private DataGridViewTextBoxColumn Cell21;
        private DataGridViewTextBoxColumn Cell22;
        private DataGridViewTextBoxColumn Cell23;
        private DataGridViewTextBoxColumn Cell24;
        private DataGridViewTextBoxColumn Cell25;
        private DataGridViewTextBoxColumn Cell26;
        private DataGridViewTextBoxColumn Cell27;
        private DataGridViewTextBoxColumn Cell28;
        private DataGridViewTextBoxColumn Cell29;
        private DataGridViewTextBoxColumn Cell30;
        private DataGridViewTextBoxColumn Cell31;
        private DataGridViewTextBoxColumn Cell32;
        private DataGridViewEx groupDataGridView;
        private DataGridViewTextBoxColumn gSSIDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn priorityDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn1;
        private DataGridViewEx neighbourDataGridView;
        private DataGridViewTextBoxColumn parameterDataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn cell1DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell2DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell3DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell4DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell5DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell6DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell7DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell8DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell9DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell10DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell11DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell12DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell13DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell14DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell15DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell16DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell17DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell18DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell19DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell20DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell21DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell22DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell23DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell24DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell25DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell26DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell27DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell28DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell29DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell30DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell31DataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cell32DataGridViewTextBoxColumn;

        public bool GroupsChanged { get; set; }

        public NetInfoWindow()
        {
            this.InitializeComponent();
            this.VisibleChanged += new EventHandler(this.NetInfoWindow_VisibleChanged);
            this.neighbourBindingSource.DataSource = (object)this._neighbourEntries;
            this.callsBindingSource.DataSource = (object)this._callsEntries;
            this.groupBindingSource.DataSource = (object)this._groupEntries;
            this.cellBindingSource.DataSource = (object)this._cellEntries;
            this.groupDataGridView.CellEndEdit += new DataGridViewCellEventHandler(this.GroupsDataGridView_CellEndEdit);
            this.groupDataGridView.Sorted += new EventHandler(this.GroupsDataGridView_Sorted);
        }

        private void GroupsDataGridView_Sorted(object sender, EventArgs e) => this.GroupsChanged = true;

        private void GroupsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e) => this.GroupsChanged = true;

        private void NetInfoWindow_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
                return;
            this.checkBox1.Checked = this.TopMost;
        }

        public int TimeOut { get; set; }

        public void UpdateCalls(SortedDictionary<int, CallsEntry> calls)
        {
            if (this.tabControl1.SelectedTab.Name != "callsTabPage")
                return;
            this.UpdateCallsGrid(calls.ToDictionary<KeyValuePair<int, CallsEntry>, int, CallsEntry>((Func<KeyValuePair<int, CallsEntry>, int>)(entry => entry.Key), (Func<KeyValuePair<int, CallsEntry>, CallsEntry>)(entry => entry.Value)));
        }

        public void UpdateSysInfo(ReceivedData syncInfo, ReceivedData sysInfo)
        {
            if (this.tabControl1.SelectedTab.Name != "cellTabPage")
                return;
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
            for (int index = 0; index < syncInfo.Data.Length; ++index)
            {
                GlobalNames name = (GlobalNames)index;
                if (syncInfo.Contains(name))
                {
                    int num = syncInfo.Value(name);
                    bool flag = false;
                    foreach (CellDisplay cellEntry in (Collection<CellDisplay>)this._cellEntries)
                    {
                        if (cellEntry.Parameter == name)
                        {
                            flag = true;
                            if (cellEntry.Value != num)
                            {
                                cellEntry.Value = num;
                                this.cellBindingSource.ResetItem(this._cellEntries.IndexOf(cellEntry));
                                break;
                            }
                            break;
                        }
                    }
                    if (!flag)
                        this._cellEntries.Add(new CellDisplay()
                        {
                            Parameter = name,
                            Value = num,
                            Comment = string.Empty
                        });
                }
            }
            for (int index = 0; index < sysInfo.Data.Length; ++index)
            {
                GlobalNames name = (GlobalNames)index;
                if (sysInfo.Contains(name))
                {
                    int num = sysInfo.Value(name);
                    bool flag = false;
                    foreach (CellDisplay cellEntry in (Collection<CellDisplay>)this._cellEntries)
                    {
                        if (cellEntry.Parameter == name)
                        {
                            flag = true;
                            if (cellEntry.Value != num)
                            {
                                cellEntry.Value = num;
                                this.cellBindingSource.ResetItem(this._cellEntries.IndexOf(cellEntry));
                                break;
                            }
                            break;
                        }
                    }
                    if (!flag)
                        this._cellEntries.Add(new CellDisplay()
                        {
                            Parameter = name,
                            Value = num,
                            Comment = string.Empty
                        });
                }
            }
        }

        public void UpdateNeighBour()
        {
            if (this.tabControl1.SelectedTab.Name != "neighbourTabPage" || Global.NeighbourList.Count == 0)
                return;
            while (Global.NeighbourList.Count > 0)
            {
                ReceivedData neighbour = Global.NeighbourList[0];
                int cellId = neighbour.Value(GlobalNames.Cell_identifier);
                for (int index = 0; index < neighbour.Data.Length; ++index)
                {
                    GlobalNames name = (GlobalNames)index;
                    if (neighbour.Contains(name))
                    {
                        int num = neighbour.Value(name);
                        bool flag = false;
                        foreach (NeighbourDisplay neighbourEntry in (Collection<NeighbourDisplay>)this._neighbourEntries)
                        {
                            if (neighbourEntry.Parameter == name)
                            {
                                flag = true;
                                object cellValue = this.GetCellValue(neighbourEntry, cellId);
                                if (cellValue != null)
                                {
                                    if ((int)cellValue == num)
                                        break;
                                }
                                this.SetCellValue(neighbourEntry, cellId, (object)num);
                                break;
                            }
                        }
                        if (!flag)
                        {
                            NeighbourDisplay entry = new NeighbourDisplay()
                            {
                                Parameter = name
                            };
                            this.SetCellValue(entry, cellId, (object)num);
                            this._neighbourEntries.Add(entry);
                        }
                    }
                }
                Global.NeighbourList.RemoveAt(0);
            }
            this._neighbourEntries.ResetBindings();
        }

        private void SetCellValue(NeighbourDisplay entry, int cellId, object value)
        {
            switch (cellId)
            {
                case 1:
                    entry.Cell1 = value;
                    break;
                case 2:
                    entry.Cell2 = value;
                    break;
                case 3:
                    entry.Cell3 = value;
                    break;
                case 4:
                    entry.Cell4 = value;
                    break;
                case 5:
                    entry.Cell5 = value;
                    break;
                case 6:
                    entry.Cell6 = value;
                    break;
                case 7:
                    entry.Cell7 = value;
                    break;
                case 8:
                    entry.Cell8 = value;
                    break;
                case 9:
                    entry.Cell9 = value;
                    break;
                case 10:
                    entry.Cell10 = value;
                    break;
                case 11:
                    entry.Cell11 = value;
                    break;
                case 12:
                    entry.Cell12 = value;
                    break;
                case 13:
                    entry.Cell13 = value;
                    break;
                case 14:
                    entry.Cell14 = value;
                    break;
                case 15:
                    entry.Cell15 = value;
                    break;
                case 16:
                    entry.Cell16 = value;
                    break;
                case 17:
                    entry.Cell17 = value;
                    break;
                case 18:
                    entry.Cell18 = value;
                    break;
                case 19:
                    entry.Cell19 = value;
                    break;
                case 20:
                    entry.Cell20 = value;
                    break;
                case 21:
                    entry.Cell21 = value;
                    break;
                case 22:
                    entry.Cell22 = value;
                    break;
                case 23:
                    entry.Cell23 = value;
                    break;
                case 24:
                    entry.Cell24 = value;
                    break;
                case 25:
                    entry.Cell25 = value;
                    break;
                case 26:
                    entry.Cell26 = value;
                    break;
                case 27:
                    entry.Cell27 = value;
                    break;
                case 28:
                    entry.Cell28 = value;
                    break;
                case 29:
                    entry.Cell29 = value;
                    break;
                case 30:
                    entry.Cell30 = value;
                    break;
                case 31:
                    entry.Cell31 = value;
                    break;
                default:
                    entry.Cell32 = value;
                    break;
            }
        }

        private object GetCellValue(NeighbourDisplay parameter, int cellId)
        {
            switch (cellId)
            {
                case 1:
                    return parameter.Cell1;
                case 2:
                    return parameter.Cell2;
                case 3:
                    return parameter.Cell3;
                case 4:
                    return parameter.Cell4;
                case 5:
                    return parameter.Cell5;
                case 6:
                    return parameter.Cell6;
                case 7:
                    return parameter.Cell7;
                case 8:
                    return parameter.Cell8;
                case 9:
                    return parameter.Cell9;
                case 10:
                    return parameter.Cell10;
                case 11:
                    return parameter.Cell11;
                case 12:
                    return parameter.Cell12;
                case 13:
                    return parameter.Cell13;
                case 14:
                    return parameter.Cell14;
                case 15:
                    return parameter.Cell15;
                case 16:
                    return parameter.Cell16;
                case 17:
                    return parameter.Cell17;
                case 18:
                    return parameter.Cell18;
                case 19:
                    return parameter.Cell19;
                case 20:
                    return parameter.Cell20;
                case 21:
                    return parameter.Cell21;
                case 22:
                    return parameter.Cell22;
                case 23:
                    return parameter.Cell23;
                case 24:
                    return parameter.Cell24;
                case 25:
                    return parameter.Cell25;
                case 26:
                    return parameter.Cell26;
                case 27:
                    return parameter.Cell27;
                case 28:
                    return parameter.Cell28;
                case 29:
                    return parameter.Cell29;
                case 30:
                    return parameter.Cell30;
                case 31:
                    return parameter.Cell31;
                default:
                    return (object)(int)parameter.Cell32;
            }
        }

        public void UpdateTextBox(List<ReceivedData> rawData)
        {
            string empty1 = string.Empty;
            int num1 = 0;
            while (rawData.Count > 0)
            {
                ReceivedData receivedData = rawData[0];
                if (!receivedData.Contains(GlobalNames.CMCE_Primitives_Type))
                {
                    rawData.RemoveAt(0);
                }
                else
                {
                    string str1 = string.Empty;
                    if (receivedData.TryGetValue(GlobalNames.Encryption_mode, ref num1) && num1 != 0)
                        str1 = str1 + "PDU encrypted:" + num1.ToString() + " Data incorrect!";
                    if (receivedData.TryGetValue(GlobalNames.Carrier_number, ref num1))
                        str1 = str1 + " Carrier:" + num1.ToString();
                    if (receivedData.TryGetValue(GlobalNames.Timeslot_assigned, ref num1))
                    {
                        string str2 = str1 + " TimeSlot:";
                        str1 = num1 != 0 ? str2 + ((num1 & 8) != 0 ? "1" : "") + ((num1 & 4) != 0 ? "2" : "") + ((num1 & 2) != 0 ? "3" : "") + ((num1 & 1) != 0 ? "4" : "") : str2 + "0";
                    }
                    if (receivedData.TryGetValue(GlobalNames.SSI, ref num1))
                        str1 = str1 + " SSI:" + num1.ToString();
                    if (receivedData.TryGetValue(GlobalNames.Call_identifier, ref num1))
                        str1 = str1 + " Call ID:" + num1.ToString();
                    if (receivedData.TryGetValue(GlobalNames.Encryption_control, ref num1))
                        str1 = str1 + " Encrypt:" + (num1 == 0 ? "Clear" : "E2EE");
                    if (receivedData.TryGetValue(GlobalNames.CMCE_Primitives_Type, ref num1))
                        str1 = str1 + " " + ((CmcePrimitivesType)num1).ToString();
                    if (receivedData.TryGetValue(GlobalNames.Transmission_grant, ref num1))
                        str1 = str1 + " Transmission " + ((TransmissionGranted)num1).ToString();
                    if (receivedData.TryGetValue(GlobalNames.Calling_party_address_SSI, ref num1))
                        str1 = str1 + " Party_SSI:" + num1.ToString();
                    if (receivedData.TryGetValue(GlobalNames.Transmitting_party_address_SSI, ref num1))
                        str1 = str1 + " Party_SSI:" + num1.ToString();
                    if (receivedData.TryGetValue(GlobalNames.Basic_service_Communication_type, ref num1))
                        str1 = str1 + " Basic_service:" + ((CommunicationType)num1).ToString();
                    if (receivedData.TryGetValue(GlobalNames.Basic_service_Encryption_flag, ref num1))
                        str1 += num1 == 0 ? " Clear" : " E2EE";
                    if (receivedData.TryGetValue(GlobalNames.Basic_service_Circuit_mode_type, ref num1))
                        str1 = str1 + " " + ((CircuitModeType)num1).ToString();
                    if (receivedData.TryGetValue(GlobalNames.Short_data_type_identifier, ref num1))
                    {
                        str1 = str1 + " Type:" + num1.ToString();
                        string empty2 = string.Empty;
                        switch (num1)
                        {
                            case 0:
                                int num2 = 16;
                                if (receivedData.TryGetValue(GlobalNames.User_Defined_Data_16, ref num1))
                                    empty2 = num1.ToString();
                                str1 = str1 + " Length:" + num2.ToString() + " Data:" + empty2;
                                break;
                            case 1:
                                int num3 = 32;
                                if (receivedData.TryGetValue(GlobalNames.User_Defined_Data_32, ref num1))
                                    empty2 = ((uint)num1).ToString();
                                str1 = str1 + " Length:" + num3.ToString() + " Data:" + empty2;
                                break;
                            case 2:
                                int num4 = 64;
                                ulong num5 = 0;
                                if (receivedData.TryGetValue(GlobalNames.User_Defined_Data_64_1, ref num1))
                                {
                                    num5 = (ulong)num1 << 32;
                                    empty2 = num5.ToString();
                                }
                                if (receivedData.TryGetValue(GlobalNames.User_Defined_Data_64_2, ref num1))
                                {
                                    num5 = (ulong)num1;
                                    empty2 = num5.ToString();
                                }
                                str1 = str1 + " Length:" + num4.ToString() + " Data:" + empty2;
                                break;
                        }
                        if (receivedData.TryGetValue(GlobalNames.Protocol_identifier, ref num1))
                            str1 = str1 + " Protocol:" + ((SdsProtocolIdent)num1).ToString();
                        if (receivedData.TryGetValue(GlobalNames.Location_PDU_type_extension, ref num1))
                            str1 = str1 + " SubType:" + ((LocationTypeExtension)num1).ToString();
                        if (receivedData.TryGetValue(GlobalNames.Latitude, ref num1))
                        {
                            double num6 = (double)num1 * 1.07288360595703E-05;
                            if (num6 >= 90.0)
                                num6 -= 180.0;
                            str1 += string.Format(" Lat:{0:0.000000}°", (object)num6);
                        }
                        if (receivedData.TryGetValue(GlobalNames.Longitude, ref num1))
                        {
                            double num6 = (double)num1 * 1.07288360595703E-05;
                            if (num6 >= 180.0)
                                num6 -= 360.0;
                            str1 += string.Format(" Long:{0:0.000000}°", (object)num6);
                        }
                        if (receivedData.TryGetValue(GlobalNames.Position_error, ref num1))
                            str1 = str1 + " Accuracy:" + (num1 * 10 * 2).ToString() + "m";
                        if (receivedData.TryGetValue(GlobalNames.Horizontal_velocity, ref num1))
                        {
                            double num6 = num1 <= 28 ? (double)num1 : 16.0 * Math.Pow(1.038, (double)(num1 - 13));
                            str1 += string.Format(" Velocity:{0:0.0}km/h", (object)num6);
                        }
                        if (receivedData.TryGetValue(GlobalNames.Direction_of_travel, ref num1))
                            str1 = str1 + " Dir:" + ((double)num1 * 22.5).ToString() + "°";
                    }
                    this.callsTextBox.AppendText(str1 + Environment.NewLine);
                    rawData.RemoveAt(0);
                }
            }
        }

        public void UpdateGroupGrid(List<GroupDisplay> groups)
        {
            this._groupEntries.Clear();
            foreach (GroupDisplay group in groups)
                this._groupEntries.Add(group);
        }

        public List<GroupDisplay> GetUpdatedGroups() => this._groupEntries.ToList<GroupDisplay>();

        public void ResetInfo()
        {
            this._neighbourList.Clear();
            this._cellEntries.Clear();
            this._groupEntries.Clear();
            this._neighbourEntries.Clear();
            this._callsEntries.Clear();
            this.callsTextBox.Clear();
            Global.NeighbourList.Clear();
        }

        private void UpdateCallsGrid(Dictionary<int, CallsEntry> calls)
        {
            this._callsEntries.Clear();
            Dictionary<int, CallsEntry>.KeyCollection keys = calls.Keys;
            foreach (KeyValuePair<int, CallsEntry> call in calls)
            {
                CallsDisplay callsDisplay = new CallsDisplay();
                int to = call.Value.To;
                string str = string.Empty + ((call.Value.AssignedSlot & 8) != 0 ? " 1" : "") + ((call.Value.AssignedSlot & 4) != 0 ? " 2" : "") + ((call.Value.AssignedSlot & 2) != 0 ? " 3" : "") + ((call.Value.AssignedSlot & 1) != 0 ? " 4" : "");
                callsDisplay.AssignedSlot = str;
                callsDisplay.CallID = call.Value.CallID;
                callsDisplay.Carrier = call.Value.Carrier;
                callsDisplay.Type = ((CommunicationType)call.Value.Type).ToString();
                callsDisplay.Encrypted = call.Value.IsClear == 1 ? " Clear" : " Encrypted";
                callsDisplay.Duplex = call.Value.Duplex == 0 ? " Simplex" : " Duplex";
                callsDisplay.From = call.Value.From;
                callsDisplay.To = to.ToString();
                if (this._groupEntries != null && this._groupEntries.Count > 0)
                {
                    foreach (GroupDisplay groupEntry in (Collection<GroupDisplay>)this._groupEntries)
                    {
                        if (groupEntry.GSSI == to && groupEntry.Name != "")
                        {
                            callsDisplay.To = groupEntry.Name;
                            break;
                        }
                    }
                }
                this._callsEntries.Add(callsDisplay);
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e) => this.TopMost = this.checkBox1.Checked;

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.timeOutTimer = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.callsTabPage = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.callsDataGridView = new SDRSharp.Tetra.DataGridViewEx();
            this.callIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.From = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.To = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Carrier = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.assignedSlotDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.encryptedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.duplexDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.callsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.callsTextBox = new System.Windows.Forms.TextBox();
            this.groupTabPage = new System.Windows.Forms.TabPage();
            this.groupDataGridView = new SDRSharp.Tetra.DataGridViewEx();
            this.gSSIDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.priorityDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cellTabPage = new System.Windows.Forms.TabPage();
            this.cellDataGridView = new System.Windows.Forms.DataGridView();
            this.parameterDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.commentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cellBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.neighbourTabPage = new System.Windows.Forms.TabPage();
            this.neighbourDataGridView = new SDRSharp.Tetra.DataGridViewEx();
            this.parameterDataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell3DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell4DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell5DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell6DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell7DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell8DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell9DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell10DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell11DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell12DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell13DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell14DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell15DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell16DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell17DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell18DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell19DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell20DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell21DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell22DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell23DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell24DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell25DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell26DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell27DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell28DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell29DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell30DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell31DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cell32DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.neighbourBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.parameterDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell20 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell21 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell22 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell23 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell24 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell25 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell26 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell27 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell28 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell29 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell30 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell31 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell32 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.callsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.callsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.callsBindingSource)).BeginInit();
            this.groupTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBindingSource)).BeginInit();
            this.cellTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cellDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cellBindingSource)).BeginInit();
            this.neighbourTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.neighbourDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.neighbourBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // timeOutTimer
            // 
            this.timeOutTimer.Enabled = true;
            this.timeOutTimer.Interval = 50;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.callsTabPage);
            this.tabControl1.Controls.Add(this.groupTabPage);
            this.tabControl1.Controls.Add(this.cellTabPage);
            this.tabControl1.Controls.Add(this.neighbourTabPage);
            this.tabControl1.Location = new System.Drawing.Point(14, 14);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(632, 492);
            this.tabControl1.TabIndex = 23;
            // 
            // callsTabPage
            // 
            this.callsTabPage.Controls.Add(this.splitContainer2);
            this.callsTabPage.Location = new System.Drawing.Point(4, 24);
            this.callsTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.callsTabPage.Name = "callsTabPage";
            this.callsTabPage.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.callsTabPage.Size = new System.Drawing.Size(624, 464);
            this.callsTabPage.TabIndex = 0;
            this.callsTabPage.Text = "Calls";
            this.callsTabPage.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(4, 3);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.callsDataGridView);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.callsTextBox);
            this.splitContainer2.Size = new System.Drawing.Size(616, 458);
            this.splitContainer2.SplitterDistance = 273;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 25;
            // 
            // callsDataGridView
            // 
            this.callsDataGridView.AllowUserToAddRows = false;
            this.callsDataGridView.AllowUserToDeleteRows = false;
            this.callsDataGridView.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.callsDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.callsDataGridView.AutoGenerateColumns = false;
            this.callsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.callsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.callIDDataGridViewTextBoxColumn,
            this.Type,
            this.From,
            this.To,
            this.Carrier,
            this.assignedSlotDataGridViewTextBoxColumn,
            this.encryptedDataGridViewTextBoxColumn,
            this.duplexDataGridViewTextBoxColumn});
            this.callsDataGridView.DataSource = this.callsBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.callsDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.callsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.callsDataGridView.Location = new System.Drawing.Point(0, 0);
            this.callsDataGridView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.callsDataGridView.Name = "callsDataGridView";
            this.callsDataGridView.ReadOnly = true;
            this.callsDataGridView.RowHeadersVisible = false;
            this.callsDataGridView.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.callsDataGridView.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Blue;
            this.callsDataGridView.ShowCellErrors = false;
            this.callsDataGridView.ShowRowErrors = false;
            this.callsDataGridView.Size = new System.Drawing.Size(614, 271);
            this.callsDataGridView.TabIndex = 0;
            this.callsDataGridView.VirtualMode = true;
            // 
            // callIDDataGridViewTextBoxColumn
            // 
            this.callIDDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.callIDDataGridViewTextBoxColumn.DataPropertyName = "CallID";
            this.callIDDataGridViewTextBoxColumn.HeaderText = "Call ID";
            this.callIDDataGridViewTextBoxColumn.MinimumWidth = 60;
            this.callIDDataGridViewTextBoxColumn.Name = "callIDDataGridViewTextBoxColumn";
            this.callIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.callIDDataGridViewTextBoxColumn.Width = 66;
            // 
            // Type
            // 
            this.Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Type.DataPropertyName = "Type";
            this.Type.HeaderText = "Type";
            this.Type.MinimumWidth = 60;
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.Width = 60;
            // 
            // From
            // 
            this.From.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.From.DataPropertyName = "From";
            this.From.HeaderText = "From";
            this.From.MinimumWidth = 60;
            this.From.Name = "From";
            this.From.ReadOnly = true;
            this.From.Width = 60;
            // 
            // To
            // 
            this.To.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.To.DataPropertyName = "To";
            this.To.HeaderText = "To";
            this.To.MinimumWidth = 60;
            this.To.Name = "To";
            this.To.ReadOnly = true;
            this.To.Width = 60;
            // 
            // Carrier
            // 
            this.Carrier.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Carrier.DataPropertyName = "Carrier";
            this.Carrier.HeaderText = "Carrier";
            this.Carrier.MinimumWidth = 60;
            this.Carrier.Name = "Carrier";
            this.Carrier.ReadOnly = true;
            this.Carrier.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Carrier.Width = 60;
            // 
            // assignedSlotDataGridViewTextBoxColumn
            // 
            this.assignedSlotDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.assignedSlotDataGridViewTextBoxColumn.DataPropertyName = "AssignedSlot";
            this.assignedSlotDataGridViewTextBoxColumn.HeaderText = "Time slot";
            this.assignedSlotDataGridViewTextBoxColumn.MinimumWidth = 60;
            this.assignedSlotDataGridViewTextBoxColumn.Name = "assignedSlotDataGridViewTextBoxColumn";
            this.assignedSlotDataGridViewTextBoxColumn.ReadOnly = true;
            this.assignedSlotDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.assignedSlotDataGridViewTextBoxColumn.Width = 61;
            // 
            // encryptedDataGridViewTextBoxColumn
            // 
            this.encryptedDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.encryptedDataGridViewTextBoxColumn.DataPropertyName = "Encrypted";
            this.encryptedDataGridViewTextBoxColumn.HeaderText = "Encrypted";
            this.encryptedDataGridViewTextBoxColumn.MinimumWidth = 60;
            this.encryptedDataGridViewTextBoxColumn.Name = "encryptedDataGridViewTextBoxColumn";
            this.encryptedDataGridViewTextBoxColumn.ReadOnly = true;
            this.encryptedDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.encryptedDataGridViewTextBoxColumn.Width = 66;
            // 
            // duplexDataGridViewTextBoxColumn
            // 
            this.duplexDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.duplexDataGridViewTextBoxColumn.DataPropertyName = "Duplex";
            this.duplexDataGridViewTextBoxColumn.HeaderText = "Duplex";
            this.duplexDataGridViewTextBoxColumn.MinimumWidth = 60;
            this.duplexDataGridViewTextBoxColumn.Name = "duplexDataGridViewTextBoxColumn";
            this.duplexDataGridViewTextBoxColumn.ReadOnly = true;
            this.duplexDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // callsBindingSource
            // 
            this.callsBindingSource.AllowNew = false;
            this.callsBindingSource.DataSource = typeof(SDRSharp.Tetra.CallsDisplay);
            // 
            // callsTextBox
            // 
            this.callsTextBox.AcceptsReturn = true;
            this.callsTextBox.AcceptsTab = true;
            this.callsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.callsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.callsTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.callsTextBox.Location = new System.Drawing.Point(0, 0);
            this.callsTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.callsTextBox.Multiline = true;
            this.callsTextBox.Name = "callsTextBox";
            this.callsTextBox.ReadOnly = true;
            this.callsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.callsTextBox.Size = new System.Drawing.Size(614, 178);
            this.callsTextBox.TabIndex = 23;
            this.callsTextBox.WordWrap = false;
            // 
            // groupTabPage
            // 
            this.groupTabPage.Controls.Add(this.groupDataGridView);
            this.groupTabPage.Location = new System.Drawing.Point(4, 24);
            this.groupTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupTabPage.Name = "groupTabPage";
            this.groupTabPage.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupTabPage.Size = new System.Drawing.Size(624, 464);
            this.groupTabPage.TabIndex = 3;
            this.groupTabPage.Text = "Groups";
            this.groupTabPage.UseVisualStyleBackColor = true;
            // 
            // groupDataGridView
            // 
            this.groupDataGridView.AutoGenerateColumns = false;
            this.groupDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.groupDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.gSSIDataGridViewTextBoxColumn1,
            this.priorityDataGridViewTextBoxColumn1,
            this.nameDataGridViewTextBoxColumn1});
            this.groupDataGridView.DataSource = this.groupBindingSource;
            this.groupDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupDataGridView.Location = new System.Drawing.Point(4, 3);
            this.groupDataGridView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupDataGridView.Name = "groupDataGridView";
            this.groupDataGridView.RowHeadersVisible = false;
            this.groupDataGridView.Size = new System.Drawing.Size(616, 458);
            this.groupDataGridView.TabIndex = 0;
            // 
            // gSSIDataGridViewTextBoxColumn1
            // 
            this.gSSIDataGridViewTextBoxColumn1.DataPropertyName = "GSSI";
            this.gSSIDataGridViewTextBoxColumn1.HeaderText = "GSSI";
            this.gSSIDataGridViewTextBoxColumn1.Name = "gSSIDataGridViewTextBoxColumn1";
            // 
            // priorityDataGridViewTextBoxColumn1
            // 
            this.priorityDataGridViewTextBoxColumn1.DataPropertyName = "Priority";
            this.priorityDataGridViewTextBoxColumn1.HeaderText = "Priority";
            this.priorityDataGridViewTextBoxColumn1.Name = "priorityDataGridViewTextBoxColumn1";
            this.priorityDataGridViewTextBoxColumn1.Width = 50;
            // 
            // nameDataGridViewTextBoxColumn1
            // 
            this.nameDataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn1.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn1.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn1.MinimumWidth = 100;
            this.nameDataGridViewTextBoxColumn1.Name = "nameDataGridViewTextBoxColumn1";
            // 
            // groupBindingSource
            // 
            this.groupBindingSource.AllowNew = false;
            this.groupBindingSource.DataSource = typeof(SDRSharp.Tetra.GroupDisplay);
            // 
            // cellTabPage
            // 
            this.cellTabPage.Controls.Add(this.cellDataGridView);
            this.cellTabPage.Location = new System.Drawing.Point(4, 24);
            this.cellTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cellTabPage.Name = "cellTabPage";
            this.cellTabPage.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cellTabPage.Size = new System.Drawing.Size(624, 464);
            this.cellTabPage.TabIndex = 2;
            this.cellTabPage.Text = "Current cell";
            this.cellTabPage.UseVisualStyleBackColor = true;
            // 
            // cellDataGridView
            // 
            this.cellDataGridView.AutoGenerateColumns = false;
            this.cellDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cellDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.parameterDataGridViewTextBoxColumn,
            this.valueDataGridViewTextBoxColumn,
            this.commentDataGridViewTextBoxColumn});
            this.cellDataGridView.DataSource = this.cellBindingSource;
            this.cellDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cellDataGridView.Location = new System.Drawing.Point(4, 3);
            this.cellDataGridView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cellDataGridView.Name = "cellDataGridView";
            this.cellDataGridView.RowHeadersVisible = false;
            this.cellDataGridView.Size = new System.Drawing.Size(616, 458);
            this.cellDataGridView.TabIndex = 0;
            // 
            // parameterDataGridViewTextBoxColumn
            // 
            this.parameterDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.parameterDataGridViewTextBoxColumn.DataPropertyName = "Parameter";
            this.parameterDataGridViewTextBoxColumn.HeaderText = "Parameter";
            this.parameterDataGridViewTextBoxColumn.MinimumWidth = 100;
            this.parameterDataGridViewTextBoxColumn.Name = "parameterDataGridViewTextBoxColumn";
            // 
            // valueDataGridViewTextBoxColumn
            // 
            this.valueDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.valueDataGridViewTextBoxColumn.DataPropertyName = "Value";
            this.valueDataGridViewTextBoxColumn.HeaderText = "Value";
            this.valueDataGridViewTextBoxColumn.MinimumWidth = 40;
            this.valueDataGridViewTextBoxColumn.Name = "valueDataGridViewTextBoxColumn";
            this.valueDataGridViewTextBoxColumn.Width = 60;
            // 
            // commentDataGridViewTextBoxColumn
            // 
            this.commentDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.commentDataGridViewTextBoxColumn.DataPropertyName = "Comment";
            this.commentDataGridViewTextBoxColumn.HeaderText = "Comment";
            this.commentDataGridViewTextBoxColumn.MinimumWidth = 100;
            this.commentDataGridViewTextBoxColumn.Name = "commentDataGridViewTextBoxColumn";
            // 
            // cellBindingSource
            // 
            this.cellBindingSource.AllowNew = false;
            this.cellBindingSource.DataSource = typeof(SDRSharp.Tetra.CellDisplay);
            // 
            // neighbourTabPage
            // 
            this.neighbourTabPage.Controls.Add(this.neighbourDataGridView);
            this.neighbourTabPage.Location = new System.Drawing.Point(4, 24);
            this.neighbourTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.neighbourTabPage.Name = "neighbourTabPage";
            this.neighbourTabPage.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.neighbourTabPage.Size = new System.Drawing.Size(624, 464);
            this.neighbourTabPage.TabIndex = 1;
            this.neighbourTabPage.Text = "Neighbour cell";
            this.neighbourTabPage.UseVisualStyleBackColor = true;
            // 
            // neighbourDataGridView
            // 
            this.neighbourDataGridView.AllowDrop = true;
            this.neighbourDataGridView.AutoGenerateColumns = false;
            this.neighbourDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.neighbourDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.parameterDataGridViewTextBoxColumn2,
            this.cell1DataGridViewTextBoxColumn,
            this.cell2DataGridViewTextBoxColumn,
            this.cell3DataGridViewTextBoxColumn,
            this.cell4DataGridViewTextBoxColumn,
            this.cell5DataGridViewTextBoxColumn,
            this.cell6DataGridViewTextBoxColumn,
            this.cell7DataGridViewTextBoxColumn,
            this.cell8DataGridViewTextBoxColumn,
            this.cell9DataGridViewTextBoxColumn,
            this.cell10DataGridViewTextBoxColumn,
            this.cell11DataGridViewTextBoxColumn,
            this.cell12DataGridViewTextBoxColumn,
            this.cell13DataGridViewTextBoxColumn,
            this.cell14DataGridViewTextBoxColumn,
            this.cell15DataGridViewTextBoxColumn,
            this.cell16DataGridViewTextBoxColumn,
            this.cell17DataGridViewTextBoxColumn,
            this.cell18DataGridViewTextBoxColumn,
            this.cell19DataGridViewTextBoxColumn,
            this.cell20DataGridViewTextBoxColumn,
            this.cell21DataGridViewTextBoxColumn,
            this.cell22DataGridViewTextBoxColumn,
            this.cell23DataGridViewTextBoxColumn,
            this.cell24DataGridViewTextBoxColumn,
            this.cell25DataGridViewTextBoxColumn,
            this.cell26DataGridViewTextBoxColumn,
            this.cell27DataGridViewTextBoxColumn,
            this.cell28DataGridViewTextBoxColumn,
            this.cell29DataGridViewTextBoxColumn,
            this.cell30DataGridViewTextBoxColumn,
            this.cell31DataGridViewTextBoxColumn,
            this.cell32DataGridViewTextBoxColumn});
            this.neighbourDataGridView.DataSource = this.neighbourBindingSource;
            this.neighbourDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.neighbourDataGridView.Location = new System.Drawing.Point(4, 3);
            this.neighbourDataGridView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.neighbourDataGridView.Name = "neighbourDataGridView";
            this.neighbourDataGridView.RowHeadersVisible = false;
            this.neighbourDataGridView.Size = new System.Drawing.Size(616, 458);
            this.neighbourDataGridView.TabIndex = 0;
            // 
            // parameterDataGridViewTextBoxColumn2
            // 
            this.parameterDataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.parameterDataGridViewTextBoxColumn2.DataPropertyName = "Parameter";
            this.parameterDataGridViewTextBoxColumn2.HeaderText = "";
            this.parameterDataGridViewTextBoxColumn2.Name = "parameterDataGridViewTextBoxColumn2";
            this.parameterDataGridViewTextBoxColumn2.Width = 19;
            // 
            // cell1DataGridViewTextBoxColumn
            // 
            this.cell1DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell1DataGridViewTextBoxColumn.DataPropertyName = "Cell1";
            this.cell1DataGridViewTextBoxColumn.HeaderText = "1";
            this.cell1DataGridViewTextBoxColumn.Name = "cell1DataGridViewTextBoxColumn";
            this.cell1DataGridViewTextBoxColumn.Width = 38;
            // 
            // cell2DataGridViewTextBoxColumn
            // 
            this.cell2DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell2DataGridViewTextBoxColumn.DataPropertyName = "Cell2";
            this.cell2DataGridViewTextBoxColumn.HeaderText = "2";
            this.cell2DataGridViewTextBoxColumn.Name = "cell2DataGridViewTextBoxColumn";
            this.cell2DataGridViewTextBoxColumn.Width = 38;
            // 
            // cell3DataGridViewTextBoxColumn
            // 
            this.cell3DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell3DataGridViewTextBoxColumn.DataPropertyName = "Cell3";
            this.cell3DataGridViewTextBoxColumn.HeaderText = "3";
            this.cell3DataGridViewTextBoxColumn.Name = "cell3DataGridViewTextBoxColumn";
            this.cell3DataGridViewTextBoxColumn.Width = 38;
            // 
            // cell4DataGridViewTextBoxColumn
            // 
            this.cell4DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell4DataGridViewTextBoxColumn.DataPropertyName = "Cell4";
            this.cell4DataGridViewTextBoxColumn.HeaderText = "4";
            this.cell4DataGridViewTextBoxColumn.Name = "cell4DataGridViewTextBoxColumn";
            this.cell4DataGridViewTextBoxColumn.Width = 38;
            // 
            // cell5DataGridViewTextBoxColumn
            // 
            this.cell5DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell5DataGridViewTextBoxColumn.DataPropertyName = "Cell5";
            this.cell5DataGridViewTextBoxColumn.HeaderText = "5";
            this.cell5DataGridViewTextBoxColumn.Name = "cell5DataGridViewTextBoxColumn";
            this.cell5DataGridViewTextBoxColumn.Width = 38;
            // 
            // cell6DataGridViewTextBoxColumn
            // 
            this.cell6DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell6DataGridViewTextBoxColumn.DataPropertyName = "Cell6";
            this.cell6DataGridViewTextBoxColumn.HeaderText = "6";
            this.cell6DataGridViewTextBoxColumn.Name = "cell6DataGridViewTextBoxColumn";
            this.cell6DataGridViewTextBoxColumn.Width = 38;
            // 
            // cell7DataGridViewTextBoxColumn
            // 
            this.cell7DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell7DataGridViewTextBoxColumn.DataPropertyName = "Cell7";
            this.cell7DataGridViewTextBoxColumn.HeaderText = "7";
            this.cell7DataGridViewTextBoxColumn.Name = "cell7DataGridViewTextBoxColumn";
            this.cell7DataGridViewTextBoxColumn.Width = 38;
            // 
            // cell8DataGridViewTextBoxColumn
            // 
            this.cell8DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell8DataGridViewTextBoxColumn.DataPropertyName = "Cell8";
            this.cell8DataGridViewTextBoxColumn.HeaderText = "8";
            this.cell8DataGridViewTextBoxColumn.Name = "cell8DataGridViewTextBoxColumn";
            this.cell8DataGridViewTextBoxColumn.Width = 38;
            // 
            // cell9DataGridViewTextBoxColumn
            // 
            this.cell9DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell9DataGridViewTextBoxColumn.DataPropertyName = "Cell9";
            this.cell9DataGridViewTextBoxColumn.HeaderText = "9";
            this.cell9DataGridViewTextBoxColumn.Name = "cell9DataGridViewTextBoxColumn";
            this.cell9DataGridViewTextBoxColumn.Width = 38;
            // 
            // cell10DataGridViewTextBoxColumn
            // 
            this.cell10DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell10DataGridViewTextBoxColumn.DataPropertyName = "Cell10";
            this.cell10DataGridViewTextBoxColumn.HeaderText = "10";
            this.cell10DataGridViewTextBoxColumn.Name = "cell10DataGridViewTextBoxColumn";
            this.cell10DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell11DataGridViewTextBoxColumn
            // 
            this.cell11DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell11DataGridViewTextBoxColumn.DataPropertyName = "Cell11";
            this.cell11DataGridViewTextBoxColumn.HeaderText = "11";
            this.cell11DataGridViewTextBoxColumn.Name = "cell11DataGridViewTextBoxColumn";
            this.cell11DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell12DataGridViewTextBoxColumn
            // 
            this.cell12DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell12DataGridViewTextBoxColumn.DataPropertyName = "Cell12";
            this.cell12DataGridViewTextBoxColumn.HeaderText = "12";
            this.cell12DataGridViewTextBoxColumn.Name = "cell12DataGridViewTextBoxColumn";
            this.cell12DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell13DataGridViewTextBoxColumn
            // 
            this.cell13DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell13DataGridViewTextBoxColumn.DataPropertyName = "Cell13";
            this.cell13DataGridViewTextBoxColumn.HeaderText = "13";
            this.cell13DataGridViewTextBoxColumn.Name = "cell13DataGridViewTextBoxColumn";
            this.cell13DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell14DataGridViewTextBoxColumn
            // 
            this.cell14DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell14DataGridViewTextBoxColumn.DataPropertyName = "Cell14";
            this.cell14DataGridViewTextBoxColumn.HeaderText = "14";
            this.cell14DataGridViewTextBoxColumn.Name = "cell14DataGridViewTextBoxColumn";
            this.cell14DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell15DataGridViewTextBoxColumn
            // 
            this.cell15DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell15DataGridViewTextBoxColumn.DataPropertyName = "Cell15";
            this.cell15DataGridViewTextBoxColumn.HeaderText = "15";
            this.cell15DataGridViewTextBoxColumn.Name = "cell15DataGridViewTextBoxColumn";
            this.cell15DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell16DataGridViewTextBoxColumn
            // 
            this.cell16DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell16DataGridViewTextBoxColumn.DataPropertyName = "Cell16";
            this.cell16DataGridViewTextBoxColumn.HeaderText = "16";
            this.cell16DataGridViewTextBoxColumn.Name = "cell16DataGridViewTextBoxColumn";
            this.cell16DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell17DataGridViewTextBoxColumn
            // 
            this.cell17DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell17DataGridViewTextBoxColumn.DataPropertyName = "Cell17";
            this.cell17DataGridViewTextBoxColumn.HeaderText = "17";
            this.cell17DataGridViewTextBoxColumn.Name = "cell17DataGridViewTextBoxColumn";
            this.cell17DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell18DataGridViewTextBoxColumn
            // 
            this.cell18DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell18DataGridViewTextBoxColumn.DataPropertyName = "Cell18";
            this.cell18DataGridViewTextBoxColumn.HeaderText = "18";
            this.cell18DataGridViewTextBoxColumn.Name = "cell18DataGridViewTextBoxColumn";
            this.cell18DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell19DataGridViewTextBoxColumn
            // 
            this.cell19DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell19DataGridViewTextBoxColumn.DataPropertyName = "Cell19";
            this.cell19DataGridViewTextBoxColumn.HeaderText = "19";
            this.cell19DataGridViewTextBoxColumn.Name = "cell19DataGridViewTextBoxColumn";
            this.cell19DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell20DataGridViewTextBoxColumn
            // 
            this.cell20DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell20DataGridViewTextBoxColumn.DataPropertyName = "Cell20";
            this.cell20DataGridViewTextBoxColumn.HeaderText = "20";
            this.cell20DataGridViewTextBoxColumn.Name = "cell20DataGridViewTextBoxColumn";
            this.cell20DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell21DataGridViewTextBoxColumn
            // 
            this.cell21DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell21DataGridViewTextBoxColumn.DataPropertyName = "Cell21";
            this.cell21DataGridViewTextBoxColumn.HeaderText = "21";
            this.cell21DataGridViewTextBoxColumn.Name = "cell21DataGridViewTextBoxColumn";
            this.cell21DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell22DataGridViewTextBoxColumn
            // 
            this.cell22DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell22DataGridViewTextBoxColumn.DataPropertyName = "Cell22";
            this.cell22DataGridViewTextBoxColumn.HeaderText = "22";
            this.cell22DataGridViewTextBoxColumn.Name = "cell22DataGridViewTextBoxColumn";
            this.cell22DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell23DataGridViewTextBoxColumn
            // 
            this.cell23DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell23DataGridViewTextBoxColumn.DataPropertyName = "Cell23";
            this.cell23DataGridViewTextBoxColumn.HeaderText = "23";
            this.cell23DataGridViewTextBoxColumn.Name = "cell23DataGridViewTextBoxColumn";
            this.cell23DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell24DataGridViewTextBoxColumn
            // 
            this.cell24DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell24DataGridViewTextBoxColumn.DataPropertyName = "Cell24";
            this.cell24DataGridViewTextBoxColumn.HeaderText = "24";
            this.cell24DataGridViewTextBoxColumn.Name = "cell24DataGridViewTextBoxColumn";
            this.cell24DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell25DataGridViewTextBoxColumn
            // 
            this.cell25DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell25DataGridViewTextBoxColumn.DataPropertyName = "Cell25";
            this.cell25DataGridViewTextBoxColumn.HeaderText = "25";
            this.cell25DataGridViewTextBoxColumn.Name = "cell25DataGridViewTextBoxColumn";
            this.cell25DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell26DataGridViewTextBoxColumn
            // 
            this.cell26DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell26DataGridViewTextBoxColumn.DataPropertyName = "Cell26";
            this.cell26DataGridViewTextBoxColumn.HeaderText = "26";
            this.cell26DataGridViewTextBoxColumn.Name = "cell26DataGridViewTextBoxColumn";
            this.cell26DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell27DataGridViewTextBoxColumn
            // 
            this.cell27DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell27DataGridViewTextBoxColumn.DataPropertyName = "Cell27";
            this.cell27DataGridViewTextBoxColumn.HeaderText = "27";
            this.cell27DataGridViewTextBoxColumn.Name = "cell27DataGridViewTextBoxColumn";
            this.cell27DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell28DataGridViewTextBoxColumn
            // 
            this.cell28DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell28DataGridViewTextBoxColumn.DataPropertyName = "Cell28";
            this.cell28DataGridViewTextBoxColumn.HeaderText = "28";
            this.cell28DataGridViewTextBoxColumn.Name = "cell28DataGridViewTextBoxColumn";
            this.cell28DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell29DataGridViewTextBoxColumn
            // 
            this.cell29DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell29DataGridViewTextBoxColumn.DataPropertyName = "Cell29";
            this.cell29DataGridViewTextBoxColumn.HeaderText = "29";
            this.cell29DataGridViewTextBoxColumn.Name = "cell29DataGridViewTextBoxColumn";
            this.cell29DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell30DataGridViewTextBoxColumn
            // 
            this.cell30DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell30DataGridViewTextBoxColumn.DataPropertyName = "Cell30";
            this.cell30DataGridViewTextBoxColumn.HeaderText = "30";
            this.cell30DataGridViewTextBoxColumn.Name = "cell30DataGridViewTextBoxColumn";
            this.cell30DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell31DataGridViewTextBoxColumn
            // 
            this.cell31DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell31DataGridViewTextBoxColumn.DataPropertyName = "Cell31";
            this.cell31DataGridViewTextBoxColumn.HeaderText = "31";
            this.cell31DataGridViewTextBoxColumn.Name = "cell31DataGridViewTextBoxColumn";
            this.cell31DataGridViewTextBoxColumn.Width = 44;
            // 
            // cell32DataGridViewTextBoxColumn
            // 
            this.cell32DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.cell32DataGridViewTextBoxColumn.DataPropertyName = "Cell32";
            this.cell32DataGridViewTextBoxColumn.HeaderText = "32";
            this.cell32DataGridViewTextBoxColumn.Name = "cell32DataGridViewTextBoxColumn";
            this.cell32DataGridViewTextBoxColumn.Width = 44;
            // 
            // neighbourBindingSource
            // 
            this.neighbourBindingSource.AllowNew = false;
            this.neighbourBindingSource.DataSource = typeof(SDRSharp.Tetra.NeighbourDisplay);
            // 
            // parameterDataGridViewTextBoxColumn1
            // 
            this.parameterDataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.parameterDataGridViewTextBoxColumn1.DataPropertyName = "Parameter";
            this.parameterDataGridViewTextBoxColumn1.HeaderText = "Parameter";
            this.parameterDataGridViewTextBoxColumn1.Name = "parameterDataGridViewTextBoxColumn1";
            this.parameterDataGridViewTextBoxColumn1.ReadOnly = true;
            this.parameterDataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Cell1
            // 
            this.Cell1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell1.DataPropertyName = "Cell1";
            this.Cell1.HeaderText = "Cell1";
            this.Cell1.Name = "Cell1";
            this.Cell1.ReadOnly = true;
            // 
            // Cell2
            // 
            this.Cell2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell2.DataPropertyName = "Cell2";
            this.Cell2.HeaderText = "Cell2";
            this.Cell2.Name = "Cell2";
            this.Cell2.ReadOnly = true;
            // 
            // Cell3
            // 
            this.Cell3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell3.DataPropertyName = "Cell3";
            this.Cell3.HeaderText = "Cell3";
            this.Cell3.Name = "Cell3";
            this.Cell3.ReadOnly = true;
            // 
            // Cell4
            // 
            this.Cell4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell4.DataPropertyName = "Cell4";
            this.Cell4.HeaderText = "Cell4";
            this.Cell4.Name = "Cell4";
            this.Cell4.ReadOnly = true;
            // 
            // Cell5
            // 
            this.Cell5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell5.DataPropertyName = "Cell5";
            this.Cell5.HeaderText = "Cell5";
            this.Cell5.Name = "Cell5";
            this.Cell5.ReadOnly = true;
            // 
            // Cell6
            // 
            this.Cell6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell6.DataPropertyName = "Cell6";
            this.Cell6.HeaderText = "Cell6";
            this.Cell6.Name = "Cell6";
            this.Cell6.ReadOnly = true;
            // 
            // Cell7
            // 
            this.Cell7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell7.DataPropertyName = "Cell7";
            this.Cell7.HeaderText = "Cell7";
            this.Cell7.Name = "Cell7";
            this.Cell7.ReadOnly = true;
            // 
            // Cell8
            // 
            this.Cell8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell8.DataPropertyName = "Cell8";
            this.Cell8.HeaderText = "Cell8";
            this.Cell8.Name = "Cell8";
            this.Cell8.ReadOnly = true;
            // 
            // Cell9
            // 
            this.Cell9.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell9.DataPropertyName = "Cell9";
            this.Cell9.HeaderText = "Cell9";
            this.Cell9.Name = "Cell9";
            this.Cell9.ReadOnly = true;
            // 
            // Cell10
            // 
            this.Cell10.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell10.DataPropertyName = "Cell10";
            this.Cell10.HeaderText = "Cell10";
            this.Cell10.Name = "Cell10";
            this.Cell10.ReadOnly = true;
            // 
            // Cell11
            // 
            this.Cell11.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell11.DataPropertyName = "Cell11";
            this.Cell11.HeaderText = "Cell11";
            this.Cell11.Name = "Cell11";
            this.Cell11.ReadOnly = true;
            // 
            // Cell12
            // 
            this.Cell12.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell12.DataPropertyName = "Cell12";
            this.Cell12.HeaderText = "Cell12";
            this.Cell12.Name = "Cell12";
            this.Cell12.ReadOnly = true;
            // 
            // Cell13
            // 
            this.Cell13.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell13.DataPropertyName = "Cell13";
            this.Cell13.HeaderText = "Cell13";
            this.Cell13.Name = "Cell13";
            this.Cell13.ReadOnly = true;
            // 
            // Cell14
            // 
            this.Cell14.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell14.DataPropertyName = "Cell14";
            this.Cell14.HeaderText = "Cell14";
            this.Cell14.Name = "Cell14";
            this.Cell14.ReadOnly = true;
            // 
            // Cell15
            // 
            this.Cell15.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell15.DataPropertyName = "Cell15";
            this.Cell15.HeaderText = "Cell15";
            this.Cell15.Name = "Cell15";
            this.Cell15.ReadOnly = true;
            // 
            // Cell16
            // 
            this.Cell16.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell16.DataPropertyName = "Cell16";
            this.Cell16.HeaderText = "Cell16";
            this.Cell16.Name = "Cell16";
            this.Cell16.ReadOnly = true;
            // 
            // Cell17
            // 
            this.Cell17.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell17.DataPropertyName = "Cell17";
            this.Cell17.HeaderText = "Cell17";
            this.Cell17.Name = "Cell17";
            this.Cell17.ReadOnly = true;
            // 
            // Cell18
            // 
            this.Cell18.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell18.DataPropertyName = "Cell18";
            this.Cell18.HeaderText = "Cell18";
            this.Cell18.Name = "Cell18";
            this.Cell18.ReadOnly = true;
            // 
            // Cell19
            // 
            this.Cell19.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell19.DataPropertyName = "Cell19";
            this.Cell19.HeaderText = "Cell19";
            this.Cell19.Name = "Cell19";
            this.Cell19.ReadOnly = true;
            // 
            // Cell20
            // 
            this.Cell20.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell20.DataPropertyName = "Cell20";
            this.Cell20.HeaderText = "Cell20";
            this.Cell20.Name = "Cell20";
            this.Cell20.ReadOnly = true;
            // 
            // Cell21
            // 
            this.Cell21.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell21.DataPropertyName = "Cell21";
            this.Cell21.HeaderText = "Cell21";
            this.Cell21.Name = "Cell21";
            this.Cell21.ReadOnly = true;
            // 
            // Cell22
            // 
            this.Cell22.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell22.DataPropertyName = "Cell22";
            this.Cell22.HeaderText = "Cell22";
            this.Cell22.Name = "Cell22";
            this.Cell22.ReadOnly = true;
            // 
            // Cell23
            // 
            this.Cell23.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell23.DataPropertyName = "Cell23";
            this.Cell23.HeaderText = "Cell23";
            this.Cell23.Name = "Cell23";
            this.Cell23.ReadOnly = true;
            // 
            // Cell24
            // 
            this.Cell24.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell24.DataPropertyName = "Cell24";
            this.Cell24.HeaderText = "Cell24";
            this.Cell24.Name = "Cell24";
            this.Cell24.ReadOnly = true;
            // 
            // Cell25
            // 
            this.Cell25.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell25.DataPropertyName = "Cell25";
            this.Cell25.HeaderText = "Cell25";
            this.Cell25.Name = "Cell25";
            this.Cell25.ReadOnly = true;
            // 
            // Cell26
            // 
            this.Cell26.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell26.DataPropertyName = "Cell26";
            this.Cell26.HeaderText = "Cell26";
            this.Cell26.Name = "Cell26";
            this.Cell26.ReadOnly = true;
            // 
            // Cell27
            // 
            this.Cell27.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell27.DataPropertyName = "Cell27";
            this.Cell27.HeaderText = "Cell27";
            this.Cell27.Name = "Cell27";
            this.Cell27.ReadOnly = true;
            // 
            // Cell28
            // 
            this.Cell28.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell28.DataPropertyName = "Cell28";
            this.Cell28.HeaderText = "Cell28";
            this.Cell28.Name = "Cell28";
            this.Cell28.ReadOnly = true;
            // 
            // Cell29
            // 
            this.Cell29.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell29.DataPropertyName = "Cell29";
            this.Cell29.HeaderText = "Cell29";
            this.Cell29.Name = "Cell29";
            this.Cell29.ReadOnly = true;
            // 
            // Cell30
            // 
            this.Cell30.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell30.DataPropertyName = "Cell30";
            this.Cell30.HeaderText = "Cell30";
            this.Cell30.Name = "Cell30";
            this.Cell30.ReadOnly = true;
            // 
            // Cell31
            // 
            this.Cell31.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell31.DataPropertyName = "Cell31";
            this.Cell31.HeaderText = "Cell31";
            this.Cell31.Name = "Cell31";
            this.Cell31.ReadOnly = true;
            // 
            // Cell32
            // 
            this.Cell32.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.Cell32.DataPropertyName = "Cell32";
            this.Cell32.HeaderText = "Cell32";
            this.Cell32.Name = "Cell32";
            this.Cell32.ReadOnly = true;
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(575, 13);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(63, 19);
            this.checkBox1.TabIndex = 27;
            this.checkBox1.Text = "On top";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // NetInfoWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 519);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.tabControl1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "NetInfoWindow";
            this.Text = "Network Info";
            this.tabControl1.ResumeLayout(false);
            this.callsTabPage.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.callsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.callsBindingSource)).EndInit();
            this.groupTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBindingSource)).EndInit();
            this.cellTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cellDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cellBindingSource)).EndInit();
            this.neighbourTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.neighbourDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.neighbourBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
