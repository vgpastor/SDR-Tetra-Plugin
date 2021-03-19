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
            _filename = fileName;
            _settingsFolder = Path.GetDirectoryName(Application.ExecutablePath);
        }

        public TetraSettings ReadStored()
        {
            var result = ReadObject<TetraSettings>(_filename);
            if (result != null)
            {
                return result;
            }
            return new TetraSettings();
        }

        public void PersistStored(TetraSettings entries)
        {
            WriteObject(entries, _filename);
        }

        private T ReadObject<T>(string fileName)
        {
            var filePath = Path.Combine(_settingsFolder, fileName);
            if (File.Exists(filePath))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var ser = new XmlSerializer(typeof(T));
                    return (T)ser.Deserialize(fileStream);
                }
            }
            return default(T);
        }

        private void WriteObject<T>(T obj, string fileName)
        {
            var filePath = Path.Combine(_settingsFolder, fileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                var ser = new XmlSerializer(obj.GetType());
                ser.Serialize(fileStream, obj);
            }
        }
    }
}
