using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using Common;
using System.Collections;
using System.Collections.Generic;

namespace Monitor
{
    /// <summary>
    /// 
    /// </summary>
    public class MonitoredStation : INotifyPropertyChanged
    {
        #region Fields

        private AgentCommunicator _agentCommunicator;
        private long _bytesSent;
        private long _bytesRecieved;
        private IList<string> _networkAdapters;
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
        private SampleRequest _ipSampleRequest;
        private SampleRequest _adapterNamesSampleRequest;
        private SampleRequest _adapterStatisticsSampleRequest;

        #endregion

        #region Properties

        public FixedSizedQueue<Sample> CPUSamples { get; private set; }
        public FixedSizedQueue<Sample> MemorySamples { get; private set; }
        public FixedSizedQueue<Sample> PingSamples { get; private set; }

        private string _selectedAdapter;
        public string SelectedAdapter
        {
            get
            {
                return _selectedAdapter;
            }
            set
            {
                _selectedAdapter = value;
            }
        }

        public long BytesSent
        {
            get
            {
                return _bytesSent;
            }
            set
            {
                _bytesSent = value;
                NotifyPropertyChanged();
            }
        }

        public long BytesRecieved
        {
            get
            {
                return _bytesRecieved;
            }
            set
            {
                _bytesRecieved = value;
                NotifyPropertyChanged();
            }
        }

        public IList<string> NetworkAdapters
        {
            get
            {
                return _networkAdapters;
            }
            set
            {
                _networkAdapters = value;
                NotifyPropertyChanged();
            }
        }

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
                        _ipSampleRequest._parameter = _ipToPing;
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

                if (_isPortTransportMonitored)
                {
                    if (!string.IsNullOrEmpty(SelectedAdapter))
                    {
                        _adapterStatisticsSampleRequest._parameter = SelectedAdapter;
                        _agentCommunicator.AdapterStatisticAgent.StartSample(_ip, _adapterStatisticsSampleRequest);
                    }
                }
                else
                {
                    _agentCommunicator.AdapterStatisticAgent.StopSample(_ip);
                    BytesSent = 0;
                    BytesRecieved = 0;
                }
            }
        }
        
        public string IPToPing { get; set; }

        #endregion

        #region CTOR

        public MonitoredStation(string stationName, IPAddress ip)
        {
            _cpuSampleRequest = new SampleRequest(SamplesEnum.CPU);
            _memorySampleRequest = new SampleRequest(SamplesEnum.MEMORY);
            _adapterNamesSampleRequest = new SampleRequest(SamplesEnum.AdapterNames);
            _adapterStatisticsSampleRequest = new SampleRequest(SamplesEnum.AdapterStatistics);
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
            _agentCommunicator.AdapterStatisticAgent.StartSample(_ip, _adapterNamesSampleRequest);

            _agentCommunicator.MemoryAgent.SampleArrived += delegate (Sample sample)
            {
                if (_ip.Equals(sample._ip))
                {
                    MemorySamples.Add(new MemorySample(sample));

                    if ((int)(float)sample._value > MaxMemory)
                    {
                        MaxMemory = (int)(float)sample._value;
                    }
                }
            };

            _agentCommunicator.CPUAgent.SampleArrived += delegate (Sample sample)
            {
                if (_ip.Equals(sample._ip))
                {
                    CPUSamples.Add(new CPUSample(sample));

                    if ((int)(float)sample._value > MaxCPU)
                    {
                        MaxCPU = (int)(float)sample._value;
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

            _agentCommunicator.AdapterStatisticAgent.SampleArrived += delegate (Sample sample)
            {
                if (_ip.Equals(sample._ip))
                {
                    var adaptersNames = sample._value as IList<string>;

                    if (adaptersNames != null)
                    {
                        NetworkAdapters = adaptersNames;
                        _agentCommunicator.AdapterStatisticAgent.StopSample(_ip);
                    }
                    else
                    {
                        var statistics = (AdapterStatistics)sample._value;
                        BytesRecieved = statistics._bytesRecieved;
                        BytesSent = statistics._bytesSent;
                    }
                }
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
