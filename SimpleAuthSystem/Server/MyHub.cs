using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using SimpleAuthSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace SimpleAuthSystem
{
    public class MyHub : Hub
    {
        private CodeGenerator _generator;
        private PersistentCounter _fileCounter;
        private long _currentId;
        private string _generatedCode;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public long CurrentId
        {
            get => _currentId;
            set { _currentId = value; OnPropertyChanged(nameof(CurrentId)); }
        }
        public string GeneratedCode
        {
            get => _generatedCode;
            set { _generatedCode = value; OnPropertyChanged(nameof(GeneratedCode)); }
        }

        public MyHub()
        {
            InitializePersistence();
        }

        private void InitializePersistence()
        {
            var counterFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "counter.txt");
            _generator = new CodeGenerator(counterFile);

            // Separate counter for saving images
            var fileCounterFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "file_counter.txt");
            _fileCounter = new PersistentCounter(fileCounterFile);
        }

        public string GenerateNew(string TitleText, string ContentText)
        {
            long id = _generator.GetNextId();

            CurrentId = id;
            GeneratedCode = _generator.GenerateCode(id);
            return GeneratedCode;
        }

        public string PurchaseTicket(int typeId, int eventId)
        {
            string qr = "";
            string message = "";

            qr = qrGenerator.GenerateNew("Title", "Content");
            //MessageBox.Show(qr, "Sent QR Code");
            message = DatabaseManager.PurchaseTicket(typeId, eventId, qr);


            switch (message)
            {
                case "Success":
                    return "-" + qr;
                case "NonExistentTicketTypeId":
                    return "Invalid Ticket Type.";
                case "NonExistentTicketEventId":
                    return "Invalid Ticket Event.";
                default:
                    return "Purchased failed";
            }
            if (message == "Success")
            {
                //MessageBox.Show(message, "Purchase Message");
                //qr = qrGenerator.GenerateNew("Title", "Content");
            } else
             return message;



            //if (DatabaseManager.PurchaseTicket(typeId, eventId, qr) == true)
            //{
            //    // Todo: fix Title & Content
            //    //qr = qrGenerator.GenerateNew("Title", "Content");

            //    //DatabaseManager.SetTicketQrCode(qr);
            //}
            //return qr;
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


        public async Task SendTicketPurchaseInfo(string incomingString)
        {
            ServerEvents.RaiseMessageReceived(incomingString);
            await Clients.Caller.SendAsync("TicketPurchaseInfoResponse", "json");
        }

        class TicketPurchaseRequest
        {
            public int TicketTypeId { get; set; }
            public int EventId { get; set; }
        }
        private readonly TicketQrGenerator qrGenerator = new TicketQrGenerator();
        public async Task FetchDataTicketPurchaseInfo()
        {
            // Log it on the server UI
            ServerEvents.RaiseMessageReceived("Client requested JSON data (no input provided).");

            // Create a simple object to send as JSON

            Tuple<int[], string[], decimal[]> ticketTypes = DatabaseManager.GetTicketTypes();
            Tuple<int[], string[], DateOnly[], DateOnly[]> eventTuple = DatabaseManager.GetEvents();

            List<Type> types = new List<Type>();
            List<Event> events = new List<Event>();
            TicketPurchase ticketPurchase = null;

            int i;
            for (i = 0; i < ticketTypes.Item1.Length; i++)
            {
                types.Add(new Type
                {
                    id = ticketTypes.Item1[i],
                    name = ticketTypes.Item2[i],
                    price = ticketTypes.Item3[i]
                });
            }

            for (i = 0; i < eventTuple.Item1.Length; i++)
            {
                events.Add(new Event
                {
                    id = eventTuple.Item1[i],
                    name = eventTuple.Item2[i],
                    start_date = eventTuple.Item3[i],
                    end_date = eventTuple.Item4[i]
                });
            }

            ticketPurchase = new TicketPurchase
            {
                types = types.ToArray(),
                events = events.ToArray(),
            };

            string json = JsonSerializer.Serialize
            (
                ticketPurchase,
                new JsonSerializerOptions { WriteIndented = true }
            );

            // Note: SignalR automatically converts C# objects to JSON strings!
            await Clients.Caller.SendAsync("TicketPurchaseInfoResponse", json);
        }

        public async Task ConfirmTicketPurchase()
        {
            // 1. Notify the Server UI that a message arrived
            ServerEvents.RaiseMessageReceived("Purchase successful.");

            await Clients.Caller.SendAsync("Purchase successfullly sent");
            //Console.WriteLine($"Received Ticket Purchase: {ticketTypeId} and {eventId}");
        }

        public async Task SendRequest(string incomingString)
        {
            // 1. Notify the Server UI that a message arrived
            ServerEvents.RaiseMessageReceived(incomingString);

            // 2. Send back a string and a boolean as the response

            Tuple<bool, string> ticketStatus = DatabaseManager.TapTicket(incomingString);

            bool successStatus = ticketStatus.Item1;
            string responseText = ticketStatus.Item2;

            await Clients.Caller.SendAsync("ReceiveResponse", responseText, successStatus);
        }

        // Inside MyHub.cs
        public async Task FetchData()
        {
            // Log it on the server UI
            ServerEvents.RaiseMessageReceived("Client requested JSON data (no input provided).");

            // Create a simple object to send as JSON
            var dataObject = new
            {
                Time = DateTime.Now.ToString("HH:mm:ss"),
                Message = "Here is your requested data",
                SystemStatus = "Healthy"
            };

            // Note: SignalR automatically converts C# objects to JSON strings!
            await Clients.Caller.SendAsync("ReceiveDataOnly", dataObject);
        }
    }

    // Simple static class to bridge the Hub and the WPF Window
    public static class ServerEvents
    {
        public static event Action<string>? OnMessageReceived;
        public static void RaiseMessageReceived(string msg) => OnMessageReceived?.Invoke(msg);
    }
}
