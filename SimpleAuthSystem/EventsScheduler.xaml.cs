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
using System;
using System.Windows;

namespace SimpleAuthSystem
{
    public partial class EventsScheduler : Window
    {
        private int[] event_Ids;
        private string[] event_Names;
        private DateOnly[] eventStartDates;
        private DateOnly[] eventEndDates;

        public EventsScheduler()
        {
            InitializeComponent();
            LoadEvents();
        }

        private void LoadEvents()
        {
            Tuple<int[], string[], DateOnly[], DateOnly[]> events = DatabaseManager.GetEvents();
            event_Ids = events.Item1;
            event_Names = events.Item2;
            eventStartDates = events.Item3;
            eventEndDates = events.Item4;

            cmbEventList.ItemsSource = event_Names;

            if (cmbEventList.Items.Count > 0)
            {
                cmbEventList.SelectedIndex = 0;
            }
        }

        private void SaveEvent_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEventName.Text))
            {
                MessageBox.Show("Please enter an event name.");
                return;
            }

            try
            {
                DatabaseManager.CreateEvent(txtEventName.Text, txtDescription.Text, (DateTime)dpStartDate.SelectedDate, (DateTime)dpEndDate.SelectedDate);

                MessageBox.Show(
                    $"Event: {txtEventName.Text}\nStart: {dpStartDate.SelectedDate}\nEnd: {dpEndDate.SelectedDate}\nDescription: {txtDescription.Text}",
                    "Event Saved"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
