using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Common;

namespace Agent.Core
{
    public class Comunicator
    {
        private const int LISTENING_PORT = 8888;
        private BinaryFormatter _formatter;
        private UdpClient _udpClient;
        private BlockingCollection<Tuple<IPEndPoint, byte[]>> _sampleRequestCollection;

        public Comunicator()
        {
            _sampleRequestCollection = new BlockingCollection<Tuple<IPEndPoint, byte[]>>();
            _formatter = new BinaryFormatter();
            _udpClient = new UdpClient(LISTENING_PORT);
            Task.Factory.StartNew(SendResponse);
        }
        
        private void SendResponse()
        {
            foreach (Tuple<IPEndPoint, byte[]> requestPair in _sampleRequestCollection.GetConsumingEnumerable())
            {
                try
                {
                    IPEndPoint endPoint = requestPair.Item1;
                    byte[] receiveBytes = requestPair.Item2;

                    SampleRequest request;

                    using (var memoryStream = new MemoryStream(receiveBytes))
                    {
                        memoryStream.Position = 0;
                        request = (SampleRequest)_formatter.Deserialize(memoryStream);
                    }

                    Logger.Debug(string.Format("deserilized {0} request", request._sampleType));

                    object sampleResult = ComputerStatistics.GetSample(request);
                    byte[] sendBytes = new SampleResponse(sampleResult).ToByteArray();

                    Logger.Debug(string.Format("Sending response to monitor - {0}, value {1}", endPoint, sampleResult));

                    _udpClient.Send(sendBytes, sendBytes.Length, endPoint);
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                }
            }
        }

        public void StartListen()
        {
            Logger.Info("Start Listening to requests from monitors...");

            while (true)
            {
                try
                {
                    IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receiveBytes = _udpClient.Receive(ref remoteIpEndPoint);

                    Logger.Info(string.Format("Recieved request from {0}", remoteIpEndPoint));

                    _sampleRequestCollection.Add(new Tuple<IPEndPoint, byte[]>(remoteIpEndPoint, receiveBytes));
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                }
            }
        }
    }
}
