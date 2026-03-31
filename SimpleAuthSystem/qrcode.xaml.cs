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
using ZXing;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using ZXing.QrCode;



namespace SimpleAuthSystem
{
    /// <summary>
    /// Interaction logic for qrcode.xaml
    /// </summary>
    public partial class qrcode : Window
    {
        public qrcode()
        {
            //var reader = new BarcodeReader();
            //var bitmap = (Bitmap)Bitmap.FromFile("qr.png");

            //var result = reader.Decode(bitmap);

            //if (result != null)
            //{
            //    string text = result.Text;
            //}
        }
    }
}
