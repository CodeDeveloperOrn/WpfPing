using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Data;
using System.Net.NetworkInformation;

namespace WpfPing
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DataTable dataTableIPAdress { get; set; }
        public int timeoutMinutes { get; set; }
        public bool isStopped { get; set; } = false;
        public MainWindow()
        {
            InitializeComponent();
            dataTableIPAdress = new DataTable();
            dataTableIPAdress.Columns.Add(new DataColumn("Adress", typeof(string)));
            dataTableIPAdress.Columns.Add(new DataColumn("Status", typeof(bool)));
            timeoutMinutes = 5;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> text = new List<string>();

            int coiuntLines = tbTextAdress.LineCount; //new TextRange(richTextAdress.Document.ContentStart, richTextAdress.Document.ContentEnd).Text.Split('\n').ToList();
            for (int i = 0; i < coiuntLines; i++)
            {
                text.Add(tbTextAdress.GetLineText(i).Trim());
            }
            dataTableIPAdress.Rows.Clear();

            /* Сортировка адресов */
            text.Sort();

            foreach (string oneLine in text)
            {
                DataRow row = dataTableIPAdress.NewRow();
                row.SetField(0, oneLine);
                row.SetField(1, false);

                dataTableIPAdress.Rows.Add(row);
            }
        }

        private async void startMonitoringIP_Click(object sender, RoutedEventArgs e)
        {
            /* Запуск пинга */
            Ping ping = new Ping();
            PingReply pingReply = null;

            while (!isStopped)
            {
                foreach (DataRow dataRow in dataTableIPAdress.Rows)
                {
                    pingReply = ping.Send(dataRow.ItemArray[0].ToString());
                    Console.WriteLine("IP={0}", dataRow.ItemArray[0].ToString());
                    if (pingReply.Status != IPStatus.TimedOut)
                    {
                        /* Доступ есть */
                        dataRow.SetField(1, true);
                        Console.WriteLine("Да");
                    }
                    else
                    {
                        /* Доступа нет */
                        dataRow.SetField(1, false);
                        Console.WriteLine("Нет");
                    }
                }
                /* ---------------------- */
                await Task.Delay(timeoutMinutes * 60000);
            }
        }

        private void stopMonitorIP_Click(object sender, RoutedEventArgs e)
        {
            isStopped = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGridAdreassIP.DataContext = dataTableIPAdress;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            int tempVar = 0;
            if (Int32.TryParse(((TextBox)sender).Text, out tempVar))
            {
                timeoutMinutes = tempVar;
            }
        }
    }
}
