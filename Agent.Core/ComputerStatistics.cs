using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using Common;
using System.Timers;

namespace Agent.Core
{
    public static class ComputerStatistics
    {
        private static PerformanceCounter _cpuCounter;
        private static PerformanceCounter _ramCounter;
        private static float _cpuSample = 0;
        private static float _memorySample = 0;
        private static object _locker = new object();
        static ComputerStatistics()
        {
            Timer timer = new Timer();
            timer.Interval = 5000;
            timer.Elapsed += updateSamples;
            timer.Start();

            _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
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

        public static float GetSample(SampleRequest sampleRequest)
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
                        return GetPingSample(sampleRequest._ip);
                    }
                //case (SamplesEnum.PORT):
                //    {
                //        return GetCpuSample();
                //    }
                default:
                    {
                        return -999;
                    }
            }
        }
    }
}

 