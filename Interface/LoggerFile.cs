using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDRSharp.Plugin.Tetra.Interface
{
    class LoggerFile
    {
        public static void Logger(string message)
        {
            SDRSharp.Tetra.TextFile textFile = new SDRSharp.Tetra.TextFile();
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
            for (int i = offset; i < offset + length; i++)
            {
                str += bitsBuffer[i];
            }
            Logger("Origin Bits " + str);
        }

    }
}
