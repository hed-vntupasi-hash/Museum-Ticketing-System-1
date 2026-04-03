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
using System.Windows.Shapes;
using SimpleAuthSystem.QR;

namespace SimpleAuthSystem
{
    /// <summary>
    /// Interaction logic for Menulist.xaml
    /// </summary>
    public partial class Menulist : Window
    {
        public Menulist()
        {
            InitializeComponent();
        }

        private void QrReader_Click(object sender, RoutedEventArgs e)
        {
            QrReaderWindow window = new QrReaderWindow();
            window.Show();
        }

        private void PurchaseTicket_Click(object sender, RoutedEventArgs e)
        {
            PurchaseTicketWindow window = new PurchaseTicketWindow();
            window.Show();
        }

        private void QRWriter_Click(object sender, RoutedEventArgs e)
        {
            QrWriterWindow window = new QrWriterWindow();
            window.Show();
        }

        private void TicketListing_Click(object sender, RoutedEventArgs e)
        {
            TicketListing window = new TicketListing();
            window.Show();

        }

        private void TicketRecords_Click(object sender, RoutedEventArgs e)
        {
            TicketRecords window = new TicketRecords();
            window.Show();
        }


        private void EventsScheduler_Click(object sender, RoutedEventArgs e)
        {
            EventsScheduler window = new EventsScheduler();
            window.Show();
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            Dashboard window = new Dashboard();
            window.Show();
        }


    }
}
