using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using SimpleAuthSystem.QR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace SimpleAuthSystem.ticketing
{
    /// <summary>
    /// Interaction logic for TicketPurchaser.xaml
    /// </summary>
    public partial class TicketPurchaser : Window
    {
        private HubConnection _connection;

        private string testString = "";

        private readonly TicketPurchaseBinding _vm = new TicketPurchaseBinding();

        public TicketPurchaser()
        {
            InitializeComponent();
            InitializeConnection();
            DataContext = _vm;
        }

        private int[] ticketIds;
        private string[] ticketTypes;
        private decimal[] ticketPrices;

        private int[] eventIds;
        private string[] events;
        private DateOnly[] startDates;
        private DateOnly[] endDates;


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

        class TicketPurchaseRequest
        {
            public Type[] types { get; set; }
            public Event[] events { get; set; }
        }

        private void InitializeConnection()
        {
            // 1. Setup Connection (Use Server IP if on different machines)
            _connection = new HubConnectionBuilder()
                .WithUrl("http://192.168.1.50:5000/requestHub")
                .WithAutomaticReconnect()
                .Build();

            _connection.On<string>("TicketPurchaseInfoResponse", (text) =>
            {
                Dispatcher.Invoke(() =>
                {
                    TicketPurchaseRequest ticketPurchase = JsonSerializer.Deserialize<TicketPurchaseRequest>
                    (
                        text,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    ticketIds = ticketPurchase.types.Select(s => s.id).ToArray();
                    ticketTypes = ticketPurchase.types.Select(s => s.name).ToArray();
                    ticketPrices = ticketPurchase.types.Select(s => s.price).ToArray();

                    // for unit testing

                        List<int> tmpId = ticketIds.ToList();
                        List<string> tmpTypes = ticketTypes.ToList();
                        List<decimal> tmpPrices = ticketPrices.ToList();

                        tmpId.Insert(0, -1);
                        tmpTypes.Insert(0, "-");
                        tmpPrices.Insert(0, 12);

                        ticketIds = tmpId.ToArray();
                        ticketTypes = tmpTypes.ToArray();
                        ticketPrices = tmpPrices.ToArray();

                    // end


                    TicketTypeComboBox.ItemsSource = ticketTypes;
                    //MessageBox.Show("Types Count: " + ticketTypes.Length);

                    if (TicketTypeComboBox.Items.Count > 0)
                        TicketTypeComboBox.SelectedIndex = 0;

                    eventIds = ticketPurchase.events.Select(s => s.id).ToArray();
                    events = ticketPurchase.events.Select(s => s.name).ToArray();
                    startDates = ticketPurchase.events.Select(s => s.start_date).ToArray();
                    endDates = ticketPurchase.events.Select(s => s.end_date).ToArray();


                    // for unit testing
                    tmpId = eventIds.ToList();
                    tmpId.Insert(0, -1);
                    eventIds = tmpId.ToArray();

                    List<string> tmpEvents = events.ToList();
                    tmpEvents.Insert(0, "-");
                    events = tmpEvents.ToArray();

                    List<DateOnly> tmpDates = startDates.ToList();
                    tmpDates.Insert(0, startDates[0]);
                    startDates = tmpDates.ToArray();

                    tmpDates = endDates.ToList();
                    tmpDates.Insert(0, endDates[0]);
                    endDates = tmpDates.ToArray();
                    // end


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

            _connection.On<string, string>("ReceiveAlert", (msg) =>
            {
                Dispatcher.Invoke(() => {
                    MessageBox.Show(msg);
                    //LblServerPulse.Text = $"Received: {msg}";
                });

                return $"WPF Client acknowledged at {DateTime.Now.ToShortTimeString()}";
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
            try
            {

                if (_connection.State == HubConnectionState.Connected)
                {
                    string qrCode = await _connection.InvokeAsync<string>
                    (
                        
                        "PurchaseTicket",
                        ticketIds[TicketTypeComboBox.SelectedIndex],
                        eventIds[EventComboBox.SelectedIndex]
                    );
                    _vm.GenerateNew(qrCode, EventComboBox.Text, TicketTypeComboBox.Text);

                    if (qrCode[0] == '-')
                    {
                        MessageBox.Show("Purchase completed!\n    Event: " + EventComboBox.Text + "\n    Ticket type: " + TicketTypeComboBox.Text);
                    }
                    else
                    {
                        MessageBox.Show(qrCode, "Error");
                    }
                }
            }

            
            catch (InvalidOperationException ex) 
            {
                await Task.Delay(3000);
                MessageBox.Show("Connection loat. Please reconnect.\n\n" + ex .Message, "Error");
                
            }
            catch (Exception ex)
            {
                await Task.Delay(3000);
                MessageBox.Show("Unexpected error:\n\n" + ex.Message, "Error");
            }
            _connection.Closed += async (error) =>
            {
                MessageBox.Show("Disconnected from server.");
            };
              
            
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}