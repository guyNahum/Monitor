using System;
using System.Net;

namespace Common
{
    [Serializable()]
    public struct SampleRequest
    {
        public SamplesEnum _sampleType;
        public object _parameter;

        public SampleRequest(SamplesEnum sample, object parameter = null)
        {
            _sampleType = sample;
            _parameter = parameter;
        }
    }
}
