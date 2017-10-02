namespace Monitor
{
    public class AgentCommunicator
    {
        private static AgentCommunicator _instance;
        private static object _locker = new object();

        public SampleAgent CPUAgent { get; private set; }
        public SampleAgent MemoryAgent { get; private set; }
        public SampleAgent PingAgent { get; private set; }
        public SampleAgent PortAgent { get; private set; }

        public static AgentCommunicator Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new AgentCommunicator();
                        }
                    }
                }

                return _instance;
            }
        }

        private AgentCommunicator()
        {
            CPUAgent    = new SampleAgent(5000, 5555, "CPUAgent");
            MemoryAgent = new SampleAgent(5500, 6666, "MemoryAgent");
            PingAgent   = new SampleAgent(10000, 7777, "PingAgent");
            PortAgent   = new SampleAgent(4000, 4444, "PortAgent");
        }                                        
    }
}
