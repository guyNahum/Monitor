using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Configuration;
using System.Collections;

namespace Monitor
{
    public class MainWindowViewModel
    {
        #region Fields

        private IList<MonitoredStation> _monitoredStations;

        #endregion

        #region Properties

        public IList<MonitoredStation> MonitoredStations
        {
            get
            {
                return _monitoredStations;
            }
        }

        #endregion

        #region Ctor

        public MainWindowViewModel()
        {
            _monitoredStations = new List<MonitoredStation>();
            LoadMonitoredStations();
        }

        #endregion

        #region Methods

        private void LoadMonitoredStations()
        {

            var monitoredStationsHashTable = (ConfigurationManager.GetSection("DeviceSettings/MonitoredStations") as System.Collections.Hashtable)
                 .Cast<DictionaryEntry>();
            //.ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());

            foreach (DictionaryEntry station in monitoredStationsHashTable)
            {
                string[] splitedIp = station.Value.ToString().Split('.');
                byte num1 = byte.Parse(splitedIp[0]);
                byte num2 = byte.Parse(splitedIp[1]);
                byte num3 = byte.Parse(splitedIp[2]);
                byte num4 = byte.Parse(splitedIp[3]);
                byte[] ip = new byte[4] { num1, num2, num3, num4 };

                _monitoredStations.Add(new MonitoredStation(station.Key.ToString(), new IPAddress(ip)));
            }
        }

        #endregion
    }
}
