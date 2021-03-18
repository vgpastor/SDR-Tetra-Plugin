using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public int TimeSlot
        {
            get { return _tn; }
        }

        public int Frame
        {
            get { return _fn; }
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

        public void Synchronize(int tn, int fn, int mn)
        {
            tn++;

            _isSynchronized = (tn == _tn) && (fn == _fn) && (mn == _mn);


            _tn = tn < 5 ? (tn > 0 ? tn : 1) : 4;
            _fn = fn < 19 ? (fn > 0 ? fn : 1) : 18;
            _mn = mn < 61 ? (mn > 0 ? mn : 1) : 60;

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

            TimeChecker();
        }

        private void TimeChecker()
        {
            _fn18 = _fn == 18;
            _timeBSCH = ((4 - (_mn + 1) % 4) == _tn) && _fn18;
            _timeBNCH = ((4 - (_mn + 3) % 4) == _tn) && _fn18;
        }
    }
}
