using System;
using System.Net;

namespace Common
{
    [Serializable()]
    public struct SampleRequest
    {
        public SamplesEnum _sampleType;
        public IPAddress _ip;

        public SampleRequest(SamplesEnum sample, IPAddress ip = null)
        {
            _sampleType = sample;
            _ip = ip;
        }
    }
}
