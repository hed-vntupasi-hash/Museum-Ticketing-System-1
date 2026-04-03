using System.Collections.Generic;
using System.Windows;

namespace SimpleAuthSystem
{
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();

            //List<EventData> events = new List<EventData>()
            //{
            //    new EventData { EventName="Music Festival", TicketsSold=320, TotalProfit=6400 },
            //    new EventData { EventName="Tech Conference", TicketsSold=210, TotalProfit=10500 },
            //    new EventData { EventName="Food Expo", TicketsSold=150, TotalProfit=3000 },
            //    new EventData { EventName="Art Exhibit", TicketsSold=95, TotalProfit=1900 }
            //};

            //EventTable.ItemsSource = events;
            UpdateTable();
        }

        public void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateTable();
        }

        private void UpdateTable()
        {
            List<EventData> events = new List<EventData>();
            Tuple<string[], int[], decimal[]> data = DatabaseManager.GetTicketsByEvent();

            for (int i = 0; i < data.Item1.Length; i++)
            {
                events.Add(new EventData
                {
                    EventName = data.Item1[i],
                    TicketsSold = data.Item2[i],
                    TotalProfit = data.Item3[i]
                });
            }
            EventTable.ItemsSource = events;
        }
    }
    // Merged class inside the same file
    public class EventData
    {
        public string EventName { get; set; }
        public int TicketsSold { get; set; }
        public decimal TotalProfit { get; set; }
    }
}