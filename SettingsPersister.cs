// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.SettingsPersister
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SDRSharp.Tetra
{
    public class SettingsPersister
    {
        private string _filename;
        private readonly string _settingsFolder;

        public SettingsPersister(string fileName)
        {
            this._filename = fileName;
            this._settingsFolder = Path.GetDirectoryName(Application.ExecutablePath);
        }

        public TetraSettings ReadStored() => this.ReadObject<TetraSettings>(this._filename) ?? new TetraSettings();

        public void PersistStored(TetraSettings entries) => this.WriteObject<TetraSettings>(entries, this._filename);

        private T ReadObject<T>(string fileName)
        {
            string path = Path.Combine(this._settingsFolder, fileName);
            if (!File.Exists(path))
                return default(T);
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                return (T)new XmlSerializer(typeof(T)).Deserialize((Stream)fileStream);
        }

        private void WriteObject<T>(T obj, string fileName)
        {
            using (FileStream fileStream = new FileStream(Path.Combine(this._settingsFolder, fileName), FileMode.Create))
                new XmlSerializer(obj.GetType()).Serialize((Stream)fileStream, (object)obj);
        }
    }
}
