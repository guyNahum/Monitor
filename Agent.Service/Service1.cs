using Agent.Core;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Agent.Service
{
    public partial class Service1 : ServiceBase
    {
        private Comunicator _comunicator;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _comunicator = new Comunicator();

            Task.Factory.StartNew(_comunicator.StartListen);
        }

        protected override void OnStop()
        {
            _comunicator.Close();
        }
    }
}
