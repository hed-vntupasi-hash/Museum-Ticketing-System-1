using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32;
using System;
using System.Drawing; // Remember to install System.Drawing.Common
using System.Windows;
using ZXing;
using ZXing.Aztec.Internal;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace SimpleAuthSystem.QR
{
    public partial class QrReaderWindow : Window
    {
        private HubConnection _connection;
        public QrReaderWindow()
        {
            InitializeComponent();
            InitializeConnection();
        }

        private void InitializeConnection()
        {
            // 1. Setup Connection (Use Server IP if on different machines)
            _connection = new HubConnectionBuilder()
                .WithUrl("http://192.168.1.50:5000/requestHub")
                .WithAutomaticReconnect()
                .Build();

            // 2. Define what happens when the server sends a response
            _connection.On<string, bool>("ReceiveResponse", (text, flag) =>
            {
                Dispatcher.Invoke(() =>
                {
                    ResponseLabel.Text = text;
                    StatusLabel.Text = $"Boolean: {flag}";
                });
            });
            StartConnection();
        }
        private async void StartConnection()
        {
            try { await _connection.StartAsync(); }
            catch (Exception ex) { MessageBox.Show($"Connection Error: {ex.Message}"); }
        }

        private async void SendTicketId(string toSend)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync("SendRequest", toSend);
            }
        }

        public static string DecodeQRCode(string filePath)
        {
            // Load bitmap from file
            Bitmap bitmap = (Bitmap)Image.FromFile(filePath);

            // Create a reader that works with System.Drawing.Bitmap
            var reader = new BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = new[] { BarcodeFormat.QR_CODE }
                }
            };

            // Decode the image
            var result = reader.Decode(bitmap);

            return result?.Text; // returns null if no QR found
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Open file dialog to select image
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    SendTicketId(DecodeQRCode(filePath));
                    //Tuple<bool, string> validity = DatabaseManager.TapTicket(DecodeQRCode(filePath));
                    //MessageBox.Show(validity.Item2);

                    //bool ticketDateIsValid = DatabaseManager.TapTicket
                    //(
                    //    DecodeQRCode(filePath)
                    //);

                    //if (validity.Item1 == true)
                    //{
                    //    MessageBox.Show("Ticket Date is valid. Proceed");
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Access Denied.");
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading image: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}