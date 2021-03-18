// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.TetraPlugin
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using SDRSharp.Common;
using System.Windows.Forms;

namespace SDRSharp.Tetra
{
    public class TetraPlugin : ISharpPlugin
    {
        private const string _displayName = "TETRA Demodulator";
        private ISharpControl _controlInterface;
        private TetraPanel _qpskPanel;

        public UserControl Gui => (UserControl)this._qpskPanel;

        public string DisplayName => "TETRA Demodulator";

        public void Initialize(ISharpControl control)
        {
            this._controlInterface = control;
            this._qpskPanel = new TetraPanel(this._controlInterface);
        }

        public void Close() => this._qpskPanel.SaveSettings();

        public static void Logger(string message)
        {
            TextFile textFile = new TextFile();
            string path = "debug_tetra.log";
            try
            {
                textFile.Write(System.DateTime.Now + "->" + message, path);
            }
            catch
            {
                if (MessageBox.Show("Unable to open file " + path, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand) != DialogResult.OK)
                    return;
            }

        }

        public unsafe static void Logger(byte* bitsBuffer, int offset, int length)
        {
            string str = "";
            for(int i = offset; i < offset+length; i++)
            {
                str += bitsBuffer[i];
            }
            TetraPlugin.Logger("Origin Bits " + str);
        }

    }
}
