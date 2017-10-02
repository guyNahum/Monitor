using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using Common;

namespace Monitor
{
    /// <summary>
    /// 
    /// </summary>
    public class MonitoredStation : INotifyPropertyChanged
    {
        #region Fields

        private AgentCommunicator _agentCommunicator;
        private double _maxCPU = 0;
        private double _maxMemory = 0;
        private IPAddress _ip;
        private string _stationName;
        private IPAddress _ipToPing;
        private int _portToMonitor;
        private bool _isCPUMonitored;
        private bool _isMemoryMonitored;
        private bool _isPingMonitored;
        private bool _isPortTransportMonitored;
        private bool _isAgentConnected;
        private SampleRequest _cpuSampleRequest;
        private SampleRequest _memorySampleRequest;
        private SampleRequest _portSampleRequest;
        private SampleRequest _ipSampleRequest;

        #endregion

        #region Properties

        public FixedSizedQueue<Sample> CPUSamples { get; private set; }
        public FixedSizedQueue<Sample> MemorySamples { get; private set; }
        public FixedSizedQueue<Sample> PingSamples { get; private set; }

        public bool IsAgentConnected
        {
            get
            {
                return _isAgentConnected;
            }
            set
            {
                _isAgentConnected = value;

                NotifyPropertyChanged();
            }
        }

        public double MaxCPU
        {
            get
            {
                return _maxCPU;
            }
            set
            {
                _maxCPU = value;

                NotifyPropertyChanged();
            }
        }

        public double MaxMemory
        {
            get
            {
                return _maxMemory;
            }
            set
            {
                _maxMemory = value;

                NotifyPropertyChanged();
            }
        }

        public bool IsCPUMonitored
        {
            get
            {
                return _isCPUMonitored;
            }
            set
            {
                _isCPUMonitored = value;

                if (!_isCPUMonitored)
                {
                    _agentCommunicator.CPUAgent.StopSample(_ip);
                    MaxCPU = 0;
                    CPUSamples.Clear();
                }
                else
                {
                    _agentCommunicator.CPUAgent.StartSample(_ip, _cpuSampleRequest);
                }
            }
        }

        public bool IsMemoryMonitored
        {
            get
            {
                return _isMemoryMonitored;
            }
            set
            {
                _isMemoryMonitored = value;

                if (!_isMemoryMonitored)
                {
                    MaxMemory = 0;
                    _agentCommunicator.MemoryAgent.StopSample(_ip);
                    MemorySamples.Clear();
                }
                else
                {
                    _agentCommunicator.MemoryAgent.StartSample(_ip, _memorySampleRequest);
                }
            }
        }

        public bool IsPingMonitored
        {
            get
            {
                return _isPingMonitored;
            }
            set
            {
                _isPingMonitored = value;

                if (IsPingMonitored)
                {
                    if (IPAddress.TryParse(IPToPing, out _ipToPing))
                    {
                        _ipSampleRequest._ip = _ipToPing;
                        _agentCommunicator.PingAgent.StartSample(_ip, _ipSampleRequest);
                    }
                }
                else
                {
                    _agentCommunicator.PingAgent.StopSample(_ip);
                    PingSamples.Clear();
                }

            }
        }

        public bool IsPortTransportMonitored
        {
            get
            {
                return _isPortTransportMonitored;
            }
            set
            {
                _isPortTransportMonitored = value;

                //IsPortTransportMonitored ? StartMonitor

            }
        }
        
        public string IPToPing { get; set; }

        #endregion

        #region CTOR

        public MonitoredStation(string stationName, IPAddress ip)
        {
            _cpuSampleRequest = new SampleRequest(SamplesEnum.CPU);
            _memorySampleRequest = new SampleRequest(SamplesEnum.MEMORY);
            _portSampleRequest = new SampleRequest(SamplesEnum.PORT);
            _ipSampleRequest = new SampleRequest(SamplesEnum.PING);

            _ip = ip;
            _stationName = stationName;
            _isCPUMonitored = false;
            _isMemoryMonitored = false;
            _isPingMonitored = false;
            _isPortTransportMonitored = false;
            CPUSamples = new FixedSizedQueue<Sample>(10);
            MemorySamples = new FixedSizedQueue<Sample>(10);
            PingSamples = new FixedSizedQueue<Sample>(10);
            _agentCommunicator = AgentCommunicator.Instance;

            _agentCommunicator.MemoryAgent.SampleArrived += delegate (Sample sample)
            {
                if (_ip.Equals(sample._ip))
                {
                    MemorySamples.Add(new MemorySample(sample));

                    if (sample._value > MaxMemory)
                    {
                        MaxMemory = sample._value;
                    }
                }
            };

            _agentCommunicator.CPUAgent.SampleArrived += delegate (Sample sample)
            {
                if (_ip.Equals(sample._ip))
                {
                    CPUSamples.Add(new CPUSample(sample));

                    if (sample._value > MaxCPU)
                    {
                        MaxCPU = sample._value;
                    }
                }
            };
            _agentCommunicator.PingAgent.SampleArrived += delegate (Sample sample)
            {
                if (_ip.Equals(sample._ip))
                {
                    PingSamples.Add(new PingSample(sample));
                }
            };

            _agentCommunicator.PortAgent.SampleArrived += delegate (Sample sample)
            {
                if (_ip.Equals(sample._ip))
                {

                }
                    //CPUSamples.Add(portSample);

                    //if (cpuSample._value > MaxCPU)
                    //{
                    //    MaxCPU = cpuSample._value;
                    //}
                };
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} - {1}", _ip.ToString(), _stationName);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 

        #endregion
    }
}
