using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (StationsList.SelectedItem == null)
            {
                MessageBox.Show(this, "נא לבחור עמדה מרשימת העמדות המנוטרות", "אזהרה", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK,MessageBoxOptions.RtlReading);
                ((CheckBox)sender).IsChecked = false;
            }
        }

        private void PingCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox_Checked(sender, e);

            IPAddress ip;

            MonitoredStation station = StationsList.SelectedItem as MonitoredStation;
            if (station != null && !IPAddress.TryParse(station.IPToPing, out ip))
            {
                MessageBox.Show(this, "כתובת האייפי הוכנסה בפורמט לא תקין, יש להכניס בפורמט הנכון, לדוגמא - 127.0.0.1.", "קלט לא חוקי", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                ((CheckBox)sender).IsChecked = false;
            }
        }
    }
}
