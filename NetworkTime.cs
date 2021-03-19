namespace SDRSharp.Tetra
{
    class NetworkTime
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

        public int TimeSlot
        {
            get { return _tn; }
        }

        public int TimeSlotSlave
        {
            get { return _tnSlave; }
        }

        public int Frame
        {
            get { return _fn; }
        }

        public int FrameSlave
        {
            get { return _fnSlave; }
        }

        public int SuperFrame
        {
            get { return _mn; }
        }

        public bool IsSynchronized
        {
            get { return _isSynchronized; }
        }

        public bool TimeForBSCH
        {
            get { return _timeBSCH; }
        }

        public bool TimeForBNCH
        {
            get { return _timeBNCH; }
        }

        public bool Frame18
        {
            get { return _fn18; }
        }

        public bool Frame18Slave
        {
            get { return _fn18slave; }
        }

        public void Synchronize(int tn, int fn, int mn)
        {
            tn++;

            _isSynchronized = (tn == _tn) && (fn == _fn) && (mn == _mn);

            _tn = tn < 5 ? (tn > 0 ? tn : 1) : 4;
            _fn = fn < 19 ? (fn > 0 ? fn : 1) : 18;
            _mn = mn < 61 ? (mn > 0 ? mn : 1) : 60;
            CalculateSlaveTime();
            TimeChecker();
        }

        public void SynchronizeMaster(int tn, int fn)
        {
            tn++;

            _isSynchronized = (tn == _tn) && (fn == _fn);

            _tn = tn < 5 ? (tn > 0 ? tn : 1) : 4;
            _fn = fn < 19 ? (fn > 0 ? fn : 1) : 18;

            CalculateSlaveTime();
            TimeChecker();
        }

        public void SynchronizeSlave(int tn, int fn)
        {
            tn++;

            _isSynchronized = (tn == _tnSlave) && (fn == _fnSlave);

            _tn = tn + 3;
            _fn = fn;

            if (_tn > 4)
            {
                _tn -= 4;
                _fn += 1;
            }

            CalculateSlaveTime();
            TimeChecker();
        }


        public void AddTimeSlot()
        {
            _tn = _tn + 1;
            if (_tn > 4)
            {
                _tn = 1;
                _fn += 1;
            }

            if (_fn > 18)
            {
                _fn = 1;
                _mn += 1;
            }

            if (_mn > 60)
            {
                _mn = 1;
            }

            CalculateSlaveTime();
            TimeChecker();
        }

        private void TimeChecker()
        {
            _fn18 = _fn == 18;
            _timeBSCH = ((4 - (_mn + 1) % 4) == _tn) && _fn18;
            _timeBNCH = ((4 - (_mn + 3) % 4) == _tn) && _fn18;

            _fn18slave = _fnSlave == 18;
        }

        private void CalculateSlaveTime()
        {
            _tnSlave = _tn - 3;
            _fnSlave = _fn;

            if (_tnSlave < 1)
            {
                _tnSlave = _tnSlave + 4;
                _fnSlave--;
            }

            if (_fnSlave < 1)
            {
                _fnSlave += 18;
            }
        }
    }
}
