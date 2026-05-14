using System.Windows;
using System.Windows.Controls;
using SimpleAuthSystem.QR;
using SimpleAuthSystem.ticketing;

namespace SimpleAuthSystem
{
    public partial class Menulist : Page
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
            TicketPurchaser window = new TicketPurchaser();
            window.Show();
        }
    }
}