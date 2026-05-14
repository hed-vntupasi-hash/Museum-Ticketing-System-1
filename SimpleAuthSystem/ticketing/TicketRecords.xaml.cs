using System.Windows;
using System.Windows.Controls;

namespace SimpleAuthSystem.Pages
{
    /// <summary>
    /// Interaction logic for TicketRecordsPage.xaml
    /// </summary>
    public partial class TicketRecordsPage : Page
    {
        public TicketRecordsPage()
        {
            InitializeComponent();
            PopulateTable();
        }
        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            PopulateTable();
        }

        private void PopulateTable()
        {
            // Example tuple result
            Tuple<int[], string[], string[], decimal[], DateTime[], string[], string[]> ticketData = DatabaseManager.GetTicketRecords();

            // Clear existing rows
            dgTicketRecords.Items.Clear();

            // Get arrays from tuple
            int[] ids = ticketData.Item1;
            string[] types = ticketData.Item2;
            string[] events = ticketData.Item3;
            decimal[] prices = ticketData.Item4;
            DateTime[] creationDates = ticketData.Item5;
            string[] qrs = ticketData.Item6;
            string[] descriptions = ticketData.Item7;

            // Populate DataGrid
            for (int i = 0; i < ids.Length; i++)
            {
                dgTicketRecords.Items.Add(new
                {
                    ID = ids[i],
                    Type = types[i],
                    Event = events[i],
                    Price = prices[i],
                    CreationDate = creationDates[i].ToString("yyyy-MM-dd HH:mm:ss"),
                    QR = qrs[i],
                    Description = descriptions[i]
                });
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null &&
                NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}