// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.DialogConfigure
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SDRSharp.Tetra
{
    public class DialogConfigure : Form
    {
        private TetraSettings _tetraSettings;
        private TetraPanel _tetra;
        private IContainer components;
        private Button btnOk;
        private TabControl tabControl1;
        private TabPage logPage;
        private Button logFolderButton;
        private Label label5;
        private Label label6;
        private TextBox logFileRulesTextBox;
        private Label label7;
        private Label label8;
        private TextBox logEntriesRulesTextBox;
        private Label label11;
        private TextBox logSeparatorTextBox;
        private FolderBrowserDialog logFolderBrowserDialog;
        private CheckBox logEnableCheckBox;
        private Label LogEntryLabel;
        private Label LogFileLabel;
        private Label label1;
        private TabPage tabPage1;
        private NumericUpDown udpPortNumericUpDown;
        private CheckBox enableUdpOutputCheckBox;
        private CheckBox ignoreEncodedSpeechCheckBox;
        private CheckBox afcCheckBox;

        public DialogConfigure(TetraSettings tetraSettings, TetraPanel tetra)
        {
            this._tetraSettings = tetraSettings;
            this._tetra = tetra;
            this.InitializeComponent();
            this.ignoreEncodedSpeechCheckBox.Checked = this._tetraSettings.IgnoreEncodedSpeech;
            this.enableUdpOutputCheckBox.Checked = this._tetraSettings.UdpEnabled;
            this.udpPortNumericUpDown.Value = (Decimal)this._tetraSettings.UdpPort;
            this.afcCheckBox.Checked = this._tetraSettings.AfcDisabled;
            this.logFolderBrowserDialog.SelectedPath = this._tetraSettings.LogWriteFolder;
            this.logFileRulesTextBox.Text = this._tetraSettings.LogFileNameRules;
            this.LogFileRulesTextBox_TextChanged((object)null, (EventArgs)null);
            this.logEntriesRulesTextBox.Text = this._tetraSettings.LogEntryRules;
            this.LogEntriesRulesTextBox_TextChanged((object)null, (EventArgs)null);
            this.logSeparatorTextBox.Text = this._tetraSettings.LogSeparator;
            this.logEnableCheckBox.Checked = this._tetraSettings.LogEnabled;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this._tetraSettings.LogFileNameRules = this.logFileRulesTextBox.Text;
            this._tetraSettings.LogEntryRules = this.logEntriesRulesTextBox.Text;
            this._tetraSettings.LogSeparator = this.logSeparatorTextBox.Text;
            this._tetraSettings.LogEnabled = this.logEnableCheckBox.Checked;
            this._tetraSettings.IgnoreEncodedSpeech = this.ignoreEncodedSpeechCheckBox.Checked;
            this._tetraSettings.UdpEnabled = this.enableUdpOutputCheckBox.Checked;
            this._tetraSettings.UdpPort = (int)this.udpPortNumericUpDown.Value;
            this._tetraSettings.AfcDisabled = this.afcCheckBox.Checked;
            this.DialogResult = DialogResult.OK;
        }

        private void LogFolderButton_Click(object sender, EventArgs e)
        {
            if (this.logFolderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;
            this._tetraSettings.LogWriteFolder = this.logFolderBrowserDialog.SelectedPath;
        }

        private void LogFileRulesTextBox_TextChanged(object sender, EventArgs e) => this.LogFileLabel.Text = this._tetra.ParseStringToPath(this.logFileRulesTextBox.Text, ".csv");

        private void LogEntriesRulesTextBox_TextChanged(object sender, EventArgs e) => this.LogEntryLabel.Text = this._tetra.ParseStringToEntries(this.logEntriesRulesTextBox.Text, (CallsEntry)null);

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnOk = new Button();
            this.tabControl1 = new TabControl();
            this.tabPage1 = new TabPage();
            this.afcCheckBox = new CheckBox();
            this.udpPortNumericUpDown = new NumericUpDown();
            this.enableUdpOutputCheckBox = new CheckBox();
            this.ignoreEncodedSpeechCheckBox = new CheckBox();
            this.logPage = new TabPage();
            this.label1 = new Label();
            this.LogEntryLabel = new Label();
            this.LogFileLabel = new Label();
            this.logEnableCheckBox = new CheckBox();
            this.label11 = new Label();
            this.logSeparatorTextBox = new TextBox();
            this.label7 = new Label();
            this.label8 = new Label();
            this.logEntriesRulesTextBox = new TextBox();
            this.logFolderButton = new Button();
            this.label5 = new Label();
            this.label6 = new Label();
            this.logFileRulesTextBox = new TextBox();
            this.logFolderBrowserDialog = new FolderBrowserDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.udpPortNumericUpDown.BeginInit();
            this.logPage.SuspendLayout();
            this.SuspendLayout();
            this.btnOk.DialogResult = DialogResult.OK;
            this.btnOk.Location = new Point(434, 301);
            this.btnOk.Margin = new Padding(2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new Size(56, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "O&K";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new EventHandler(this.BtnOk_Click);
            this.tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.tabControl1.Controls.Add((Control)this.tabPage1);
            this.tabControl1.Controls.Add((Control)this.logPage);
            this.tabControl1.Location = new Point(12, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new Size(478, 293);
            this.tabControl1.TabIndex = 26;
            this.tabPage1.BackColor = SystemColors.Menu;
            this.tabPage1.Controls.Add((Control)this.afcCheckBox);
            this.tabPage1.Controls.Add((Control)this.udpPortNumericUpDown);
            this.tabPage1.Controls.Add((Control)this.enableUdpOutputCheckBox);
            this.tabPage1.Controls.Add((Control)this.ignoreEncodedSpeechCheckBox);
            this.tabPage1.Location = new Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new Padding(3);
            this.tabPage1.Size = new Size(470, 267);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Settings";
            this.afcCheckBox.AutoSize = true;
            this.afcCheckBox.Location = new Point(6, 64);
            this.afcCheckBox.Name = "afcCheckBox";
            this.afcCheckBox.Size = new Size(224, 17);
            this.afcCheckBox.TabIndex = 5;
            this.afcCheckBox.Text = "Disable AFC (automatic frequency control)";
            this.afcCheckBox.UseVisualStyleBackColor = true;
            this.udpPortNumericUpDown.Location = new Point(150, 40);
            this.udpPortNumericUpDown.Maximum = new Decimal(new int[4]
            {
        (int) ushort.MaxValue,
        0,
        0,
        0
            });
            this.udpPortNumericUpDown.Name = "udpPortNumericUpDown";
            this.udpPortNumericUpDown.Size = new Size(63, 20);
            this.udpPortNumericUpDown.TabIndex = 4;
            this.udpPortNumericUpDown.Value = new Decimal(new int[4]
            {
        20025,
        0,
        0,
        0
            });
            this.enableUdpOutputCheckBox.AutoSize = true;
            this.enableUdpOutputCheckBox.Location = new Point(6, 41);
            this.enableUdpOutputCheckBox.Name = "enableUdpOutputCheckBox";
            this.enableUdpOutputCheckBox.Size = new Size(138, 17);
            this.enableUdpOutputCheckBox.TabIndex = 3;
            this.enableUdpOutputCheckBox.Text = "Enable udp output. Port";
            this.enableUdpOutputCheckBox.UseVisualStyleBackColor = true;
            this.ignoreEncodedSpeechCheckBox.AutoSize = true;
            this.ignoreEncodedSpeechCheckBox.Location = new Point(6, 18);
            this.ignoreEncodedSpeechCheckBox.Name = "ignoreEncodedSpeechCheckBox";
            this.ignoreEncodedSpeechCheckBox.Size = new Size(140, 17);
            this.ignoreEncodedSpeechCheckBox.TabIndex = 1;
            this.ignoreEncodedSpeechCheckBox.Text = "Listen only clear speech";
            this.ignoreEncodedSpeechCheckBox.UseVisualStyleBackColor = true;
            this.logPage.BackColor = SystemColors.Menu;
            this.logPage.Controls.Add((Control)this.label1);
            this.logPage.Controls.Add((Control)this.LogEntryLabel);
            this.logPage.Controls.Add((Control)this.LogFileLabel);
            this.logPage.Controls.Add((Control)this.logEnableCheckBox);
            this.logPage.Controls.Add((Control)this.label11);
            this.logPage.Controls.Add((Control)this.logSeparatorTextBox);
            this.logPage.Controls.Add((Control)this.label7);
            this.logPage.Controls.Add((Control)this.label8);
            this.logPage.Controls.Add((Control)this.logEntriesRulesTextBox);
            this.logPage.Controls.Add((Control)this.logFolderButton);
            this.logPage.Controls.Add((Control)this.label5);
            this.logPage.Controls.Add((Control)this.label6);
            this.logPage.Controls.Add((Control)this.logFileRulesTextBox);
            this.logPage.Location = new Point(4, 22);
            this.logPage.Name = "logPage";
            this.logPage.Size = new Size(470, 267);
            this.logPage.TabIndex = 2;
            this.logPage.Text = "Log";
            this.label1.AutoSize = true;
            this.label1.Location = new Point(74, 171);
            this.label1.Name = "label1";
            this.label1.Size = new Size(118, 13);
            this.label1.TabIndex = 64;
            this.label1.Text = "slot, callid, \"any text\", +";
            this.LogEntryLabel.AutoSize = true;
            this.LogEntryLabel.Location = new Point(12, 224);
            this.LogEntryLabel.Name = "LogEntryLabel";
            this.LogEntryLabel.Size = new Size(13, 13);
            this.LogEntryLabel.TabIndex = 63;
            this.LogEntryLabel.Text = "_";
            this.LogFileLabel.AutoSize = true;
            this.LogFileLabel.Location = new Point(11, 102);
            this.LogFileLabel.Name = "LogFileLabel";
            this.LogFileLabel.Size = new Size(13, 13);
            this.LogFileLabel.TabIndex = 62;
            this.LogFileLabel.Text = "_";
            this.logEnableCheckBox.AutoSize = true;
            this.logEnableCheckBox.Location = new Point(8, 10);
            this.logEnableCheckBox.Name = "logEnableCheckBox";
            this.logEnableCheckBox.Size = new Size(59, 17);
            this.logEnableCheckBox.TabIndex = 61;
            this.logEnableCheckBox.Text = "Enable";
            this.logEnableCheckBox.UseVisualStyleBackColor = true;
            this.label11.AutoSize = true;
            this.label11.Location = new Point(333, 130);
            this.label11.Name = "label11";
            this.label11.Size = new Size(97, 13);
            this.label11.TabIndex = 59;
            this.label11.Text = "Elements separator";
            this.logSeparatorTextBox.Location = new Point(438, (int)sbyte.MaxValue);
            this.logSeparatorTextBox.Name = "logSeparatorTextBox";
            this.logSeparatorTextBox.Size = new Size(24, 20);
            this.logSeparatorTextBox.TabIndex = 58;
            this.label7.AutoSize = true;
            this.label7.Location = new Point(5, 151);
            this.label7.Name = "label7";
            this.label7.Size = new Size(400, 13);
            this.label7.TabIndex = 54;
            this.label7.Text = "You can use: date, time, carrier, mcc, mnc, la, cc, type, from, to, encryption, duplex,";
            this.label8.AutoSize = true;
            this.label8.Location = new Point(5, 132);
            this.label8.Name = "label8";
            this.label8.Size = new Size(144, 13);
            this.label8.TabIndex = 53;
            this.label8.Text = "Rules for creating log entries.";
            this.logEntriesRulesTextBox.Location = new Point(8, 196);
            this.logEntriesRulesTextBox.Name = "logEntriesRulesTextBox";
            this.logEntriesRulesTextBox.Size = new Size(458, 20);
            this.logEntriesRulesTextBox.TabIndex = 52;
            this.logEntriesRulesTextBox.TextChanged += new EventHandler(this.LogEntriesRulesTextBox_TextChanged);
            this.logFolderButton.Location = new Point(387, 43);
            this.logFolderButton.Name = "logFolderButton";
            this.logFolderButton.Size = new Size(75, 23);
            this.logFolderButton.TabIndex = 51;
            this.logFolderButton.Text = "Folder";
            this.logFolderButton.UseVisualStyleBackColor = true;
            this.logFolderButton.Click += new EventHandler(this.LogFolderButton_Click);
            this.label5.AutoSize = true;
            this.label5.Location = new Point(5, 53);
            this.label5.Name = "label5";
            this.label5.Size = new Size(358, 13);
            this.label5.TabIndex = 50;
            this.label5.Text = "You can use: date, time, frequency, mcc, mnc, la, cc, to, \"any text\", +, \\, /";
            this.label6.AutoSize = true;
            this.label6.Location = new Point(5, 34);
            this.label6.Name = "label6";
            this.label6.Size = new Size(160, 13);
            this.label6.TabIndex = 49;
            this.label6.Text = "Rules for creating log file names.";
            this.logFileRulesTextBox.Location = new Point(8, 72);
            this.logFileRulesTextBox.Name = "logFileRulesTextBox";
            this.logFileRulesTextBox.Size = new Size(458, 20);
            this.logFileRulesTextBox.TabIndex = 48;
            this.logFileRulesTextBox.TextChanged += new EventHandler(this.LogFileRulesTextBox_TextChanged);
            this.AcceptButton = (IButtonControl)this.btnOk;
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(501, 335);
            this.Controls.Add((Control)this.tabControl1);
            this.Controls.Add((Control)this.btnOk);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Margin = new Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogConfigure";
            this.SizeGripStyle = SizeGripStyle.Hide;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Configure";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.udpPortNumericUpDown.EndInit();
            this.logPage.ResumeLayout(false);
            this.logPage.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
