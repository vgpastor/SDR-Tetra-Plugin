// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.TetraSettings
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using System.Collections.Generic;

namespace SDRSharp.Tetra
{
    public class TetraSettings
    {
        public string LogFileNameRules { get; set; }

        public string LogWriteFolder { get; set; }

        public string LogEntryRules { get; set; }

        public string LogSeparator { get; set; }

        public bool LogEnabled { get; set; }

        public int BlockedLevel { get; set; }

        public bool TopMostInfo { get; set; }

        public List<GroupsEntries> NetworkBase { get; set; }

        public bool AutoPlay { get; set; }

        public bool IgnoreEncodedSpeech { get; set; }

        public bool UdpEnabled { get; set; }

        public int UdpPort { get; set; }

        public bool AfcDisabled { get; set; }
    }
}
