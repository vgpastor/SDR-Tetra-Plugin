// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.TextFile
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SDRSharp.Tetra
{
  internal class TextFile
  {
    public void Write(string line, string path)
    {
      using (StreamWriter streamWriter = new StreamWriter(path, true, Encoding.Default))
        streamWriter.WriteLine(line);
    }

    public List<string> Read(string path)
    {
      List<string> stringList = new List<string>();
      using (StreamReader streamReader = new StreamReader(path, Encoding.Default))
      {
        while (streamReader.Peek() >= 0)
          stringList.Add(streamReader.ReadLine());
      }
      return stringList;
    }
  }
}
