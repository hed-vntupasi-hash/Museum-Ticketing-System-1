using System;
using System.Collections.Generic;
using System.Data;
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


namespace SimpleAuthSystem
{
    /// <summary>
    /// Interaction logic for TicketListing.xaml
    /// </summary>
    public partial class TicketListing : Window
    {
        public TicketListing()
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
            // Simply close this window to return to the previous one
            this.Close();
        }
        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            // Simply call the existing method that fetches data from MySQL
            LoadData();

            // Optional: Visual feedback to the user
            MessageBox.Show("Data refreshed successfully!", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
