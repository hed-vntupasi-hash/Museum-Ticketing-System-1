using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace SimpleAuthSystem
{
    /// <summary>
    /// Interaction logic for TicketListingPage.xaml
    /// </summary>
    public partial class TicketListingPage : Page
    {
        public TicketListingPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                DataTable dt = DatabaseManager.GetTickets();
                dgTickets.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            LoadData();

            MessageBox.Show("Data refreshed successfully!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
    }
}