namespace SDRSharp.Tetra
{
    partial class DialogConfigure
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnOk = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.logPage = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.LogEntryLabel = new System.Windows.Forms.Label();
            this.LogFileLabel = new System.Windows.Forms.Label();
            this.logEnableCheckBox = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.logSeparatorTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.logEntriesRulesTextBox = new System.Windows.Forms.TextBox();
            this.logFolderButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.logFileRulesTextBox = new System.Windows.Forms.TextBox();
            this.recorderTabPage = new System.Windows.Forms.TabPage();
            this.folderButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.resultLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.folderTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.NewFileTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.newFileTimeEnableCheckBox = new System.Windows.Forms.CheckBox();
            this.continueRecordTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.dontWritePauseCheckBox = new System.Windows.Forms.CheckBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.udpPortNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.enableUdpOutputCheckBox = new System.Windows.Forms.CheckBox();
            this.showGroupNameCheckBox = new System.Windows.Forms.CheckBox();
            this.ignoreEncodedSpeechCheckBox = new System.Windows.Forms.CheckBox();
            this.showEncodedCheckBox = new System.Windows.Forms.CheckBox();
            this.displayTimer = new System.Windows.Forms.Timer(this.components);
            this.writeFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.logFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.afcCheckBox = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.logPage.SuspendLayout();
            this.recorderTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NewFileTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.continueRecordTimeNumericUpDown)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udpPortNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(434, 301);
            this.btnOk.Margin = new System.Windows.Forms.Padding(2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(56, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "O&K";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.logPage);
            this.tabControl1.Controls.Add(this.recorderTabPage);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(478, 293);
            this.tabControl1.TabIndex = 26;
            // 
            // logPage
            // 
            this.logPage.BackColor = System.Drawing.SystemColors.Menu;
            this.logPage.Controls.Add(this.label1);
            this.logPage.Controls.Add(this.LogEntryLabel);
            this.logPage.Controls.Add(this.LogFileLabel);
            this.logPage.Controls.Add(this.logEnableCheckBox);
            this.logPage.Controls.Add(this.label11);
            this.logPage.Controls.Add(this.logSeparatorTextBox);
            this.logPage.Controls.Add(this.label7);
            this.logPage.Controls.Add(this.label8);
            this.logPage.Controls.Add(this.logEntriesRulesTextBox);
            this.logPage.Controls.Add(this.logFolderButton);
            this.logPage.Controls.Add(this.label5);
            this.logPage.Controls.Add(this.label6);
            this.logPage.Controls.Add(this.logFileRulesTextBox);
            this.logPage.Location = new System.Drawing.Point(4, 22);
            this.logPage.Name = "logPage";
            this.logPage.Size = new System.Drawing.Size(470, 267);
            this.logPage.TabIndex = 2;
            this.logPage.Text = "Log";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(74, 171);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 64;
            this.label1.Text = "slot, callid, \"any text\", +";
            // 
            // LogEntryLabel
            // 
            this.LogEntryLabel.AutoSize = true;
            this.LogEntryLabel.Location = new System.Drawing.Point(12, 224);
            this.LogEntryLabel.Name = "LogEntryLabel";
            this.LogEntryLabel.Size = new System.Drawing.Size(13, 13);
            this.LogEntryLabel.TabIndex = 63;
            this.LogEntryLabel.Text = "_";
            // 
            // LogFileLabel
            // 
            this.LogFileLabel.AutoSize = true;
            this.LogFileLabel.Location = new System.Drawing.Point(11, 102);
            this.LogFileLabel.Name = "LogFileLabel";
            this.LogFileLabel.Size = new System.Drawing.Size(13, 13);
            this.LogFileLabel.TabIndex = 62;
            this.LogFileLabel.Text = "_";
            // 
            // logEnableCheckBox
            // 
            this.logEnableCheckBox.AutoSize = true;
            this.logEnableCheckBox.Location = new System.Drawing.Point(8, 10);
            this.logEnableCheckBox.Name = "logEnableCheckBox";
            this.logEnableCheckBox.Size = new System.Drawing.Size(59, 17);
            this.logEnableCheckBox.TabIndex = 61;
            this.logEnableCheckBox.Text = "Enable";
            this.logEnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(333, 130);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(97, 13);
            this.label11.TabIndex = 59;
            this.label11.Text = "Elements separator";
            // 
            // logSeparatorTextBox
            // 
            this.logSeparatorTextBox.Location = new System.Drawing.Point(438, 127);
            this.logSeparatorTextBox.Name = "logSeparatorTextBox";
            this.logSeparatorTextBox.Size = new System.Drawing.Size(24, 20);
            this.logSeparatorTextBox.TabIndex = 58;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 151);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(380, 13);
            this.label7.TabIndex = 54;
            this.label7.Text = "You can use: date, time, carrier, mcc, mnc, la, cc, group, tx, encryption, duplex" +
    ",";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 132);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(144, 13);
            this.label8.TabIndex = 53;
            this.label8.Text = "Rules for creating log entries.";
            // 
            // logEntriesRulesTextBox
            // 
            this.logEntriesRulesTextBox.Location = new System.Drawing.Point(8, 196);
            this.logEntriesRulesTextBox.Name = "logEntriesRulesTextBox";
            this.logEntriesRulesTextBox.Size = new System.Drawing.Size(458, 20);
            this.logEntriesRulesTextBox.TabIndex = 52;
            this.logEntriesRulesTextBox.TextChanged += new System.EventHandler(this.logEntriesRulesTextBox_TextChanged);
            // 
            // logFolderButton
            // 
            this.logFolderButton.Location = new System.Drawing.Point(387, 43);
            this.logFolderButton.Name = "logFolderButton";
            this.logFolderButton.Size = new System.Drawing.Size(75, 23);
            this.logFolderButton.TabIndex = 51;
            this.logFolderButton.Text = "Folder";
            this.logFolderButton.UseVisualStyleBackColor = true;
            this.logFolderButton.Click += new System.EventHandler(this.logFolderButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(376, 13);
            this.label5.TabIndex = 50;
            this.label5.Text = "You can use: date, time, frequency, mcc, mnc, la, cc, group, \"any text\", +, \\, /";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(160, 13);
            this.label6.TabIndex = 49;
            this.label6.Text = "Rules for creating log file names.";
            // 
            // logFileRulesTextBox
            // 
            this.logFileRulesTextBox.Location = new System.Drawing.Point(8, 72);
            this.logFileRulesTextBox.Name = "logFileRulesTextBox";
            this.logFileRulesTextBox.Size = new System.Drawing.Size(458, 20);
            this.logFileRulesTextBox.TabIndex = 48;
            this.logFileRulesTextBox.TextChanged += new System.EventHandler(this.logFileRulesTextBox_TextChanged);
            // 
            // recorderTabPage
            // 
            this.recorderTabPage.BackColor = System.Drawing.SystemColors.Menu;
            this.recorderTabPage.Controls.Add(this.folderButton);
            this.recorderTabPage.Controls.Add(this.label2);
            this.recorderTabPage.Controls.Add(this.resultLabel);
            this.recorderTabPage.Controls.Add(this.label4);
            this.recorderTabPage.Controls.Add(this.folderTextBox);
            this.recorderTabPage.Controls.Add(this.label3);
            this.recorderTabPage.Controls.Add(this.NewFileTimeNumericUpDown);
            this.recorderTabPage.Controls.Add(this.newFileTimeEnableCheckBox);
            this.recorderTabPage.Controls.Add(this.continueRecordTimeNumericUpDown);
            this.recorderTabPage.Controls.Add(this.dontWritePauseCheckBox);
            this.recorderTabPage.Location = new System.Drawing.Point(4, 22);
            this.recorderTabPage.Name = "recorderTabPage";
            this.recorderTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.recorderTabPage.Size = new System.Drawing.Size(470, 267);
            this.recorderTabPage.TabIndex = 1;
            this.recorderTabPage.Text = "Recorder options";
            // 
            // folderButton
            // 
            this.folderButton.Location = new System.Drawing.Point(389, 147);
            this.folderButton.Name = "folderButton";
            this.folderButton.Size = new System.Drawing.Size(75, 23);
            this.folderButton.TabIndex = 56;
            this.folderButton.Text = "Folder";
            this.folderButton.UseVisualStyleBackColor = true;
            this.folderButton.Click += new System.EventHandler(this.folderButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 157);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(310, 13);
            this.label2.TabIndex = 55;
            this.label2.Text = "You can use: date, time, frequency, group, ssi, \"any text\", +, \\, /";
            // 
            // resultLabel
            // 
            this.resultLabel.AutoSize = true;
            this.resultLabel.Location = new System.Drawing.Point(6, 206);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(13, 13);
            this.resultLabel.TabIndex = 54;
            this.resultLabel.Text = "_";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(143, 13);
            this.label4.TabIndex = 53;
            this.label4.Text = "Rules for creating file names.";
            // 
            // folderTextBox
            // 
            this.folderTextBox.Location = new System.Drawing.Point(9, 176);
            this.folderTextBox.Name = "folderTextBox";
            this.folderTextBox.Size = new System.Drawing.Size(458, 20);
            this.folderTextBox.TabIndex = 52;
            this.folderTextBox.Text = "/ date / name + \"_\" + frequency / time + \"_\" + name + \"_\" + frequency";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(300, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "Continue recording after the squelch has been closed, second";
            // 
            // NewFileTimeNumericUpDown
            // 
            this.NewFileTimeNumericUpDown.Location = new System.Drawing.Point(280, 5);
            this.NewFileTimeNumericUpDown.Name = "NewFileTimeNumericUpDown";
            this.NewFileTimeNumericUpDown.Size = new System.Drawing.Size(63, 20);
            this.NewFileTimeNumericUpDown.TabIndex = 27;
            // 
            // newFileTimeEnableCheckBox
            // 
            this.newFileTimeEnableCheckBox.AutoSize = true;
            this.newFileTimeEnableCheckBox.Location = new System.Drawing.Point(6, 6);
            this.newFileTimeEnableCheckBox.Name = "newFileTimeEnableCheckBox";
            this.newFileTimeEnableCheckBox.Size = new System.Drawing.Size(268, 17);
            this.newFileTimeEnableCheckBox.TabIndex = 23;
            this.newFileTimeEnableCheckBox.Text = "New file after the squelch has been closed, second";
            this.newFileTimeEnableCheckBox.UseVisualStyleBackColor = true;
            // 
            // continueRecordTimeNumericUpDown
            // 
            this.continueRecordTimeNumericUpDown.Location = new System.Drawing.Point(330, 51);
            this.continueRecordTimeNumericUpDown.Name = "continueRecordTimeNumericUpDown";
            this.continueRecordTimeNumericUpDown.Size = new System.Drawing.Size(57, 20);
            this.continueRecordTimeNumericUpDown.TabIndex = 22;
            this.continueRecordTimeNumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // dontWritePauseCheckBox
            // 
            this.dontWritePauseCheckBox.AutoSize = true;
            this.dontWritePauseCheckBox.Location = new System.Drawing.Point(6, 29);
            this.dontWritePauseCheckBox.Name = "dontWritePauseCheckBox";
            this.dontWritePauseCheckBox.Size = new System.Drawing.Size(108, 17);
            this.dontWritePauseCheckBox.TabIndex = 20;
            this.dontWritePauseCheckBox.Text = "Don\'t write pause";
            this.dontWritePauseCheckBox.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Menu;
            this.tabPage1.Controls.Add(this.afcCheckBox);
            this.tabPage1.Controls.Add(this.udpPortNumericUpDown);
            this.tabPage1.Controls.Add(this.enableUdpOutputCheckBox);
            this.tabPage1.Controls.Add(this.showGroupNameCheckBox);
            this.tabPage1.Controls.Add(this.ignoreEncodedSpeechCheckBox);
            this.tabPage1.Controls.Add(this.showEncodedCheckBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(470, 267);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Settings";
            // 
            // udpPortNumericUpDown
            // 
            this.udpPortNumericUpDown.Location = new System.Drawing.Point(150, 74);
            this.udpPortNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.udpPortNumericUpDown.Name = "udpPortNumericUpDown";
            this.udpPortNumericUpDown.Size = new System.Drawing.Size(63, 20);
            this.udpPortNumericUpDown.TabIndex = 4;
            this.udpPortNumericUpDown.Value = new decimal(new int[] {
            20025,
            0,
            0,
            0});
            // 
            // enableUdpOutputCheckBox
            // 
            this.enableUdpOutputCheckBox.AutoSize = true;
            this.enableUdpOutputCheckBox.Location = new System.Drawing.Point(6, 75);
            this.enableUdpOutputCheckBox.Name = "enableUdpOutputCheckBox";
            this.enableUdpOutputCheckBox.Size = new System.Drawing.Size(138, 17);
            this.enableUdpOutputCheckBox.TabIndex = 3;
            this.enableUdpOutputCheckBox.Text = "Enable udp output. Port";
            this.enableUdpOutputCheckBox.UseVisualStyleBackColor = true;
            // 
            // showGroupNameCheckBox
            // 
            this.showGroupNameCheckBox.AutoSize = true;
            this.showGroupNameCheckBox.Location = new System.Drawing.Point(6, 52);
            this.showGroupNameCheckBox.Name = "showGroupNameCheckBox";
            this.showGroupNameCheckBox.Size = new System.Drawing.Size(112, 17);
            this.showGroupNameCheckBox.TabIndex = 2;
            this.showGroupNameCheckBox.Text = "Show group name";
            this.showGroupNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // ignoreEncodedSpeechCheckBox
            // 
            this.ignoreEncodedSpeechCheckBox.AutoSize = true;
            this.ignoreEncodedSpeechCheckBox.Location = new System.Drawing.Point(6, 29);
            this.ignoreEncodedSpeechCheckBox.Name = "ignoreEncodedSpeechCheckBox";
            this.ignoreEncodedSpeechCheckBox.Size = new System.Drawing.Size(140, 17);
            this.ignoreEncodedSpeechCheckBox.TabIndex = 1;
            this.ignoreEncodedSpeechCheckBox.Text = "Listen only clear speech";
            this.ignoreEncodedSpeechCheckBox.UseVisualStyleBackColor = true;
            // 
            // showEncodedCheckBox
            // 
            this.showEncodedCheckBox.AutoSize = true;
            this.showEncodedCheckBox.Location = new System.Drawing.Point(6, 6);
            this.showEncodedCheckBox.Name = "showEncodedCheckBox";
            this.showEncodedCheckBox.Size = new System.Drawing.Size(130, 17);
            this.showEncodedCheckBox.TabIndex = 0;
            this.showEncodedCheckBox.Text = "Ignore encrypted data";
            this.showEncodedCheckBox.UseVisualStyleBackColor = true;
            // 
            // displayTimer
            // 
            this.displayTimer.Enabled = true;
            this.displayTimer.Tick += new System.EventHandler(this.displayTimer_Tick);
            // 
            // afcCheckBox
            // 
            this.afcCheckBox.AutoSize = true;
            this.afcCheckBox.Location = new System.Drawing.Point(6, 98);
            this.afcCheckBox.Name = "afcCheckBox";
            this.afcCheckBox.Size = new System.Drawing.Size(222, 17);
            this.afcCheckBox.TabIndex = 5;
            this.afcCheckBox.Text = "Enable AFC (automatic frequency control)";
            this.afcCheckBox.UseVisualStyleBackColor = true;
            // 
            // DialogConfigure
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 335);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogConfigure";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configure";
            this.tabControl1.ResumeLayout(false);
            this.logPage.ResumeLayout(false);
            this.logPage.PerformLayout();
            this.recorderTabPage.ResumeLayout(false);
            this.recorderTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NewFileTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.continueRecordTimeNumericUpDown)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udpPortNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage recorderTabPage;
        private System.Windows.Forms.NumericUpDown continueRecordTimeNumericUpDown;
        private System.Windows.Forms.CheckBox dontWritePauseCheckBox;
        private System.Windows.Forms.CheckBox newFileTimeEnableCheckBox;
        private System.Windows.Forms.Timer displayTimer;
        private System.Windows.Forms.NumericUpDown NewFileTimeNumericUpDown;
        private System.Windows.Forms.FolderBrowserDialog writeFolderBrowserDialog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage logPage;
        private System.Windows.Forms.Button logFolderButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox logFileRulesTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox logEntriesRulesTextBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox logSeparatorTextBox;
        private System.Windows.Forms.FolderBrowserDialog logFolderBrowserDialog;
        private System.Windows.Forms.CheckBox logEnableCheckBox;
        private System.Windows.Forms.Label LogEntryLabel;
        private System.Windows.Forms.Label LogFileLabel;
        private System.Windows.Forms.Button folderButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label resultLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox folderTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.NumericUpDown udpPortNumericUpDown;
        private System.Windows.Forms.CheckBox enableUdpOutputCheckBox;
        private System.Windows.Forms.CheckBox showGroupNameCheckBox;
        private System.Windows.Forms.CheckBox ignoreEncodedSpeechCheckBox;
        private System.Windows.Forms.CheckBox showEncodedCheckBox;
        private System.Windows.Forms.CheckBox afcCheckBox;
    }
}