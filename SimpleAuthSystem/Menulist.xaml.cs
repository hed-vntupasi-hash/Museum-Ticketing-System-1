using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SimpleAuthSystem.QR;
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
            StartBackgroundServer();
        }

        private void StartBackgroundServer()
        {
            // Listen for Hub events to update UI
            //ServerEvents.OnMessageReceived += (msg) =>
            //{
            //    Dispatcher.Invoke(() => RequestList.Items.Add($"{DateTime.Now:HH:mm:ss}: {msg}"));
            //};

            Task.Run(() =>
            {
                var builder = WebApplication.CreateBuilder();
                builder.Services.AddSignalR();

                // Listen on Port 5000 on ALL local network adapters
                builder.WebHost.UseUrls("http://0.0.0.0:5000");


                var app = builder.Build();
                app.MapHub<MyHub>("/requestHub");

                //Dispatcher.Invoke(() => StatusLabel.Text = "Status: Server Running on Port 5000");
                app.Run();
            });
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

        private void ClientReceiver_Click(object sender, RoutedEventArgs e)
        {
            ClientReceiver window = new ClientReceiver();
            window.Show();
        }
        
    }
}