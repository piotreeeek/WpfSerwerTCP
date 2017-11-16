using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfSerwerTCP
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Serwer s = new Serwer();
        internal static MainWindow main;

        public MainWindow()
        {
            main = this;
            InitializeComponent();
            clients.Text = "0";
            BT_Stop.IsEnabled = false;
        }

        private void BT_Start_Click(object sender, RoutedEventArgs e)
        {
            int number;
            Int32.TryParse(port.Text, out number);
            if (s.Start(number))
            {
                messages.Text = "Serwer działa. Aktualny port: " + number.ToString() + ".\n" + messages.Text;
                BT_Start.IsEnabled = false;
                BT_Stop.IsEnabled = true;
            }
            else
            {
                messages.Text = "Polączneie nieudane. Prosze wybrać inny port.\n " + messages.Text;
            }
            

        }

        private void BT_Stop_Click(object sender, RoutedEventArgs e)
        {
            s.Stop();
            messages.Text = "Serwer zakończył działanie \n" + messages.Text; 
            BT_Start.IsEnabled = true;
            BT_Stop.IsEnabled = false;
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void ModifyClientCount(int i)
        {
            Dispatcher.Invoke(new Action(() => 
            {
                Int32.TryParse(clients.Text, out int temp);
                clients.Text = (temp + i).ToString();
            }));
        }
        public void AddStatement(string text)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                messages.Text = text + messages.Text;
            }));
        }
    }
}
