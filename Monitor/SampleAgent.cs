using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;
using Common;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Monitor
{
    public class SampleAgent
    {
        public Action<Sample> SampleArrived;
        private const int REMOT_AGENT_PORT = 8888;
        private ConcurrentDictionary<IPEndPoint, byte[]> _registeredIPs;
        private Timer _timer;
        private UdpClient _udpClient;
        private BinaryFormatter _formatter;
        private BlockingCollection<Tuple<IPEndPoint, byte[]>> _sampleResponseCollection;
        private string _name;

        public SampleAgent(double interval, int port, string name)
        {
            _name = name;
            _timer = new Timer(interval);
            _registeredIPs = new ConcurrentDictionary<IPEndPoint, byte[]>();
            _timer.Elapsed += (sender, e) => sendSampleRequest();
            _udpClient = new UdpClient(port);
            _timer.Start();
            _sampleResponseCollection = new BlockingCollection<Tuple<IPEndPoint, byte[]>>();
            _formatter = new BinaryFormatter();
            Task.Factory.StartNew(StartListen);
            Task.Factory.StartNew(AnalyzeResponse);
        }

        private void AnalyzeResponse()
        {
            foreach (Tuple<IPEndPoint, byte[]> requestPair in _sampleResponseCollection.GetConsumingEnumerable())
            {
                try
                {
                    IPEndPoint endPoint = requestPair.Item1;
                    byte[] receiveBytes = requestPair.Item2;

                    SampleResponse response;

                    using (var memoryStream = new MemoryStream(receiveBytes))
                    {
                        memoryStream.Position = 0;
                        response = (SampleResponse)_formatter.Deserialize(memoryStream);
                    }

                    Logger.Debug(string.Format("{0} deserilized response with result {1}", _name, response._result));

                    if (SampleArrived != null)
                    {
                        Sample sample = new Sample(response._result, endPoint.Address);
                        SampleArrived(sample);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                }
            }
        }

        private void StartListen()
        {
            Logger.Info("Start Listening to responses from agents...");

            while (true)
            {
                try
                {
                    IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receiveBytes = _udpClient.Receive(ref remoteIpEndPoint);

                    Logger.Info(string.Format("Recieved response from {0}", remoteIpEndPoint));

                    _sampleResponseCollection.Add(new Tuple<IPEndPoint, byte[]>(remoteIpEndPoint, receiveBytes));
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                }
            }
        }

        private void sendSampleRequest()
        {
            _timer.Stop();

            byte[] sendBytes;

            foreach (IPEndPoint endpoint in _registeredIPs.Keys)
            {
                _registeredIPs.TryGetValue(endpoint, out sendBytes);

                try
                {
                    _udpClient.Send(sendBytes, sendBytes.Length, endpoint);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message + e.Source);
                }
            }

            _timer.Start();
        }
        
        public void StartSample(IPAddress ip, SampleRequest request)
        {
            _registeredIPs.TryAdd(new IPEndPoint(ip, REMOT_AGENT_PORT), request.ToByteArray());
        }

        public void StopSample(IPAddress ip)
        {
            byte[] temp;
            _registeredIPs.TryRemove(new IPEndPoint(ip, REMOT_AGENT_PORT), out temp);
        }
    }
}
