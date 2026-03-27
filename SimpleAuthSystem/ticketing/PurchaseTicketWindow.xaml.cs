using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Org.BouncyCastle.Crypto;

namespace SimpleAuthSystem
{
    /// <summary>
    /// Interaction logic for PurchaseTicketWindow.xaml
    /// </summary>
    public partial class PurchaseTicketWindow : Window
    {
        int[] ids;
        string[] ticketNames;
        decimal[] prices;

        public PurchaseTicketWindow()
        {
            InitializeComponent();

            LoadTicketTypes();
        }

        private void LoadTicketTypes()
        {
            Tuple<int[], string[], decimal[]> ticketTypes = DatabaseManager.GetTicketTypes();
            ids = ticketTypes.Item1;
            ticketNames = ticketTypes.Item2;
            prices = ticketTypes.Item3;

            EventComboBox.ItemsSource = ticketNames;
            prices = ticketTypes.Item3;

            if (EventComboBox.Items.Count > 0)
                EventComboBox.SelectedIndex = 0;
        }
    }
}
