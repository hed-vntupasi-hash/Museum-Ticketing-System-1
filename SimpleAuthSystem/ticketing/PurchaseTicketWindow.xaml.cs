
using Org.BouncyCastle.Crypto;
using SimpleAuthSystem.QR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.SignalR.Client;

namespace SimpleAuthSystem
{
    /// <summary>
    /// Interaction logic for PurchaseTicketWindow.xaml
    /// </summary>
    public partial class PurchaseTicketWindow : Window
    {
        //private readonly QR.MainViewModel qrGenerator = new QR.MainViewModel();
        private HubConnection _connection;

        public PurchaseTicketWindow()
        {
            InitializeComponent();
            InitializeConnection();
        }

        class Type
        {
            public int id { get; set; }
            public string name { get; set; }
            public decimal price { get; set; }
        }

        class Event
        {
            public int id { get; set; }
            public string name { get; set; }
            public DateOnly start_date { get; set; }
            public DateOnly end_date { get; set; }
        }

        class TicketPurchase
        {
            public Type[] types { get; set; }
            public Event[] events { get; set; }
        }
        private int[] ticketIds;
        private string[] ticketTypes;
        private decimal[] ticketPrices;

        private int[] eventIds;
        private string[] events;
        private DateOnly[] startDates;
        private DateOnly[] endDates;


















        private void InitializeConnection()
        {
            // 1. Setup Connection (Use Server IP if on different machines)
            _connection = new HubConnectionBuilder()
                .WithUrl("http://192.168.1.50:5000/requestHub")
                .WithAutomaticReconnect()
                .Build();

            // 2. Define what happens when the server sends a response
            _connection.On<string>("TicketPurchaseInfoResponse", (text) =>
            {
                Dispatcher.Invoke(() =>
                {
                    TicketPurchase ticketPurchase = JsonSerializer.Deserialize<TicketPurchase>
                    (
                        text,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    ticketIds = ticketPurchase.types.Select(s => s.id).ToArray();
                    ticketTypes = ticketPurchase.types.Select(s => s.name).ToArray();
                    ticketPrices = ticketPurchase.types.Select(s => s.price).ToArray();

                    TicketTypeComboBox.ItemsSource = ticketTypes;
                    //MessageBox.Show("Types Count: " + ticketTypes.Length);

                    if (TicketTypeComboBox.Items.Count > 0)
                        TicketTypeComboBox.SelectedIndex = 0;

                    eventIds = ticketPurchase.events.Select(s => s.id).ToArray();
                    events = ticketPurchase.events.Select(s => s.name).ToArray(); ;
                    startDates = ticketPurchase.events.Select(s => s.start_date).ToArray(); ;
                    endDates = ticketPurchase.events.Select(s => s.end_date).ToArray(); ;

                    EventComboBox.ItemsSource = events;

                    if (EventComboBox.Items.Count > 0)
                    {
                        EventComboBox.SelectedIndex = 0;
                        DisplayDates(0);
                    }

                    //ResponseLabel.Text = text;
                });
            });

            _connection.On<object>("ReceiveDataOnly", (data) =>
            {
                Dispatcher.Invoke(() =>
                {
                    // 'data' comes in as a JsonElement/Object
                    //ResponseLabel.Text = "Received Data: " + data.ToString();
                });
            });




            _connection.On<string>("ConfirmTicketPurchase", (Data) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Purchase Confirmation " + Data);
                    // 'data' comes in as a JsonElement/Object
                    //ResponseLabel.Text = "Received Data: " + data.ToString();
                });
            });












            StartConnection();
        }

        private async void StartConnection()
        {
            try
            {
                await _connection.StartAsync();

                if (_connection.State == HubConnectionState.Connected)
                {
                    await _connection.InvokeAsync("FetchDataTicketPurchaseInfo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection Error: {ex.Message}");
            }

        }

        private async void SendTicketPurchaseRequest()
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                int selectedTicketTypeId = ticketIds[TicketTypeComboBox.SelectedIndex];
                int selectedEventId = eventIds[EventComboBox.SelectedIndex];
                var requestData = new
                {
                    TicketTypeId = selectedTicketTypeId,
                    EventId = selectedEventId
                };
                string jsonData = JsonSerializer.Serialize(requestData);
                await _connection.InvokeAsync("PurchaseTicket", jsonData);
            }
        }

        private string ConvertDate(DateOnly date)
        {
            return date.ToString("MMMM d, yyyy");
        }

        private void DisplayDates(int index)
        {
            StartDate.Text = ConvertDate(startDates[index]);
            EndDate.Text = ConvertDate(endDates[index]);
        }

        private void EventComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayDates(EventComboBox.SelectedIndex);
        }

        private void PurchaseTicket_Click(object sender, RoutedEventArgs e)
        {
            SendTicketPurchaseRequest();
            //if (DatabaseManager.PurchaseTicket
            //(
            //    ticketType_Ids[TicketTypeComboBox.SelectedIndex],
            //    event_Ids[EventComboBox.SelectedIndex]
            //) == true)
            //{
            //    DatabaseManager.SetTicketQrCode(qrGenerator.GenerateNew(EventComboBox.Text, TicketTypeComboBox.Text));
            //    MessageBox.Show("Ticket purchased successfully!", "Purchase Successful");
            //}

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}