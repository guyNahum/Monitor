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
        public double _result;

        public SampleResponse(double result)
        {
            _result = result;
        }
    }
}
