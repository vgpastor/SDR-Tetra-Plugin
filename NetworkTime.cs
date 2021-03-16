// Decompiled with JetBrains decompiler
// Type: SDRSharp.Tetra.NetworkTime
// Assembly: SDRSharp.Tetra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C3C6F0AC-F9E4-4213-8F19-E6F878CA40B0
// Assembly location: E:\RADIO\SdrSharp1810\Plugins\tetra1.0.0.0\SDRSharp.Tetra.dll

namespace SDRSharp.Tetra
{
    internal class NetworkTime
    {
        private int _tn = 1;
        private int _fn = 1;
        private int _mn = 1;
        private bool _isSynchronized;
        private bool _timeBNCH;
        private bool _fn18;
        private bool _timeBSCH;
        private bool _fn18slave;
        private int _tnSlave;
        private int _fnSlave;

        public int TimeSlot => this._tn;

        public int TimeSlotSlave => this._tnSlave;

        public int Frame => this._fn;

        public int FrameSlave => this._fnSlave;

        public int SuperFrame => this._mn;

        public bool IsSynchronized => this._isSynchronized;

        public bool TimeForBSCH => this._timeBSCH;

        public bool TimeForBNCH => this._timeBNCH;

        public bool Frame18 => this._fn18;

        public bool Frame18Slave => this._fn18slave;

        public void Synchronize(int tn, int fn, int mn)
        {
            ++tn;
            this._isSynchronized = tn == this._tn && fn == this._fn && mn == this._mn;
            this._tn = tn < 5 ? (tn > 0 ? tn : 1) : 4;
            this._fn = fn < 19 ? (fn > 0 ? fn : 1) : 18;
            this._mn = mn < 61 ? (mn > 0 ? mn : 1) : 60;
            this.CalculateSlaveTime();
            this.TimeChecker();
        }

        public void SynchronizeMaster(int tn, int fn)
        {
            ++tn;
            this._isSynchronized = tn == this._tn && fn == this._fn;
            this._tn = tn < 5 ? (tn > 0 ? tn : 1) : 4;
            this._fn = fn < 19 ? (fn > 0 ? fn : 1) : 18;
            this.CalculateSlaveTime();
            this.TimeChecker();
        }

        public void SynchronizeSlave(int tn, int fn)
        {
            ++tn;
            this._isSynchronized = tn == this._tnSlave && fn == this._fnSlave;
            this._tn = tn + 3;
            this._fn = fn;
            if (this._tn > 4)
            {
                this._tn -= 4;
                ++this._fn;
            }
            this.CalculateSlaveTime();
            this.TimeChecker();
        }

        public void AddTimeSlot()
        {
            ++this._tn;
            if (this._tn > 4)
            {
                this._tn = 1;
                ++this._fn;
            }
            if (this._fn > 18)
            {
                this._fn = 1;
                ++this._mn;
            }
            if (this._mn > 60)
                this._mn = 1;
            this.CalculateSlaveTime();
            this.TimeChecker();
        }

        private void TimeChecker()
        {
            this._fn18 = this._fn == 18;
            this._timeBSCH = 4 - (this._mn + 1) % 4 == this._tn && this._fn18;
            this._timeBNCH = 4 - (this._mn + 3) % 4 == this._tn && this._fn18;
            this._fn18slave = this._fnSlave == 18;
        }

        private void CalculateSlaveTime()
        {
            this._tnSlave = this._tn - 3;
            this._fnSlave = this._fn;
            if (this._tnSlave < 1)
            {
                this._tnSlave += 4;
                --this._fnSlave;
            }
            if (this._fnSlave >= 1)
                return;
            this._fnSlave += 18;
        }
    }
}
