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
      this.neighbourBindingSource.DataSource = (object) this._neighbourEntries;
      this.callsBindingSource.DataSource = (object) this._callsEntries;
      this.groupBindingSource.DataSource = (object) this._groupEntries;
      this.cellBindingSource.DataSource = (object) this._cellEntries;
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
      this.UpdateCallsGrid(calls.ToDictionary<KeyValuePair<int, CallsEntry>, int, CallsEntry>((Func<KeyValuePair<int, CallsEntry>, int>) (entry => entry.Key), (Func<KeyValuePair<int, CallsEntry>, CallsEntry>) (entry => entry.Value)));
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
        GlobalNames name = (GlobalNames) index;
        if (syncInfo.Contains(name))
        {
          int num = syncInfo.Value(name);
          bool flag = false;
          foreach (CellDisplay cellEntry in (Collection<CellDisplay>) this._cellEntries)
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
        GlobalNames name = (GlobalNames) index;
        if (sysInfo.Contains(name))
        {
          int num = sysInfo.Value(name);
          bool flag = false;
          foreach (CellDisplay cellEntry in (Collection<CellDisplay>) this._cellEntries)
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
      if (this.tabControl1.SelectedTab.Name != "neighbourTabPage" || GlobalFunction.NeighbourList.Count == 0)
        return;
      while (GlobalFunction.NeighbourList.Count > 0)
      {
        ReceivedData neighbour = GlobalFunction.NeighbourList[0];
        int cellId = neighbour.Value(GlobalNames.Cell_identifier);
        for (int index = 0; index < neighbour.Data.Length; ++index)
        {
          GlobalNames name = (GlobalNames) index;
          if (neighbour.Contains(name))
          {
            int num = neighbour.Value(name);
            bool flag = false;
            foreach (NeighbourDisplay neighbourEntry in (Collection<NeighbourDisplay>) this._neighbourEntries)
            {
              if (neighbourEntry.Parameter == name)
              {
                flag = true;
                object cellValue = this.GetCellValue(neighbourEntry, cellId);
                if (cellValue != null)
                {
                  if ((int) cellValue == num)
                    break;
                }
                this.SetCellValue(neighbourEntry, cellId, (object) num);
                break;
              }
            }
            if (!flag)
            {
              NeighbourDisplay entry = new NeighbourDisplay()
              {
                Parameter = name
              };
              this.SetCellValue(entry, cellId, (object) num);
              this._neighbourEntries.Add(entry);
            }
          }
        }
        GlobalFunction.NeighbourList.RemoveAt(0);
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
          return (object) (int) parameter.Cell32;
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
            str1 = str1 + " " + ((CmcePrimitivesType) num1).ToString();
          if (receivedData.TryGetValue(GlobalNames.Transmission_grant, ref num1))
            str1 = str1 + " Transmission " + ((TransmissionGranted) num1).ToString();
          if (receivedData.TryGetValue(GlobalNames.Calling_party_address_SSI, ref num1))
            str1 = str1 + " Party_SSI:" + num1.ToString();
          if (receivedData.TryGetValue(GlobalNames.Transmitting_party_address_SSI, ref num1))
            str1 = str1 + " Party_SSI:" + num1.ToString();
          if (receivedData.TryGetValue(GlobalNames.Basic_service_Communication_type, ref num1))
            str1 = str1 + " Basic_service:" + ((CommunicationType) num1).ToString();
          if (receivedData.TryGetValue(GlobalNames.Basic_service_Encryption_flag, ref num1))
            str1 += num1 == 0 ? " Clear" : " E2EE";
          if (receivedData.TryGetValue(GlobalNames.Basic_service_Circuit_mode_type, ref num1))
            str1 = str1 + " " + ((CircuitModeType) num1).ToString();
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
                  empty2 = ((uint) num1).ToString();
                str1 = str1 + " Length:" + num3.ToString() + " Data:" + empty2;
                break;
              case 2:
                int num4 = 64;
                ulong num5 = 0;
                if (receivedData.TryGetValue(GlobalNames.User_Defined_Data_64_1, ref num1))
                {
                  num5 = (ulong) num1 << 32;
                  empty2 = num5.ToString();
                }
                if (receivedData.TryGetValue(GlobalNames.User_Defined_Data_64_2, ref num1))
                {
                  num5 = (ulong) num1;
                  empty2 = num5.ToString();
                }
                str1 = str1 + " Length:" + num4.ToString() + " Data:" + empty2;
                break;
            }
            if (receivedData.TryGetValue(GlobalNames.Protocol_identifier, ref num1))
              str1 = str1 + " Protocol:" + ((SdsProtocolIdent) num1).ToString();
            if (receivedData.TryGetValue(GlobalNames.Location_PDU_type_extension, ref num1))
              str1 = str1 + " SubType:" + ((LocationTypeExtension) num1).ToString();
            if (receivedData.TryGetValue(GlobalNames.Latitude, ref num1))
            {
              double num6 = (double) num1 * 1.07288360595703E-05;
              if (num6 >= 90.0)
                num6 -= 180.0;
              str1 += string.Format(" Lat:{0:0.000000}°", (object) num6);
            }
            if (receivedData.TryGetValue(GlobalNames.Longitude, ref num1))
            {
              double num6 = (double) num1 * 1.07288360595703E-05;
              if (num6 >= 180.0)
                num6 -= 360.0;
              str1 += string.Format(" Long:{0:0.000000}°", (object) num6);
            }
            if (receivedData.TryGetValue(GlobalNames.Position_error, ref num1))
              str1 = str1 + " Accuracy:" + (num1 * 10 * 2).ToString() + "m";
            if (receivedData.TryGetValue(GlobalNames.Horizontal_velocity, ref num1))
            {
              double num6 = num1 <= 28 ? (double) num1 : 16.0 * Math.Pow(1.038, (double) (num1 - 13));
              str1 += string.Format(" Velocity:{0:0.0}km/h", (object) num6);
            }
            if (receivedData.TryGetValue(GlobalNames.Direction_of_travel, ref num1))
              str1 = str1 + " Dir:" + ((double) num1 * 22.5).ToString() + "°";
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
      GlobalFunction.NeighbourList.Clear();
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
        callsDisplay.Type = ((CommunicationType) call.Value.Type).ToString();
        callsDisplay.Encrypted = call.Value.IsClear == 1 ? " Clear" : " Encrypted";
        callsDisplay.Duplex = call.Value.Duplex == 0 ? " Simplex" : " Duplex";
        callsDisplay.From = call.Value.From;
        callsDisplay.To = to.ToString();
        if (this._groupEntries != null && this._groupEntries.Count > 0)
        {
          foreach (GroupDisplay groupEntry in (Collection<GroupDisplay>) this._groupEntries)
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
      this.components = (IContainer) new Container();
      DataGridViewCellStyle gridViewCellStyle1 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle2 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle3 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle4 = new DataGridViewCellStyle();
      this.timeOutTimer = new Timer(this.components);
      this.tabControl1 = new TabControl();
      this.callsTabPage = new TabPage();
      this.splitContainer2 = new SplitContainer();
      this.callsDataGridView = new DataGridViewEx();
      this.callIDDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.Type = new DataGridViewTextBoxColumn();
      this.From = new DataGridViewTextBoxColumn();
      this.To = new DataGridViewTextBoxColumn();
      this.Carrier = new DataGridViewTextBoxColumn();
      this.assignedSlotDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.encryptedDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.duplexDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.callsBindingSource = new BindingSource(this.components);
      this.callsTextBox = new TextBox();
      this.groupTabPage = new TabPage();
      this.groupDataGridView = new DataGridViewEx();
      this.gSSIDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
      this.priorityDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
      this.nameDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
      this.groupBindingSource = new BindingSource(this.components);
      this.cellTabPage = new TabPage();
      this.cellDataGridView = new DataGridView();
      this.parameterDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.valueDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.commentDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cellBindingSource = new BindingSource(this.components);
      this.neighbourTabPage = new TabPage();
      this.neighbourBindingSource = new BindingSource(this.components);
      this.parameterDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
      this.Cell1 = new DataGridViewTextBoxColumn();
      this.Cell2 = new DataGridViewTextBoxColumn();
      this.Cell3 = new DataGridViewTextBoxColumn();
      this.Cell4 = new DataGridViewTextBoxColumn();
      this.Cell5 = new DataGridViewTextBoxColumn();
      this.Cell6 = new DataGridViewTextBoxColumn();
      this.Cell7 = new DataGridViewTextBoxColumn();
      this.Cell8 = new DataGridViewTextBoxColumn();
      this.Cell9 = new DataGridViewTextBoxColumn();
      this.Cell10 = new DataGridViewTextBoxColumn();
      this.Cell11 = new DataGridViewTextBoxColumn();
      this.Cell12 = new DataGridViewTextBoxColumn();
      this.Cell13 = new DataGridViewTextBoxColumn();
      this.Cell14 = new DataGridViewTextBoxColumn();
      this.Cell15 = new DataGridViewTextBoxColumn();
      this.Cell16 = new DataGridViewTextBoxColumn();
      this.Cell17 = new DataGridViewTextBoxColumn();
      this.Cell18 = new DataGridViewTextBoxColumn();
      this.Cell19 = new DataGridViewTextBoxColumn();
      this.Cell20 = new DataGridViewTextBoxColumn();
      this.Cell21 = new DataGridViewTextBoxColumn();
      this.Cell22 = new DataGridViewTextBoxColumn();
      this.Cell23 = new DataGridViewTextBoxColumn();
      this.Cell24 = new DataGridViewTextBoxColumn();
      this.Cell25 = new DataGridViewTextBoxColumn();
      this.Cell26 = new DataGridViewTextBoxColumn();
      this.Cell27 = new DataGridViewTextBoxColumn();
      this.Cell28 = new DataGridViewTextBoxColumn();
      this.Cell29 = new DataGridViewTextBoxColumn();
      this.Cell30 = new DataGridViewTextBoxColumn();
      this.Cell31 = new DataGridViewTextBoxColumn();
      this.Cell32 = new DataGridViewTextBoxColumn();
      this.checkBox1 = new CheckBox();
      this.cell32DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell31DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell30DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell29DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell28DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell27DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell26DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell25DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell24DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell23DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell22DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell21DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell20DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell19DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell18DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell17DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell16DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell15DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell14DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell13DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell12DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell11DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell10DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell9DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell8DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell7DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell6DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell5DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell4DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell3DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell2DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.cell1DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
      this.parameterDataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
      this.neighbourDataGridView = new DataGridViewEx();
      this.tabControl1.SuspendLayout();
      this.callsTabPage.SuspendLayout();
      this.splitContainer2.BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      ((ISupportInitialize) this.callsDataGridView).BeginInit();
      ((ISupportInitialize) this.callsBindingSource).BeginInit();
      this.groupTabPage.SuspendLayout();
      ((ISupportInitialize) this.groupDataGridView).BeginInit();
      ((ISupportInitialize) this.groupBindingSource).BeginInit();
      this.cellTabPage.SuspendLayout();
      ((ISupportInitialize) this.cellDataGridView).BeginInit();
      ((ISupportInitialize) this.cellBindingSource).BeginInit();
      this.neighbourTabPage.SuspendLayout();
      ((ISupportInitialize) this.neighbourBindingSource).BeginInit();
      ((ISupportInitialize) this.neighbourDataGridView).BeginInit();
      this.SuspendLayout();
      this.timeOutTimer.Enabled = true;
      this.timeOutTimer.Interval = 50;
      this.tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.tabControl1.Controls.Add((Control) this.callsTabPage);
      this.tabControl1.Controls.Add((Control) this.groupTabPage);
      this.tabControl1.Controls.Add((Control) this.cellTabPage);
      this.tabControl1.Controls.Add((Control) this.neighbourTabPage);
      this.tabControl1.Location = new Point(12, 12);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new Size(542, 426);
      this.tabControl1.TabIndex = 23;
      this.callsTabPage.Controls.Add((Control) this.splitContainer2);
      this.callsTabPage.Location = new Point(4, 22);
      this.callsTabPage.Name = "callsTabPage";
      this.callsTabPage.Padding = new Padding(3);
      this.callsTabPage.Size = new Size(534, 400);
      this.callsTabPage.TabIndex = 0;
      this.callsTabPage.Text = "Calls";
      this.callsTabPage.UseVisualStyleBackColor = true;
      this.splitContainer2.BorderStyle = BorderStyle.FixedSingle;
      this.splitContainer2.Dock = DockStyle.Fill;
      this.splitContainer2.Location = new Point(3, 3);
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Orientation = Orientation.Horizontal;
      this.splitContainer2.Panel1.Controls.Add((Control) this.callsDataGridView);
      this.splitContainer2.Panel2.Controls.Add((Control) this.callsTextBox);
      this.splitContainer2.Size = new Size(528, 394);
      this.splitContainer2.SplitterDistance = 235;
      this.splitContainer2.TabIndex = 25;
      this.callsDataGridView.AllowUserToAddRows = false;
      this.callsDataGridView.AllowUserToDeleteRows = false;
      this.callsDataGridView.AllowUserToOrderColumns = true;
      gridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 204);
      this.callsDataGridView.AlternatingRowsDefaultCellStyle = gridViewCellStyle1;
      this.callsDataGridView.AutoGenerateColumns = false;
      this.callsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.callsDataGridView.Columns.AddRange((DataGridViewColumn) this.callIDDataGridViewTextBoxColumn, (DataGridViewColumn) this.Type, (DataGridViewColumn) this.From, (DataGridViewColumn) this.To, (DataGridViewColumn) this.Carrier, (DataGridViewColumn) this.assignedSlotDataGridViewTextBoxColumn, (DataGridViewColumn) this.encryptedDataGridViewTextBoxColumn, (DataGridViewColumn) this.duplexDataGridViewTextBoxColumn);
      this.callsDataGridView.DataSource = (object) this.callsBindingSource;
      gridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle2.BackColor = SystemColors.Window;
      gridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 204);
      gridViewCellStyle2.ForeColor = SystemColors.ControlText;
      gridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle2.WrapMode = DataGridViewTriState.False;
      this.callsDataGridView.DefaultCellStyle = gridViewCellStyle2;
      this.callsDataGridView.Dock = DockStyle.Fill;
      this.callsDataGridView.Location = new Point(0, 0);
      this.callsDataGridView.Name = "callsDataGridView";
      this.callsDataGridView.ReadOnly = true;
      this.callsDataGridView.RowHeadersVisible = false;
      this.callsDataGridView.RowTemplate.DefaultCellStyle.ForeColor = Color.Black;
      this.callsDataGridView.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.Blue;
      this.callsDataGridView.ShowCellErrors = false;
      this.callsDataGridView.ShowRowErrors = false;
      this.callsDataGridView.Size = new Size(526, 233);
      this.callsDataGridView.TabIndex = 0;
      this.callsDataGridView.VirtualMode = true;
      this.callIDDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      this.callIDDataGridViewTextBoxColumn.DataPropertyName = "CallID";
      this.callIDDataGridViewTextBoxColumn.HeaderText = "Call ID";
      this.callIDDataGridViewTextBoxColumn.MinimumWidth = 60;
      this.callIDDataGridViewTextBoxColumn.Name = "callIDDataGridViewTextBoxColumn";
      this.callIDDataGridViewTextBoxColumn.ReadOnly = true;
      this.callIDDataGridViewTextBoxColumn.Width = 63;
      this.Type.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      this.Type.DataPropertyName = "Type";
      this.Type.HeaderText = "Type";
      this.Type.MinimumWidth = 60;
      this.Type.Name = "Type";
      this.Type.ReadOnly = true;
      this.Type.Width = 60;
      this.From.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      this.From.DataPropertyName = "From";
      this.From.HeaderText = "From";
      this.From.MinimumWidth = 60;
      this.From.Name = "From";
      this.From.ReadOnly = true;
      this.From.Width = 60;
      this.To.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      this.To.DataPropertyName = "To";
      this.To.HeaderText = "To";
      this.To.MinimumWidth = 60;
      this.To.Name = "To";
      this.To.ReadOnly = true;
      this.To.Width = 60;
      this.Carrier.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      this.Carrier.DataPropertyName = "Carrier";
      this.Carrier.HeaderText = "Carrier";
      this.Carrier.MinimumWidth = 60;
      this.Carrier.Name = "Carrier";
      this.Carrier.ReadOnly = true;
      this.Carrier.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.Carrier.Width = 60;
      this.assignedSlotDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      this.assignedSlotDataGridViewTextBoxColumn.DataPropertyName = "AssignedSlot";
      this.assignedSlotDataGridViewTextBoxColumn.HeaderText = "Time slot";
      this.assignedSlotDataGridViewTextBoxColumn.MinimumWidth = 60;
      this.assignedSlotDataGridViewTextBoxColumn.Name = "assignedSlotDataGridViewTextBoxColumn";
      this.assignedSlotDataGridViewTextBoxColumn.ReadOnly = true;
      this.assignedSlotDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.assignedSlotDataGridViewTextBoxColumn.Width = 60;
      this.encryptedDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      this.encryptedDataGridViewTextBoxColumn.DataPropertyName = "Encrypted";
      this.encryptedDataGridViewTextBoxColumn.HeaderText = "Encrypted";
      this.encryptedDataGridViewTextBoxColumn.MinimumWidth = 60;
      this.encryptedDataGridViewTextBoxColumn.Name = "encryptedDataGridViewTextBoxColumn";
      this.encryptedDataGridViewTextBoxColumn.ReadOnly = true;
      this.encryptedDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.encryptedDataGridViewTextBoxColumn.Width = 61;
      this.duplexDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      this.duplexDataGridViewTextBoxColumn.DataPropertyName = "Duplex";
      this.duplexDataGridViewTextBoxColumn.HeaderText = "Duplex";
      this.duplexDataGridViewTextBoxColumn.MinimumWidth = 60;
      this.duplexDataGridViewTextBoxColumn.Name = "duplexDataGridViewTextBoxColumn";
      this.duplexDataGridViewTextBoxColumn.ReadOnly = true;
      this.duplexDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.callsBindingSource.AllowNew = false;
      this.callsBindingSource.DataSource = (object) typeof (CallsDisplay);
      this.callsTextBox.AcceptsReturn = true;
      this.callsTextBox.AcceptsTab = true;
      this.callsTextBox.BorderStyle = BorderStyle.None;
      this.callsTextBox.Dock = DockStyle.Fill;
      this.callsTextBox.Font = new Font("Microsoft Sans Serif", 8.25f);
      this.callsTextBox.Location = new Point(0, 0);
      this.callsTextBox.Multiline = true;
      this.callsTextBox.Name = "callsTextBox";
      this.callsTextBox.ReadOnly = true;
      this.callsTextBox.ScrollBars = ScrollBars.Both;
      this.callsTextBox.Size = new Size(526, 153);
      this.callsTextBox.TabIndex = 23;
      this.callsTextBox.WordWrap = false;
      this.groupTabPage.Controls.Add((Control) this.groupDataGridView);
      this.groupTabPage.Location = new Point(4, 22);
      this.groupTabPage.Name = "groupTabPage";
      this.groupTabPage.Padding = new Padding(3);
      this.groupTabPage.Size = new Size(534, 400);
      this.groupTabPage.TabIndex = 3;
      this.groupTabPage.Text = "Groups";
      this.groupTabPage.UseVisualStyleBackColor = true;
      this.groupDataGridView.AutoGenerateColumns = false;
      this.groupDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.groupDataGridView.Columns.AddRange((DataGridViewColumn) this.gSSIDataGridViewTextBoxColumn1, (DataGridViewColumn) this.priorityDataGridViewTextBoxColumn1, (DataGridViewColumn) this.nameDataGridViewTextBoxColumn1);
      this.groupDataGridView.DataSource = (object) this.groupBindingSource;
      this.groupDataGridView.Dock = DockStyle.Fill;
      this.groupDataGridView.Location = new Point(3, 3);
      this.groupDataGridView.Name = "groupDataGridView";
      this.groupDataGridView.RowHeadersVisible = false;
      this.groupDataGridView.Size = new Size(528, 394);
      this.groupDataGridView.TabIndex = 0;
      this.gSSIDataGridViewTextBoxColumn1.DataPropertyName = "GSSI";
      this.gSSIDataGridViewTextBoxColumn1.HeaderText = "GSSI";
      this.gSSIDataGridViewTextBoxColumn1.Name = "gSSIDataGridViewTextBoxColumn1";
      this.priorityDataGridViewTextBoxColumn1.DataPropertyName = "Priority";
      this.priorityDataGridViewTextBoxColumn1.HeaderText = "Priority";
      this.priorityDataGridViewTextBoxColumn1.Name = "priorityDataGridViewTextBoxColumn1";
      this.priorityDataGridViewTextBoxColumn1.Width = 50;
      this.nameDataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      this.nameDataGridViewTextBoxColumn1.DataPropertyName = "Name";
      this.nameDataGridViewTextBoxColumn1.HeaderText = "Name";
      this.nameDataGridViewTextBoxColumn1.MinimumWidth = 100;
      this.nameDataGridViewTextBoxColumn1.Name = "nameDataGridViewTextBoxColumn1";
      this.groupBindingSource.AllowNew = false;
      this.groupBindingSource.DataSource = (object) typeof (GroupDisplay);
      this.cellTabPage.Controls.Add((Control) this.cellDataGridView);
      this.cellTabPage.Location = new Point(4, 22);
      this.cellTabPage.Name = "cellTabPage";
      this.cellTabPage.Padding = new Padding(3);
      this.cellTabPage.Size = new Size(534, 400);
      this.cellTabPage.TabIndex = 2;
      this.cellTabPage.Text = "Current cell";
      this.cellTabPage.UseVisualStyleBackColor = true;
      this.cellDataGridView.AutoGenerateColumns = false;
      this.cellDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.cellDataGridView.Columns.AddRange((DataGridViewColumn) this.parameterDataGridViewTextBoxColumn, (DataGridViewColumn) this.valueDataGridViewTextBoxColumn, (DataGridViewColumn) this.commentDataGridViewTextBoxColumn);
      this.cellDataGridView.DataSource = (object) this.cellBindingSource;
      this.cellDataGridView.Dock = DockStyle.Fill;
      this.cellDataGridView.Location = new Point(3, 3);
      this.cellDataGridView.Name = "cellDataGridView";
      this.cellDataGridView.RowHeadersVisible = false;
      this.cellDataGridView.Size = new Size(528, 394);
      this.cellDataGridView.TabIndex = 0;
      this.parameterDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      this.parameterDataGridViewTextBoxColumn.DataPropertyName = "Parameter";
      this.parameterDataGridViewTextBoxColumn.HeaderText = "Parameter";
      this.parameterDataGridViewTextBoxColumn.MinimumWidth = 100;
      this.parameterDataGridViewTextBoxColumn.Name = "parameterDataGridViewTextBoxColumn";
      this.valueDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      this.valueDataGridViewTextBoxColumn.DataPropertyName = "Value";
      this.valueDataGridViewTextBoxColumn.HeaderText = "Value";
      this.valueDataGridViewTextBoxColumn.MinimumWidth = 40;
      this.valueDataGridViewTextBoxColumn.Name = "valueDataGridViewTextBoxColumn";
      this.valueDataGridViewTextBoxColumn.Width = 59;
      this.commentDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      this.commentDataGridViewTextBoxColumn.DataPropertyName = "Comment";
      this.commentDataGridViewTextBoxColumn.HeaderText = "Comment";
      this.commentDataGridViewTextBoxColumn.MinimumWidth = 100;
      this.commentDataGridViewTextBoxColumn.Name = "commentDataGridViewTextBoxColumn";
      this.cellBindingSource.AllowNew = false;
      this.cellBindingSource.DataSource = (object) typeof (CellDisplay);
      this.neighbourTabPage.Controls.Add((Control) this.neighbourDataGridView);
      this.neighbourTabPage.Location = new Point(4, 22);
      this.neighbourTabPage.Name = "neighbourTabPage";
      this.neighbourTabPage.Padding = new Padding(3);
      this.neighbourTabPage.Size = new Size(534, 400);
      this.neighbourTabPage.TabIndex = 1;
      this.neighbourTabPage.Text = "Neighbour cell";
      this.neighbourTabPage.UseVisualStyleBackColor = true;
      this.neighbourBindingSource.AllowNew = false;
      this.neighbourBindingSource.DataSource = (object) typeof (NeighbourDisplay);
      this.parameterDataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      this.parameterDataGridViewTextBoxColumn1.DataPropertyName = "Parameter";
      this.parameterDataGridViewTextBoxColumn1.HeaderText = "Parameter";
      this.parameterDataGridViewTextBoxColumn1.Name = "parameterDataGridViewTextBoxColumn1";
      this.parameterDataGridViewTextBoxColumn1.ReadOnly = true;
      this.parameterDataGridViewTextBoxColumn1.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.parameterDataGridViewTextBoxColumn1.Width = 5;
      this.Cell1.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell1.DataPropertyName = "Cell1";
      this.Cell1.HeaderText = "Cell1";
      this.Cell1.Name = "Cell1";
      this.Cell1.ReadOnly = true;
      this.Cell1.Width = 5;
      this.Cell2.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell2.DataPropertyName = "Cell2";
      this.Cell2.HeaderText = "Cell2";
      this.Cell2.Name = "Cell2";
      this.Cell2.ReadOnly = true;
      this.Cell2.Width = 5;
      this.Cell3.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell3.DataPropertyName = "Cell3";
      this.Cell3.HeaderText = "Cell3";
      this.Cell3.Name = "Cell3";
      this.Cell3.ReadOnly = true;
      this.Cell3.Width = 5;
      this.Cell4.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell4.DataPropertyName = "Cell4";
      this.Cell4.HeaderText = "Cell4";
      this.Cell4.Name = "Cell4";
      this.Cell4.ReadOnly = true;
      this.Cell4.Width = 5;
      this.Cell5.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell5.DataPropertyName = "Cell5";
      this.Cell5.HeaderText = "Cell5";
      this.Cell5.Name = "Cell5";
      this.Cell5.ReadOnly = true;
      this.Cell5.Width = 5;
      this.Cell6.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell6.DataPropertyName = "Cell6";
      this.Cell6.HeaderText = "Cell6";
      this.Cell6.Name = "Cell6";
      this.Cell6.ReadOnly = true;
      this.Cell6.Width = 5;
      this.Cell7.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell7.DataPropertyName = "Cell7";
      this.Cell7.HeaderText = "Cell7";
      this.Cell7.Name = "Cell7";
      this.Cell7.ReadOnly = true;
      this.Cell7.Width = 5;
      this.Cell8.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell8.DataPropertyName = "Cell8";
      this.Cell8.HeaderText = "Cell8";
      this.Cell8.Name = "Cell8";
      this.Cell8.ReadOnly = true;
      this.Cell8.Width = 5;
      this.Cell9.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell9.DataPropertyName = "Cell9";
      this.Cell9.HeaderText = "Cell9";
      this.Cell9.Name = "Cell9";
      this.Cell9.ReadOnly = true;
      this.Cell9.Width = 5;
      this.Cell10.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell10.DataPropertyName = "Cell10";
      this.Cell10.HeaderText = "Cell10";
      this.Cell10.Name = "Cell10";
      this.Cell10.ReadOnly = true;
      this.Cell10.Width = 5;
      this.Cell11.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell11.DataPropertyName = "Cell11";
      this.Cell11.HeaderText = "Cell11";
      this.Cell11.Name = "Cell11";
      this.Cell11.ReadOnly = true;
      this.Cell11.Width = 5;
      this.Cell12.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell12.DataPropertyName = "Cell12";
      this.Cell12.HeaderText = "Cell12";
      this.Cell12.Name = "Cell12";
      this.Cell12.ReadOnly = true;
      this.Cell12.Width = 5;
      this.Cell13.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell13.DataPropertyName = "Cell13";
      this.Cell13.HeaderText = "Cell13";
      this.Cell13.Name = "Cell13";
      this.Cell13.ReadOnly = true;
      this.Cell13.Width = 5;
      this.Cell14.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell14.DataPropertyName = "Cell14";
      this.Cell14.HeaderText = "Cell14";
      this.Cell14.Name = "Cell14";
      this.Cell14.ReadOnly = true;
      this.Cell14.Width = 5;
      this.Cell15.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell15.DataPropertyName = "Cell15";
      this.Cell15.HeaderText = "Cell15";
      this.Cell15.Name = "Cell15";
      this.Cell15.ReadOnly = true;
      this.Cell15.Width = 5;
      this.Cell16.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell16.DataPropertyName = "Cell16";
      this.Cell16.HeaderText = "Cell16";
      this.Cell16.Name = "Cell16";
      this.Cell16.ReadOnly = true;
      this.Cell16.Width = 5;
      this.Cell17.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell17.DataPropertyName = "Cell17";
      this.Cell17.HeaderText = "Cell17";
      this.Cell17.Name = "Cell17";
      this.Cell17.ReadOnly = true;
      this.Cell17.Width = 5;
      this.Cell18.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell18.DataPropertyName = "Cell18";
      this.Cell18.HeaderText = "Cell18";
      this.Cell18.Name = "Cell18";
      this.Cell18.ReadOnly = true;
      this.Cell18.Width = 5;
      this.Cell19.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell19.DataPropertyName = "Cell19";
      this.Cell19.HeaderText = "Cell19";
      this.Cell19.Name = "Cell19";
      this.Cell19.ReadOnly = true;
      this.Cell19.Width = 5;
      this.Cell20.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell20.DataPropertyName = "Cell20";
      this.Cell20.HeaderText = "Cell20";
      this.Cell20.Name = "Cell20";
      this.Cell20.ReadOnly = true;
      this.Cell20.Width = 5;
      this.Cell21.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell21.DataPropertyName = "Cell21";
      this.Cell21.HeaderText = "Cell21";
      this.Cell21.Name = "Cell21";
      this.Cell21.ReadOnly = true;
      this.Cell21.Width = 5;
      this.Cell22.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell22.DataPropertyName = "Cell22";
      this.Cell22.HeaderText = "Cell22";
      this.Cell22.Name = "Cell22";
      this.Cell22.ReadOnly = true;
      this.Cell22.Width = 5;
      this.Cell23.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell23.DataPropertyName = "Cell23";
      this.Cell23.HeaderText = "Cell23";
      this.Cell23.Name = "Cell23";
      this.Cell23.ReadOnly = true;
      this.Cell23.Width = 5;
      this.Cell24.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell24.DataPropertyName = "Cell24";
      this.Cell24.HeaderText = "Cell24";
      this.Cell24.Name = "Cell24";
      this.Cell24.ReadOnly = true;
      this.Cell24.Width = 5;
      this.Cell25.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell25.DataPropertyName = "Cell25";
      this.Cell25.HeaderText = "Cell25";
      this.Cell25.Name = "Cell25";
      this.Cell25.ReadOnly = true;
      this.Cell25.Width = 5;
      this.Cell26.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell26.DataPropertyName = "Cell26";
      this.Cell26.HeaderText = "Cell26";
      this.Cell26.Name = "Cell26";
      this.Cell26.ReadOnly = true;
      this.Cell26.Width = 5;
      this.Cell27.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell27.DataPropertyName = "Cell27";
      this.Cell27.HeaderText = "Cell27";
      this.Cell27.Name = "Cell27";
      this.Cell27.ReadOnly = true;
      this.Cell27.Width = 5;
      this.Cell28.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell28.DataPropertyName = "Cell28";
      this.Cell28.HeaderText = "Cell28";
      this.Cell28.Name = "Cell28";
      this.Cell28.ReadOnly = true;
      this.Cell28.Width = 5;
      this.Cell29.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell29.DataPropertyName = "Cell29";
      this.Cell29.HeaderText = "Cell29";
      this.Cell29.Name = "Cell29";
      this.Cell29.ReadOnly = true;
      this.Cell29.Width = 5;
      this.Cell30.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell30.DataPropertyName = "Cell30";
      this.Cell30.HeaderText = "Cell30";
      this.Cell30.Name = "Cell30";
      this.Cell30.ReadOnly = true;
      this.Cell30.Width = 5;
      this.Cell31.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell31.DataPropertyName = "Cell31";
      this.Cell31.HeaderText = "Cell31";
      this.Cell31.Name = "Cell31";
      this.Cell31.ReadOnly = true;
      this.Cell31.Width = 5;
      this.Cell32.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
      this.Cell32.DataPropertyName = "Cell32";
      this.Cell32.HeaderText = "Cell32";
      this.Cell32.Name = "Cell32";
      this.Cell32.ReadOnly = true;
      this.Cell32.Width = 5;
      this.checkBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.checkBox1.AutoSize = true;
      this.checkBox1.Location = new Point(489, 11);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new Size(58, 17);
      this.checkBox1.TabIndex = 27;
      this.checkBox1.Text = "On top";
      this.checkBox1.UseVisualStyleBackColor = true;
      this.checkBox1.CheckedChanged += new EventHandler(this.CheckBox1_CheckedChanged);
      this.cell32DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell32DataGridViewTextBoxColumn.DataPropertyName = "Cell32";
      this.cell32DataGridViewTextBoxColumn.HeaderText = "32";
      this.cell32DataGridViewTextBoxColumn.Name = "cell32DataGridViewTextBoxColumn";
      this.cell32DataGridViewTextBoxColumn.Width = 44;
      this.cell31DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell31DataGridViewTextBoxColumn.DataPropertyName = "Cell31";
      this.cell31DataGridViewTextBoxColumn.HeaderText = "31";
      this.cell31DataGridViewTextBoxColumn.Name = "cell31DataGridViewTextBoxColumn";
      this.cell31DataGridViewTextBoxColumn.Width = 44;
      this.cell30DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell30DataGridViewTextBoxColumn.DataPropertyName = "Cell30";
      this.cell30DataGridViewTextBoxColumn.HeaderText = "30";
      this.cell30DataGridViewTextBoxColumn.Name = "cell30DataGridViewTextBoxColumn";
      this.cell30DataGridViewTextBoxColumn.Width = 44;
      this.cell29DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell29DataGridViewTextBoxColumn.DataPropertyName = "Cell29";
      this.cell29DataGridViewTextBoxColumn.HeaderText = "29";
      this.cell29DataGridViewTextBoxColumn.Name = "cell29DataGridViewTextBoxColumn";
      this.cell29DataGridViewTextBoxColumn.Width = 44;
      this.cell28DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell28DataGridViewTextBoxColumn.DataPropertyName = "Cell28";
      this.cell28DataGridViewTextBoxColumn.HeaderText = "28";
      this.cell28DataGridViewTextBoxColumn.Name = "cell28DataGridViewTextBoxColumn";
      this.cell28DataGridViewTextBoxColumn.Width = 44;
      this.cell27DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell27DataGridViewTextBoxColumn.DataPropertyName = "Cell27";
      this.cell27DataGridViewTextBoxColumn.HeaderText = "27";
      this.cell27DataGridViewTextBoxColumn.Name = "cell27DataGridViewTextBoxColumn";
      this.cell27DataGridViewTextBoxColumn.Width = 44;
      this.cell26DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell26DataGridViewTextBoxColumn.DataPropertyName = "Cell26";
      this.cell26DataGridViewTextBoxColumn.HeaderText = "26";
      this.cell26DataGridViewTextBoxColumn.Name = "cell26DataGridViewTextBoxColumn";
      this.cell26DataGridViewTextBoxColumn.Width = 44;
      this.cell25DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell25DataGridViewTextBoxColumn.DataPropertyName = "Cell25";
      this.cell25DataGridViewTextBoxColumn.HeaderText = "25";
      this.cell25DataGridViewTextBoxColumn.Name = "cell25DataGridViewTextBoxColumn";
      this.cell25DataGridViewTextBoxColumn.Width = 44;
      this.cell24DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell24DataGridViewTextBoxColumn.DataPropertyName = "Cell24";
      this.cell24DataGridViewTextBoxColumn.HeaderText = "24";
      this.cell24DataGridViewTextBoxColumn.Name = "cell24DataGridViewTextBoxColumn";
      this.cell24DataGridViewTextBoxColumn.Width = 44;
      this.cell23DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell23DataGridViewTextBoxColumn.DataPropertyName = "Cell23";
      this.cell23DataGridViewTextBoxColumn.HeaderText = "23";
      this.cell23DataGridViewTextBoxColumn.Name = "cell23DataGridViewTextBoxColumn";
      this.cell23DataGridViewTextBoxColumn.Width = 44;
      this.cell22DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell22DataGridViewTextBoxColumn.DataPropertyName = "Cell22";
      this.cell22DataGridViewTextBoxColumn.HeaderText = "22";
      this.cell22DataGridViewTextBoxColumn.Name = "cell22DataGridViewTextBoxColumn";
      this.cell22DataGridViewTextBoxColumn.Width = 44;
      this.cell21DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell21DataGridViewTextBoxColumn.DataPropertyName = "Cell21";
      this.cell21DataGridViewTextBoxColumn.HeaderText = "21";
      this.cell21DataGridViewTextBoxColumn.Name = "cell21DataGridViewTextBoxColumn";
      this.cell21DataGridViewTextBoxColumn.Width = 44;
      this.cell20DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell20DataGridViewTextBoxColumn.DataPropertyName = "Cell20";
      this.cell20DataGridViewTextBoxColumn.HeaderText = "20";
      this.cell20DataGridViewTextBoxColumn.Name = "cell20DataGridViewTextBoxColumn";
      this.cell20DataGridViewTextBoxColumn.Width = 44;
      this.cell19DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell19DataGridViewTextBoxColumn.DataPropertyName = "Cell19";
      this.cell19DataGridViewTextBoxColumn.HeaderText = "19";
      this.cell19DataGridViewTextBoxColumn.Name = "cell19DataGridViewTextBoxColumn";
      this.cell19DataGridViewTextBoxColumn.Width = 44;
      this.cell18DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell18DataGridViewTextBoxColumn.DataPropertyName = "Cell18";
      this.cell18DataGridViewTextBoxColumn.HeaderText = "18";
      this.cell18DataGridViewTextBoxColumn.Name = "cell18DataGridViewTextBoxColumn";
      this.cell18DataGridViewTextBoxColumn.Width = 44;
      this.cell17DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell17DataGridViewTextBoxColumn.DataPropertyName = "Cell17";
      this.cell17DataGridViewTextBoxColumn.HeaderText = "17";
      this.cell17DataGridViewTextBoxColumn.Name = "cell17DataGridViewTextBoxColumn";
      this.cell17DataGridViewTextBoxColumn.Width = 44;
      this.cell16DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell16DataGridViewTextBoxColumn.DataPropertyName = "Cell16";
      this.cell16DataGridViewTextBoxColumn.HeaderText = "16";
      this.cell16DataGridViewTextBoxColumn.Name = "cell16DataGridViewTextBoxColumn";
      this.cell16DataGridViewTextBoxColumn.Width = 44;
      this.cell15DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell15DataGridViewTextBoxColumn.DataPropertyName = "Cell15";
      this.cell15DataGridViewTextBoxColumn.HeaderText = "15";
      this.cell15DataGridViewTextBoxColumn.Name = "cell15DataGridViewTextBoxColumn";
      this.cell15DataGridViewTextBoxColumn.Width = 44;
      this.cell14DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell14DataGridViewTextBoxColumn.DataPropertyName = "Cell14";
      this.cell14DataGridViewTextBoxColumn.HeaderText = "14";
      this.cell14DataGridViewTextBoxColumn.Name = "cell14DataGridViewTextBoxColumn";
      this.cell14DataGridViewTextBoxColumn.Width = 44;
      this.cell13DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell13DataGridViewTextBoxColumn.DataPropertyName = "Cell13";
      this.cell13DataGridViewTextBoxColumn.HeaderText = "13";
      this.cell13DataGridViewTextBoxColumn.Name = "cell13DataGridViewTextBoxColumn";
      this.cell13DataGridViewTextBoxColumn.Width = 44;
      this.cell12DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell12DataGridViewTextBoxColumn.DataPropertyName = "Cell12";
      this.cell12DataGridViewTextBoxColumn.HeaderText = "12";
      this.cell12DataGridViewTextBoxColumn.Name = "cell12DataGridViewTextBoxColumn";
      this.cell12DataGridViewTextBoxColumn.Width = 44;
      this.cell11DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell11DataGridViewTextBoxColumn.DataPropertyName = "Cell11";
      this.cell11DataGridViewTextBoxColumn.HeaderText = "11";
      this.cell11DataGridViewTextBoxColumn.Name = "cell11DataGridViewTextBoxColumn";
      this.cell11DataGridViewTextBoxColumn.Width = 44;
      this.cell10DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell10DataGridViewTextBoxColumn.DataPropertyName = "Cell10";
      this.cell10DataGridViewTextBoxColumn.HeaderText = "10";
      this.cell10DataGridViewTextBoxColumn.Name = "cell10DataGridViewTextBoxColumn";
      this.cell10DataGridViewTextBoxColumn.Width = 44;
      this.cell9DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell9DataGridViewTextBoxColumn.DataPropertyName = "Cell9";
      this.cell9DataGridViewTextBoxColumn.HeaderText = "9";
      this.cell9DataGridViewTextBoxColumn.Name = "cell9DataGridViewTextBoxColumn";
      this.cell9DataGridViewTextBoxColumn.Width = 38;
      this.cell8DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell8DataGridViewTextBoxColumn.DataPropertyName = "Cell8";
      this.cell8DataGridViewTextBoxColumn.HeaderText = "8";
      this.cell8DataGridViewTextBoxColumn.Name = "cell8DataGridViewTextBoxColumn";
      this.cell8DataGridViewTextBoxColumn.Width = 38;
      this.cell7DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell7DataGridViewTextBoxColumn.DataPropertyName = "Cell7";
      this.cell7DataGridViewTextBoxColumn.HeaderText = "7";
      this.cell7DataGridViewTextBoxColumn.Name = "cell7DataGridViewTextBoxColumn";
      this.cell7DataGridViewTextBoxColumn.Width = 38;
      this.cell6DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell6DataGridViewTextBoxColumn.DataPropertyName = "Cell6";
      this.cell6DataGridViewTextBoxColumn.HeaderText = "6";
      this.cell6DataGridViewTextBoxColumn.Name = "cell6DataGridViewTextBoxColumn";
      this.cell6DataGridViewTextBoxColumn.Width = 38;
      this.cell5DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell5DataGridViewTextBoxColumn.DataPropertyName = "Cell5";
      this.cell5DataGridViewTextBoxColumn.HeaderText = "5";
      this.cell5DataGridViewTextBoxColumn.Name = "cell5DataGridViewTextBoxColumn";
      this.cell5DataGridViewTextBoxColumn.Width = 38;
      this.cell4DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell4DataGridViewTextBoxColumn.DataPropertyName = "Cell4";
      this.cell4DataGridViewTextBoxColumn.HeaderText = "4";
      this.cell4DataGridViewTextBoxColumn.Name = "cell4DataGridViewTextBoxColumn";
      this.cell4DataGridViewTextBoxColumn.Width = 38;
      this.cell3DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell3DataGridViewTextBoxColumn.DataPropertyName = "Cell3";
      this.cell3DataGridViewTextBoxColumn.HeaderText = "3";
      this.cell3DataGridViewTextBoxColumn.Name = "cell3DataGridViewTextBoxColumn";
      this.cell3DataGridViewTextBoxColumn.Width = 38;
      this.cell2DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell2DataGridViewTextBoxColumn.DataPropertyName = "Cell2";
      this.cell2DataGridViewTextBoxColumn.HeaderText = "2";
      this.cell2DataGridViewTextBoxColumn.Name = "cell2DataGridViewTextBoxColumn";
      this.cell2DataGridViewTextBoxColumn.Width = 38;
      this.cell1DataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.cell1DataGridViewTextBoxColumn.DataPropertyName = "Cell1";
      this.cell1DataGridViewTextBoxColumn.HeaderText = "1";
      this.cell1DataGridViewTextBoxColumn.Name = "cell1DataGridViewTextBoxColumn";
      this.cell1DataGridViewTextBoxColumn.Width = 38;
      this.parameterDataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.parameterDataGridViewTextBoxColumn2.DataPropertyName = "Parameter";
      this.parameterDataGridViewTextBoxColumn2.HeaderText = "";
      this.parameterDataGridViewTextBoxColumn2.Name = "parameterDataGridViewTextBoxColumn2";
      this.parameterDataGridViewTextBoxColumn2.Width = 19;
      this.neighbourDataGridView.AllowDrop = true;
      this.neighbourDataGridView.AutoGenerateColumns = false;
      this.neighbourDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.neighbourDataGridView.Columns.AddRange((DataGridViewColumn) this.parameterDataGridViewTextBoxColumn2, (DataGridViewColumn) this.cell1DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell2DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell3DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell4DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell5DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell6DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell7DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell8DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell9DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell10DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell11DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell12DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell13DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell14DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell15DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell16DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell17DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell18DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell19DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell20DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell21DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell22DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell23DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell24DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell25DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell26DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell27DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell28DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell29DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell30DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell31DataGridViewTextBoxColumn, (DataGridViewColumn) this.cell32DataGridViewTextBoxColumn);
      this.neighbourDataGridView.DataSource = (object) this.neighbourBindingSource;
      this.neighbourDataGridView.Dock = DockStyle.Fill;
      this.neighbourDataGridView.Location = new Point(3, 3);
      this.neighbourDataGridView.Name = "neighbourDataGridView";
      this.neighbourDataGridView.RowHeadersVisible = false;
      this.neighbourDataGridView.Size = new Size(528, 394);
      this.neighbourDataGridView.TabIndex = 0;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(566, 450);
      this.Controls.Add((Control) this.checkBox1);
      this.Controls.Add((Control) this.tabControl1);
      this.DoubleBuffered = true;
      this.MaximizeBox = false;
      this.Name = nameof (NetInfoWindow);
      this.Text = "Network Info";
      this.tabControl1.ResumeLayout(false);
      this.callsTabPage.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      this.splitContainer2.Panel2.PerformLayout();
      this.splitContainer2.EndInit();
      this.splitContainer2.ResumeLayout(false);
      ((ISupportInitialize) this.callsDataGridView).EndInit();
      ((ISupportInitialize) this.callsBindingSource).EndInit();
      this.groupTabPage.ResumeLayout(false);
      ((ISupportInitialize) this.groupDataGridView).EndInit();
      ((ISupportInitialize) this.groupBindingSource).EndInit();
      this.cellTabPage.ResumeLayout(false);
      ((ISupportInitialize) this.cellDataGridView).EndInit();
      ((ISupportInitialize) this.cellBindingSource).EndInit();
      this.neighbourTabPage.ResumeLayout(false);
      ((ISupportInitialize) this.neighbourBindingSource).EndInit();
      ((ISupportInitialize) this.neighbourDataGridView).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
