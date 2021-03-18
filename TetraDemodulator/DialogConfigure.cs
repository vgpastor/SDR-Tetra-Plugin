using System;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SDRSharp.Tetra
{
    public partial class DialogConfigure : Form
    {
        private TetraSettings _tetraSettings;
        private TetraPanel _tetra;

        public DialogConfigure(TetraSettings tetraSettings, TetraPanel tetra)
        {
            _tetraSettings = tetraSettings;
            _tetra = tetra;
            InitializeComponent();

            showEncodedCheckBox.Checked = _tetraSettings.IgnoreEncodedData;
            ignoreEncodedSpeechCheckBox.Checked = _tetraSettings.IgnoreEncodedSpeech;
            showGroupNameCheckBox.Checked = _tetraSettings.ShowGroupName;
            enableUdpOutputCheckBox.Checked = _tetraSettings.UdpEnabled;
            udpPortNumericUpDown.Value = _tetraSettings.UdpPort;
            afcCheckBox.Checked = _tetraSettings.AfcEnabled;

            dontWritePauseCheckBox.Checked = _tetraSettings.DontWritePause;

            continueRecordTimeNumericUpDown.Value = _tetraSettings.ContinueRecordTime / 1000;

            newFileTimeEnableCheckBox.Checked = _tetraSettings.NewFileTimeEnable;
            NewFileTimeNumericUpDown.Value = _tetraSettings.NewFileTime / 1000;

            writeFolderBrowserDialog.SelectedPath = _tetraSettings.WriteFolder;

            folderTextBox.Text = _tetraSettings.FileNameRules;
            folderTextBox_TextChanged(null, null);

            logFolderBrowserDialog.SelectedPath = _tetraSettings.LogWriteFolder;
            logFileRulesTextBox.Text = _tetraSettings.LogFileNameRules;
            logFileRulesTextBox_TextChanged(null, null);
            logEntriesRulesTextBox.Text = _tetraSettings.LogEntryRules;
            logEntriesRulesTextBox_TextChanged(null, null);
            logSeparatorTextBox.Text = _tetraSettings.LogSeparator;
            logEnableCheckBox.Checked = _tetraSettings.LogEnabled;
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            _tetraSettings.DontWritePause = dontWritePauseCheckBox.Checked;
            _tetraSettings.ContinueRecordTime = (int)continueRecordTimeNumericUpDown.Value * 1000;

            _tetraSettings.NewFileTimeEnable = newFileTimeEnableCheckBox.Checked;
            _tetraSettings.NewFileTime = (int)NewFileTimeNumericUpDown.Value * 1000;

            _tetraSettings.FileNameRules = folderTextBox.Text;

            _tetraSettings.LogFileNameRules = logFileRulesTextBox.Text;
            _tetraSettings.LogEntryRules = logEntriesRulesTextBox.Text;
            _tetraSettings.LogSeparator = logSeparatorTextBox.Text;
            _tetraSettings.LogEnabled = logEnableCheckBox.Checked;

            _tetraSettings.IgnoreEncodedData = showEncodedCheckBox.Checked;
            _tetraSettings.IgnoreEncodedSpeech = ignoreEncodedSpeechCheckBox.Checked;
            _tetraSettings.ShowGroupName = showGroupNameCheckBox.Checked;
            _tetraSettings.UdpEnabled = enableUdpOutputCheckBox.Checked;
            _tetraSettings.UdpPort = (int) udpPortNumericUpDown.Value;

            _tetraSettings.AfcEnabled = afcCheckBox.Checked;

            DialogResult = DialogResult.OK;
        }

        private void folderTextBox_TextChanged(object sender, EventArgs e)
        {
            resultLabel.Text = _tetra.ParseStringToPath(folderTextBox.Text, ".wav");
        }

        private void displayTimer_Tick(object sender, EventArgs e)
        {
            NewFileTimeNumericUpDown.Enabled = newFileTimeEnableCheckBox.Checked;
            continueRecordTimeNumericUpDown.Enabled = dontWritePauseCheckBox.Checked;
        }

        private void folderButton_Click(object sender, EventArgs e)
        {
            if (writeFolderBrowserDialog.ShowDialog() == DialogResult.OK) _tetraSettings.WriteFolder = writeFolderBrowserDialog.SelectedPath;
        }
        
        private void logFolderButton_Click(object sender, EventArgs e)
        {
            if (logFolderBrowserDialog.ShowDialog() == DialogResult.OK) _tetraSettings.LogWriteFolder = logFolderBrowserDialog.SelectedPath;
        }

        private void logFileRulesTextBox_TextChanged(object sender, EventArgs e)
        {
            LogFileLabel.Text = _tetra.ParseStringToPath(logFileRulesTextBox.Text, ".csv");
        }

        private void logEntriesRulesTextBox_TextChanged(object sender, EventArgs e)
        {
            LogEntryLabel.Text = _tetra.ParseStringToEntries(logEntriesRulesTextBox.Text, null);
        }
    }
}
