using System.Runtime.InteropServices;


namespace SDRSharp.Tetra
{
    public unsafe static class NativeMethods
    {
        private const string LibTetraDecoder = "tetraVoiceDec";

        [DllImport(LibTetraDecoder, CallingConvention = CallingConvention.Cdecl)]
        public static extern void* tetra_decode_init();

        [DllImport(LibTetraDecoder, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tetra_cdec(int fp, byte* inp, short* outp, int hs);

        [DllImport(LibTetraDecoder, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tetra_sdec(short* inp, short* outp, void* chStruct);
    }
}
