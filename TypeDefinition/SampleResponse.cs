using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable()]
    public struct SampleResponse
    {
        public object _result;

        public SampleResponse(object result)
        {
            _result = result;
        }
    }
}
