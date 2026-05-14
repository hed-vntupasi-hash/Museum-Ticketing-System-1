using SimpleAuthSystem;
using SimpleAuthSystem.Pages;
using System.Windows;

namespace SimpleAuthSystem
{
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();

            // Load Dashboard Home by default
            MainFrame.Navigate(new DashboardHomePage());
        }

        private void TicketListing_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TicketListingPage());
        }


        private void TicketRecords_Click(object sender, RoutedEventArgs e)
        {

            MainFrame.Navigate(new TicketRecordsPage());
        }

        private void EventsScheduler_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EventsSchedulerPage());

        }

        private void ClientReceiver_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientReceiverPage());
        }
    }
}