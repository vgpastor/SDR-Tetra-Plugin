// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.NativeMethods
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using System.Runtime.InteropServices;

namespace SDRSharp.Tetra
{
    public static class NativeMethods
    {
        private const string LibTetraDecoder = "tetraVoiceDec";

        [DllImport("tetraVoiceDec", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void* tetra_decode_init();

        [DllImport("tetraVoiceDec", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int tetra_cdec(int fp, byte* inp, short* outp, int hs);

        [DllImport("tetraVoiceDec", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int tetra_sdec(short* inp, short* outp, void* chStruct);
    }
}
