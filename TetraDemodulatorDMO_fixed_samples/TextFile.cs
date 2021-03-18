using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SDRSharp.Tetra
{
    class TextFile
    {
        public void Write(string line, string path)
        {
            using (StreamWriter sw = new StreamWriter(path, true, Encoding.Default))
            {
                sw.WriteLine(line);
            }
        }

        public List<string> Read(string path)
        {
            var result = new List<string>();

            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                
                while (sr.Peek() >= 0)
                {
                    result.Add(sr.ReadLine());
                }
            }
            return result;
        }
    }
}
