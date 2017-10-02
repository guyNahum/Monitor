using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    public class Sample
    {
        protected DateTime _time;
        public double _value;
        public IPAddress _ip;
        
        public Sample(double value, IPAddress ip)
        {
            _ip = ip;
            _value = value;
            _time = DateTime.Now;
        }
    }

    public class CPUSample : Sample
    {
        public CPUSample(Sample sample) : base(sample._value, sample._ip) { }

        public override string ToString()
        {
            return string.Format("{0} :: {1} אחוז", _time.ToLongTimeString(), _value);
        }
    }

    public class MemorySample : Sample
    {
        public MemorySample(Sample sample) : base(sample._value, sample._ip) { }

        public override string ToString()
        {
            return string.Format("{0} :: {1} מ''ב ", _time.ToLongTimeString(), _value);
        }
    }

    public class PingSample : Sample
    {
        private const string FAIL_MESSAGE = "Ping failed";
        public PingSample(Sample sample) : base(sample._value, sample._ip) { }

        public override string ToString()
        {
            return _value == -1 ? FAIL_MESSAGE : 
                                  string.Format("{0} :: {1} שניות", _time.ToLongTimeString(), _value);
        }
    }
}
