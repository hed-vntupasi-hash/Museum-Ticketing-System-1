using System;
using System.Windows;
using Microsoft.Win32;
using System.Drawing; // Remember to install System.Drawing.Common
using ZXing;
using ZXing.Windows.Compatibility;
using ZXing.Common;

namespace SimpleAuthSystem.QR
{
    public partial class QrReaderWindow : Window
    {
        public QrReaderWindow()
        {
            InitializeComponent();
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
                    bool ticketDateIsValid = DatabaseManager.TapTicket
                    (
                        DecodeQRCode(filePath)
                    );

                    if (ticketDateIsValid == true)
                    {
                        MessageBox.Show("Ticket Date is valid. Proceed");
                    }
                    else
                    {
                        MessageBox.Show("Access Denied.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading image: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}