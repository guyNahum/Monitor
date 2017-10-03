using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using Common;
using System.Timers;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Agent.Core
{
    public static class ComputerStatistics
    {
        private static PerformanceCounter _cpuCounter;
        private static PerformanceCounter _ramCounter;
        private static float _cpuSample = 0;
        private static float _memorySample = 0;
        private static object _locker = new object();
        private static NetworkInterface[] _interfaces;

        static ComputerStatistics()
        {
            Timer timer = new Timer();
            timer.Interval = 5000;
            timer.Elapsed += updateSamples;
            timer.Start();

            _interfaces = NetworkInterface.GetAllNetworkInterfaces();
            _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        public static IList<string> GetNetworkAdaptersNames()
        {
            return _interfaces.Select(GetName).ToList();
        }

        public static AdapterStatistics GetNetworkAdapterStatistics(string adapterName)
        {
            NetworkInterface adapter = _interfaces.FirstOrDefault(a => a.Name.Equals(adapterName));
            //NetworkInterface adapter = _interfaces.FirstOrDefault();

            if (adapter == null)
            {
                Logger.Error(string.Format("Recieved unknows network adapter name - {0}", adapterName));
                return new AdapterStatistics();
            }
            else
            {
                IPv4InterfaceStatistics statistics = adapter.GetIPv4Statistics();
                return new AdapterStatistics(statistics.BytesSent, statistics.BytesReceived);
            }
        }

        private static string GetName(NetworkInterface networkInterface)
        {
            return networkInterface.Name;
        }

        private static void updateSamples(object sender, ElapsedEventArgs e)
        {
            lock (_locker)
            {
                _cpuSample = _cpuCounter.NextValue();
                _memorySample = _ramCounter.NextValue();
            }
        }

        public static float GetCpuSample()
        {
            lock (_locker)
            {
                return _cpuSample;
            }
        }

        public static float GetMemorySample()
        {
            lock (_locker)
            {
                return _memorySample;
            }
        }

        public static float GetPingSample(IPAddress ip)
        {
            PingReply pingReply = new Ping().Send(ip);

            return pingReply.Status == IPStatus.Success ? pingReply.RoundtripTime : -1;
        }

        public static object GetSample(SampleRequest sampleRequest)
        {
            switch (sampleRequest._sampleType)
            {
                case (SamplesEnum.CPU):
                    {
                        return GetCpuSample();
                    }
                case (SamplesEnum.MEMORY):
                    {
                        return GetMemorySample();
                    }
                case (SamplesEnum.PING):
                    {
                        return GetPingSample((IPAddress)sampleRequest._parameter);
                    }
                case (SamplesEnum.AdapterNames):
                    {
                        return GetNetworkAdaptersNames();
                    }
                case (SamplesEnum.AdapterStatistics):
                    {
                        return GetNetworkAdapterStatistics(sampleRequest._parameter as string);
                    }
                default:
                    {
                        return -999;
                    }
            }
        }
    }
}

 