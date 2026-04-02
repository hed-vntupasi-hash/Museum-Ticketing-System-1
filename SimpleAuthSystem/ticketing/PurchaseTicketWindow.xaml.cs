using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
using Org.BouncyCastle.Crypto;
using SimpleAuthSystem.QR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimpleAuthSystem
{
    /// <summary>
    /// Interaction logic for PurchaseTicketWindow.xaml
    /// </summary>
    public partial class PurchaseTicketWindow : Window
    {
        private readonly QR.MainViewModel qrGenerator = new QR.MainViewModel();

        private int[] ticketType_Ids;
        private string[] ticketType_Names;
        private decimal[] prices;

        private int[] event_Ids;
        private string[] event_Names;
        private DateOnly[] eventStartDates;
        private DateOnly[] eventEndDates;

        public PurchaseTicketWindow()
        {
            InitializeComponent();

            LoadTicketTypes();
            LoadEvents();
        }

        private void LoadTicketTypes()
        {
            Tuple<int[], string[], decimal[]> ticketTypes = DatabaseManager.GetTicketTypes();
            ticketType_Ids = ticketTypes.Item1;
            ticketType_Names = ticketTypes.Item2;
            prices = ticketTypes.Item3;

            TicketTypeComboBox.ItemsSource = ticketType_Names;

            if (TicketTypeComboBox.Items.Count > 0)
                TicketTypeComboBox.SelectedIndex = 0;
        }

        private void LoadEvents()
        {
            Tuple<int[], string[], DateOnly[], DateOnly[]> events = DatabaseManager.GetEvents();
            event_Ids = events.Item1;
            event_Names = events.Item2;
            eventStartDates = events.Item3;
            eventEndDates = events.Item4;

            EventComboBox.ItemsSource = event_Names;

            if (EventComboBox.Items.Count > 0)
            {
                EventComboBox.SelectedIndex = 0;
                DisplayDates(0);
            }
        }

        private string ConvertDate(DateOnly date)
        {
            return date.ToString("MMMM d, yyyy");
        }

        private void DisplayDates(int index)
        {
            StartDate.Text = ConvertDate(eventStartDates[index]);
            EndDate.Text = ConvertDate(eventEndDates[index]);
        }

        private void EventComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayDates(EventComboBox.SelectedIndex);
        }

        private void PurchaseTicket_Click(object sender, RoutedEventArgs e)
        {
            if (DatabaseManager.PurchaseTicket
            (
                ticketType_Ids[TicketTypeComboBox.SelectedIndex],
                event_Ids[EventComboBox.SelectedIndex]
            ) == true)
            {
                DatabaseManager.SetTicketQrCode(qrGenerator.GenerateNew(EventComboBox.Text, TicketTypeComboBox.Text));
                MessageBox.Show("Ticket purchased successfully!", "Purchase Successful");
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
