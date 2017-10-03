using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable()]
    public struct AdapterStatistics
    {       
        public long _bytesSent;
        public long _bytesRecieved;

        public AdapterStatistics(long bytesSent = 0, long bytesRecieved = 0)
        {
            _bytesRecieved = bytesRecieved;
            _bytesSent = bytesSent;
        }

        public override string ToString()
        {
            return string.Format("*bytes sent {0}, bytes recieved {1}", _bytesSent, _bytesRecieved);
        }
    }
}
