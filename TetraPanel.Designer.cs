namespace SDRSharp.Tetra
{
    partial class TetraPanel
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.merLabel = new System.Windows.Forms.Label();
            this.berLabel = new System.Windows.Forms.Label();
            this.ferLabel = new System.Windows.Forms.Label();
            this.mainFrequencyLinkLabel = new System.Windows.Forms.LinkLabel();
            this.currentFrequencyLabel = new System.Windows.Forms.Label();
            this.currentCarrierLabel = new System.Windows.Forms.Label();
            this.mainCarrierLabel = new System.Windows.Forms.Label();
            this.freqLabel = new System.Windows.Forms.Label();
            this.currentFreqLabel = new System.Windows.Forms.Label();
            this.netInfoButton = new System.Windows.Forms.Button();
            this.laLabel = new System.Windows.Forms.Label();
            this.colorLabel = new System.Windows.Forms.Label();
            this.mncLabel = new System.Windows.Forms.Label();
            this.mccLabel = new System.Windows.Forms.Label();
            this.configButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.blockNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.gssiLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.issiLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.autoCheckBox = new System.Windows.Forms.CheckBox();
            this.ch4RadioButton = new System.Windows.Forms.RadioButton();
            this.ch3RadioButton = new System.Windows.Forms.RadioButton();
            this.ch2RadioButton = new System.Windows.Forms.RadioButton();
            this.ch1RadioButton = new System.Windows.Forms.RadioButton();
            this.displayGroupBox = new System.Windows.Forms.GroupBox();
            this.display = new SDRSharp.Tetra.Display();
            this.enabledCheckBox = new System.Windows.Forms.CheckBox();
            this.connectLabel = new System.Windows.Forms.Label();
            this.markerTimer = new System.Windows.Forms.Timer(this.components);
            this.timerGui = new System.Windows.Forms.Timer(this.components);
            this.afcTimer = new System.Windows.Forms.Timer(this.components);
            this.dataExtractorTimer = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blockNumericUpDown)).BeginInit();
            this.displayGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(226, 493);
            this.panel1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.merLabel);
            this.groupBox2.Controls.Add(this.berLabel);
            this.groupBox2.Controls.Add(this.ferLabel);
            this.groupBox2.Controls.Add(this.mainFrequencyLinkLabel);
            this.groupBox2.Controls.Add(this.currentFrequencyLabel);
            this.groupBox2.Controls.Add(this.currentCarrierLabel);
            this.groupBox2.Controls.Add(this.mainCarrierLabel);
            this.groupBox2.Controls.Add(this.freqLabel);
            this.groupBox2.Controls.Add(this.currentFreqLabel);
            this.groupBox2.Controls.Add(this.netInfoButton);
            this.groupBox2.Controls.Add(this.laLabel);
            this.groupBox2.Controls.Add(this.colorLabel);
            this.groupBox2.Controls.Add(this.mncLabel);
            this.groupBox2.Controls.Add(this.mccLabel);
            this.groupBox2.Controls.Add(this.configButton);
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Controls.Add(this.displayGroupBox);
            this.groupBox2.Controls.Add(this.enabledCheckBox);
            this.groupBox2.Controls.Add(this.connectLabel);
            this.groupBox2.Location = new System.Drawing.Point(3, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(220, 490);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Demodulator";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(91, 104);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "label11";
            // 
            // merLabel
            // 
            this.merLabel.AutoSize = true;
            this.merLabel.Location = new System.Drawing.Point(143, 271);
            this.merLabel.Name = "merLabel";
            this.merLabel.Size = new System.Drawing.Size(31, 13);
            this.merLabel.TabIndex = 29;
            this.merLabel.Text = "MER";
            // 
            // berLabel
            // 
            this.berLabel.AutoSize = true;
            this.berLabel.Location = new System.Drawing.Point(81, 271);
            this.berLabel.Name = "berLabel";
            this.berLabel.Size = new System.Drawing.Size(29, 13);
            this.berLabel.TabIndex = 29;
            this.berLabel.Text = "BER";
            // 
            // ferLabel
            // 
            this.ferLabel.AutoSize = true;
            this.ferLabel.Location = new System.Drawing.Point(14, 271);
            this.ferLabel.Name = "ferLabel";
            this.ferLabel.Size = new System.Drawing.Size(28, 13);
            this.ferLabel.TabIndex = 29;
            this.ferLabel.Text = "FER";
            // 
            // mainFrequencyLinkLabel
            // 
            this.mainFrequencyLinkLabel.AutoSize = true;
            this.mainFrequencyLinkLabel.Location = new System.Drawing.Point(130, 38);
            this.mainFrequencyLinkLabel.Name = "mainFrequencyLinkLabel";
            this.mainFrequencyLinkLabel.Size = new System.Drawing.Size(38, 13);
            this.mainFrequencyLinkLabel.TabIndex = 70;
            this.mainFrequencyLinkLabel.TabStop = true;
            this.mainFrequencyLinkLabel.Text = "0 MHz";
            this.mainFrequencyLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.MainFrequencyLinkLabel_LinkClicked);
            // 
            // currentFrequencyLabel
            // 
            this.currentFrequencyLabel.AutoSize = true;
            this.currentFrequencyLabel.Location = new System.Drawing.Point(130, 74);
            this.currentFrequencyLabel.Name = "currentFrequencyLabel";
            this.currentFrequencyLabel.Size = new System.Drawing.Size(38, 13);
            this.currentFrequencyLabel.TabIndex = 69;
            this.currentFrequencyLabel.Text = "0 MHz";
            // 
            // currentCarrierLabel
            // 
            this.currentCarrierLabel.AutoSize = true;
            this.currentCarrierLabel.Location = new System.Drawing.Point(92, 74);
            this.currentCarrierLabel.Name = "currentCarrierLabel";
            this.currentCarrierLabel.Size = new System.Drawing.Size(31, 13);
            this.currentCarrierLabel.TabIndex = 68;
            this.currentCarrierLabel.Text = "0000";
            // 
            // mainCarrierLabel
            // 
            this.mainCarrierLabel.AutoSize = true;
            this.mainCarrierLabel.Location = new System.Drawing.Point(91, 38);
            this.mainCarrierLabel.Name = "mainCarrierLabel";
            this.mainCarrierLabel.Size = new System.Drawing.Size(31, 13);
            this.mainCarrierLabel.TabIndex = 67;
            this.mainCarrierLabel.Text = "0000";
            // 
            // freqLabel
            // 
            this.freqLabel.AutoSize = true;
            this.freqLabel.Location = new System.Drawing.Point(90, 20);
            this.freqLabel.Name = "freqLabel";
            this.freqLabel.Size = new System.Drawing.Size(33, 13);
            this.freqLabel.TabIndex = 54;
            this.freqLabel.Text = "Main:";
            // 
            // currentFreqLabel
            // 
            this.currentFreqLabel.AutoSize = true;
            this.currentFreqLabel.Location = new System.Drawing.Point(91, 56);
            this.currentFreqLabel.Name = "currentFreqLabel";
            this.currentFreqLabel.Size = new System.Drawing.Size(44, 13);
            this.currentFreqLabel.TabIndex = 60;
            this.currentFreqLabel.Text = "Current:";
            // 
            // netInfoButton
            // 
            this.netInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.netInfoButton.Location = new System.Drawing.Point(143, 99);
            this.netInfoButton.Name = "netInfoButton";
            this.netInfoButton.Size = new System.Drawing.Size(69, 23);
            this.netInfoButton.TabIndex = 59;
            this.netInfoButton.Text = "Net Info";
            this.netInfoButton.UseVisualStyleBackColor = true;
            this.netInfoButton.Click += new System.EventHandler(this.NetInfoButton_Click);
            // 
            // laLabel
            // 
            this.laLabel.AutoSize = true;
            this.laLabel.Location = new System.Drawing.Point(6, 56);
            this.laLabel.Name = "laLabel";
            this.laLabel.Size = new System.Drawing.Size(23, 13);
            this.laLabel.TabIndex = 53;
            this.laLabel.Text = "LA:";
            // 
            // colorLabel
            // 
            this.colorLabel.AutoSize = true;
            this.colorLabel.Location = new System.Drawing.Point(6, 74);
            this.colorLabel.Name = "colorLabel";
            this.colorLabel.Size = new System.Drawing.Size(34, 13);
            this.colorLabel.TabIndex = 49;
            this.colorLabel.Text = "Color:";
            // 
            // mncLabel
            // 
            this.mncLabel.AutoSize = true;
            this.mncLabel.Location = new System.Drawing.Point(6, 38);
            this.mncLabel.Name = "mncLabel";
            this.mncLabel.Size = new System.Drawing.Size(34, 13);
            this.mncLabel.TabIndex = 48;
            this.mncLabel.Text = "MNC:";
            // 
            // mccLabel
            // 
            this.mccLabel.AutoSize = true;
            this.mccLabel.Location = new System.Drawing.Point(6, 20);
            this.mccLabel.Name = "mccLabel";
            this.mccLabel.Size = new System.Drawing.Size(33, 13);
            this.mccLabel.TabIndex = 47;
            this.mccLabel.Text = "MCC:";
            // 
            // configButton
            // 
            this.configButton.Location = new System.Drawing.Point(6, 99);
            this.configButton.Name = "configButton";
            this.configButton.Size = new System.Drawing.Size(69, 23);
            this.configButton.TabIndex = 66;
            this.configButton.Text = "Config";
            this.configButton.UseVisualStyleBackColor = true;
            this.configButton.Click += new System.EventHandler(this.ConfigButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.blockNumericUpDown);
            this.groupBox1.Controls.Add(this.gssiLabel);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.issiLabel);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.autoCheckBox);
            this.groupBox1.Controls.Add(this.ch4RadioButton);
            this.groupBox1.Controls.Add(this.ch3RadioButton);
            this.groupBox1.Controls.Add(this.ch2RadioButton);
            this.groupBox1.Controls.Add(this.ch1RadioButton);
            this.groupBox1.Location = new System.Drawing.Point(5, 128);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(209, 136);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Voice";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 111);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(94, 13);
            this.label12.TabIndex = 73;
            this.label12.Text = "Blocking threshold";
            // 
            // blockNumericUpDown
            // 
            this.blockNumericUpDown.Location = new System.Drawing.Point(128, 109);
            this.blockNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.blockNumericUpDown.Name = "blockNumericUpDown";
            this.blockNumericUpDown.Size = new System.Drawing.Size(47, 20);
            this.blockNumericUpDown.TabIndex = 72;
            this.blockNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.blockNumericUpDown.ValueChanged += new System.EventHandler(this.BlockNumericUpDown_ValueChanged);
            // 
            // gssiLabel
            // 
            this.gssiLabel.AutoSize = true;
            this.gssiLabel.Location = new System.Drawing.Point(118, 17);
            this.gssiLabel.Name = "gssiLabel";
            this.gssiLabel.Size = new System.Drawing.Size(20, 13);
            this.gssiLabel.TabIndex = 71;
            this.gssiLabel.Text = "To";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(118, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 70;
            this.label6.Text = "0";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(118, 71);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 69;
            this.label7.Text = "0";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(118, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 13);
            this.label8.TabIndex = 68;
            this.label8.Text = "0";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(118, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 13);
            this.label9.TabIndex = 67;
            this.label9.Text = "0";
            // 
            // issiLabel
            // 
            this.issiLabel.AutoSize = true;
            this.issiLabel.Location = new System.Drawing.Point(53, 17);
            this.issiLabel.Name = "issiLabel";
            this.issiLabel.Size = new System.Drawing.Size(30, 13);
            this.issiLabel.TabIndex = 65;
            this.issiLabel.Text = "From";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(53, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 64;
            this.label4.Text = "0";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(53, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 63;
            this.label3.Text = "0";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(53, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 62;
            this.label2.Text = "0";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(53, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 61;
            this.label1.Text = "0";
            // 
            // autoCheckBox
            // 
            this.autoCheckBox.AutoSize = true;
            this.autoCheckBox.Location = new System.Drawing.Point(3, 16);
            this.autoCheckBox.Name = "autoCheckBox";
            this.autoCheckBox.Size = new System.Drawing.Size(48, 17);
            this.autoCheckBox.TabIndex = 60;
            this.autoCheckBox.Text = "Auto";
            this.autoCheckBox.UseVisualStyleBackColor = true;
            this.autoCheckBox.CheckedChanged += new System.EventHandler(this.AutoCheckBox_CheckedChanged);
            // 
            // ch4RadioButton
            // 
            this.ch4RadioButton.AutoSize = true;
            this.ch4RadioButton.Location = new System.Drawing.Point(3, 86);
            this.ch4RadioButton.Name = "ch4RadioButton";
            this.ch4RadioButton.Size = new System.Drawing.Size(46, 17);
            this.ch4RadioButton.TabIndex = 57;
            this.ch4RadioButton.TabStop = true;
            this.ch4RadioButton.Text = "Ts 4";
            this.ch4RadioButton.UseVisualStyleBackColor = true;
            this.ch4RadioButton.CheckedChanged += new System.EventHandler(this.Ch4RadioButton_CheckedChanged);
            // 
            // ch3RadioButton
            // 
            this.ch3RadioButton.AutoSize = true;
            this.ch3RadioButton.Location = new System.Drawing.Point(3, 69);
            this.ch3RadioButton.Name = "ch3RadioButton";
            this.ch3RadioButton.Size = new System.Drawing.Size(46, 17);
            this.ch3RadioButton.TabIndex = 56;
            this.ch3RadioButton.TabStop = true;
            this.ch3RadioButton.Text = "Ts 3";
            this.ch3RadioButton.UseVisualStyleBackColor = true;
            this.ch3RadioButton.CheckedChanged += new System.EventHandler(this.Ch3RadioButton_CheckedChanged);
            // 
            // ch2RadioButton
            // 
            this.ch2RadioButton.AutoSize = true;
            this.ch2RadioButton.Location = new System.Drawing.Point(3, 52);
            this.ch2RadioButton.Name = "ch2RadioButton";
            this.ch2RadioButton.Size = new System.Drawing.Size(46, 17);
            this.ch2RadioButton.TabIndex = 55;
            this.ch2RadioButton.TabStop = true;
            this.ch2RadioButton.Text = "Ts 2";
            this.ch2RadioButton.UseVisualStyleBackColor = true;
            this.ch2RadioButton.CheckedChanged += new System.EventHandler(this.Ch2RadioButton_CheckedChanged);
            // 
            // ch1RadioButton
            // 
            this.ch1RadioButton.AutoSize = true;
            this.ch1RadioButton.Location = new System.Drawing.Point(3, 35);
            this.ch1RadioButton.Name = "ch1RadioButton";
            this.ch1RadioButton.Size = new System.Drawing.Size(46, 17);
            this.ch1RadioButton.TabIndex = 54;
            this.ch1RadioButton.TabStop = true;
            this.ch1RadioButton.Text = "Ts 1";
            this.ch1RadioButton.UseVisualStyleBackColor = true;
            this.ch1RadioButton.CheckedChanged += new System.EventHandler(this.Ch1RadioButton_CheckedChanged);
            // 
            // displayGroupBox
            // 
            this.displayGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.displayGroupBox.Controls.Add(this.display);
            this.displayGroupBox.Location = new System.Drawing.Point(5, 291);
            this.displayGroupBox.Name = "displayGroupBox";
            this.displayGroupBox.Size = new System.Drawing.Size(209, 171);
            this.displayGroupBox.TabIndex = 42;
            this.displayGroupBox.TabStop = false;
            this.displayGroupBox.Text = "Diagram";
            // 
            // display
            // 
            this.display.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.display.BackColor = System.Drawing.Color.Black;
            this.display.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.display.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.display.Location = new System.Drawing.Point(6, 19);
            this.display.Name = "display";
            this.display.Size = new System.Drawing.Size(193, 148);
            this.display.TabIndex = 27;
            // 
            // enabledCheckBox
            // 
            this.enabledCheckBox.AutoSize = true;
            this.enabledCheckBox.Location = new System.Drawing.Point(9, 0);
            this.enabledCheckBox.Name = "enabledCheckBox";
            this.enabledCheckBox.Size = new System.Drawing.Size(86, 17);
            this.enabledCheckBox.TabIndex = 0;
            this.enabledCheckBox.Text = "Demodulator";
            this.enabledCheckBox.UseVisualStyleBackColor = true;
            this.enabledCheckBox.CheckedChanged += new System.EventHandler(this.EnabledCheckBox_CheckedChanged);
            // 
            // connectLabel
            // 
            this.connectLabel.AutoSize = true;
            this.connectLabel.ForeColor = System.Drawing.Color.Red;
            this.connectLabel.Location = new System.Drawing.Point(143, 1);
            this.connectLabel.Name = "connectLabel";
            this.connectLabel.Size = new System.Drawing.Size(53, 13);
            this.connectLabel.TabIndex = 45;
            this.connectLabel.Text = "Received";
            this.connectLabel.Visible = false;
            // 
            // markerTimer
            // 
            this.markerTimer.Enabled = true;
            this.markerTimer.Interval = 50;
            this.markerTimer.Tick += new System.EventHandler(this.MarkerTimer_Tick);
            // 
            // timerGui
            // 
            this.timerGui.Enabled = true;
            this.timerGui.Interval = 500;
            this.timerGui.Tick += new System.EventHandler(this.TimerGui_Tick);
            // 
            // afcTimer
            // 
            this.afcTimer.Enabled = true;
            this.afcTimer.Interval = 1000;
            this.afcTimer.Tick += new System.EventHandler(this.AfcTimer_Tick);
            // 
            // dataExtractorTimer
            // 
            this.dataExtractorTimer.Enabled = true;
            this.dataExtractorTimer.Interval = 50;
            this.dataExtractorTimer.Tick += new System.EventHandler(this.DataExtractorTimer_Tick);
            // 
            // TetraPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "TetraPanel";
            this.Size = new System.Drawing.Size(229, 499);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blockNumericUpDown)).EndInit();
            this.displayGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Timer markerTimer;
        private System.Windows.Forms.CheckBox enabledCheckBox;
        private Display display;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox displayGroupBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label connectLabel;
        private System.Windows.Forms.Timer timerGui;
        private System.Windows.Forms.RadioButton ch4RadioButton;
        private System.Windows.Forms.RadioButton ch3RadioButton;
        private System.Windows.Forms.RadioButton ch2RadioButton;
        private System.Windows.Forms.RadioButton ch1RadioButton;
        private System.Windows.Forms.Button netInfoButton;
        private System.Windows.Forms.CheckBox autoCheckBox;
        private System.Windows.Forms.Label colorLabel;
        private System.Windows.Forms.Label mncLabel;
        private System.Windows.Forms.Label mccLabel;
        private System.Windows.Forms.Label laLabel;
        private System.Windows.Forms.Label freqLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label issiLabel;
        private System.Windows.Forms.Button configButton;
        private System.Windows.Forms.Label gssiLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label currentFreqLabel;
        private System.Windows.Forms.NumericUpDown blockNumericUpDown;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label currentCarrierLabel;
        private System.Windows.Forms.Label mainCarrierLabel;
        private System.Windows.Forms.Label currentFrequencyLabel;
        private System.Windows.Forms.LinkLabel mainFrequencyLinkLabel;
        private System.Windows.Forms.Label merLabel;
        private System.Windows.Forms.Label berLabel;
        private System.Windows.Forms.Label ferLabel;
        private System.Windows.Forms.Timer afcTimer;
        private System.Windows.Forms.Timer dataExtractorTimer;
        private System.Windows.Forms.Label label11;
    }
}
