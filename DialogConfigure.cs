using System;
using System.Windows.Forms;

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

            ignoreEncodedSpeechCheckBox.Checked = _tetraSettings.IgnoreEncodedSpeech;
            enableUdpOutputCheckBox.Checked = _tetraSettings.UdpEnabled;
            udpPortNumericUpDown.Value = _tetraSettings.UdpPort;
            afcCheckBox.Checked = _tetraSettings.AfcDisabled;

            logFolderBrowserDialog.SelectedPath = _tetraSettings.LogWriteFolder;
            logFileRulesTextBox.Text = _tetraSettings.LogFileNameRules;
            LogFileRulesTextBox_TextChanged(null, null);
            logEntriesRulesTextBox.Text = _tetraSettings.LogEntryRules;
            LogEntriesRulesTextBox_TextChanged(null, null);
            logSeparatorTextBox.Text = _tetraSettings.LogSeparator;
            logEnableCheckBox.Checked = _tetraSettings.LogEnabled;
        }


        private void BtnOk_Click(object sender, EventArgs e)
        {
            _tetraSettings.LogFileNameRules = logFileRulesTextBox.Text;
            _tetraSettings.LogEntryRules = logEntriesRulesTextBox.Text;
            _tetraSettings.LogSeparator = logSeparatorTextBox.Text;
            _tetraSettings.LogEnabled = logEnableCheckBox.Checked;

            _tetraSettings.IgnoreEncodedSpeech = ignoreEncodedSpeechCheckBox.Checked;
            _tetraSettings.UdpEnabled = enableUdpOutputCheckBox.Checked;
            _tetraSettings.UdpPort = (int)udpPortNumericUpDown.Value;

            _tetraSettings.AfcDisabled = afcCheckBox.Checked;

            DialogResult = DialogResult.OK;
        }

        private void LogFolderButton_Click(object sender, EventArgs e)
        {
            if (logFolderBrowserDialog.ShowDialog() == DialogResult.OK) _tetraSettings.LogWriteFolder = logFolderBrowserDialog.SelectedPath;
        }

        private void LogFileRulesTextBox_TextChanged(object sender, EventArgs e)
        {
            LogFileLabel.Text = _tetra.ParseStringToPath(logFileRulesTextBox.Text, ".csv");
        }

        private void LogEntriesRulesTextBox_TextChanged(object sender, EventArgs e)
        {
            LogEntryLabel.Text = _tetra.ParseStringToEntries(logEntriesRulesTextBox.Text, null);
        }
    }
}
