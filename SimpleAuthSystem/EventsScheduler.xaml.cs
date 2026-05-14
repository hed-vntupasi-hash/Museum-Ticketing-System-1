using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SimpleAuthSystem.Pages
{
    public partial class EventsSchedulerPage : Page
    {
        private int[] event_Ids;
        private string[] event_Names;
        private DateOnly[] eventStartDates;
        private DateOnly[] eventEndDates;

        public EventsSchedulerPage()
        {
            InitializeComponent();
            LoadEvents();
        }

        // MODEL FOR DATAGRID

        public class EventItem
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateOnly StartDate { get; set; }

            public DateOnly EndDate { get; set; }
        }

        private void LoadEvents()
        {
            Tuple<int[], string[], DateOnly[], DateOnly[]> events =
                DatabaseManager.GetEvents();

            event_Ids = events.Item1;
            event_Names = events.Item2;
            eventStartDates = events.Item3;
            eventEndDates = events.Item4;

            // Convert arrays into a list for DataGrid

            List<EventItem> eventList = new List<EventItem>();

            for (int i = 0; i < event_Ids.Length; i++)
            {
                eventList.Add(new EventItem
                {
                    Id = event_Ids[i],
                    Name = event_Names[i],
                    StartDate = eventStartDates[i],
                    EndDate = eventEndDates[i]
                });
            }

            dgEvents.ItemsSource = eventList;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void SaveEvent_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEventName.Text))
            {
                MessageBox.Show("Please enter an event name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(dpStartDate.Text))
            {
                MessageBox.Show("Please enter a start date.");
                return;
            }

            if (string.IsNullOrWhiteSpace(dpEndDate.Text))
            {
                MessageBox.Show("Please enter an end date.");
                return;
            }

            try
            {
                DatabaseManager.CreateEvent(
                    txtEventName.Text,
                    txtDescription.Text,
                    (DateTime)dpStartDate.SelectedDate,
                    (DateTime)dpEndDate.SelectedDate
                );

                MessageBox.Show(
                    $"Event: {txtEventName.Text}\n" +
                    $"Start: {dpStartDate.SelectedDate}\n" +
                    $"End: {dpEndDate.SelectedDate}\n" +
                    $"Description: {txtDescription.Text}",
                    "Event Saved"
                );

                // Refresh table

                LoadEvents();

                // Clear fields

                txtEventName.Clear();
                txtDescription.Clear();

                dpStartDate.SelectedDate = null;
                dpEndDate.SelectedDate = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}