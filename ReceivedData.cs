// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.ReceivedData
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

using System.Runtime.CompilerServices;

namespace SDRSharp.Tetra
{
    public class ReceivedData
    {
        public int[] Data;

        public ReceivedData()
        {
            this.Data = new int[280];
            this.Clear();
        }

        public void Clear()
        {
            for (int index = 0; index < this.Data.Length; ++index)
                this.Data[index] = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(GlobalNames name, ref int value)
        {
            if (this.Data[(int)name] == -1)
                return false;
            value = this.Data[(int)name];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(GlobalNames name) => this.Data[(int)name] != -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Value(GlobalNames name) => this.Data[(int)name];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(GlobalNames name, int value) => this.Data[(int)name] = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(GlobalNames name, int value) => this.Data[(int)name] = value;
    }
}
