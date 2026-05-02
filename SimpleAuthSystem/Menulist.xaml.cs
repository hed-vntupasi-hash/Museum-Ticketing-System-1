using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SimpleAuthSystem.QR;
using SimpleAuthSystem.ticketing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            TicketPurchaser window = new TicketPurchaser();
            window.Show();
        }

    }
}
 