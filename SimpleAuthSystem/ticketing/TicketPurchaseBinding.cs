using QRCoder;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

public class TicketPurchaseBinding : INotifyPropertyChanged
{
    private readonly CodeGenerator _generator;
    private readonly PersistentCounter _fileCounter;

    private long _currentId;
    private string _generatedCode;
    private BitmapImage _qrCodeImage;

    // Sample text variables
    public string TitleText { get; set; } = "Title";
    public string ContentText { get; set; } = "Seasonal Autoregressive Integrated Moving Average";

    public TicketPurchaseBinding()
    {
        var counterFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "counter.txt");
        _generator = new CodeGenerator(counterFile);

        // Separate counter for saving images
        var fileCounterFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "file_counter.txt");
        _fileCounter = new PersistentCounter(fileCounterFile);
    }

    //public long CurrentId
    //{
    //    get => _currentId;
    //    set { _currentId = value; OnPropertyChanged(nameof(CurrentId)); }
    //}

    //public string GeneratedCode
    //{
    //    get => _generatedCode;
    //    set { _generatedCode = value; OnPropertyChanged(nameof(GeneratedCode)); }
    //}

    public BitmapImage QrCodeImage
    {
        get => _qrCodeImage;
        set { _qrCodeImage = value; OnPropertyChanged(nameof(QrCodeImage)); }
    }

    /// <summary>
    /// Generates a new code, updates UI bindings, returns the obfuscated string, and saves the image.
    /// </summary>
    public string GenerateNew(string code, string TitleText, string ContentText)
    {
        long id = _generator.GetNextId();

        //CurrentId = id;
        //GeneratedCode = _generator.GenerateCode(id);

        var bmp = GenerateQrCodeWithText(code, TitleText, ContentText);

        // Save automatically
        SaveImage(bmp, TitleText);

        // Convert to BitmapImage for UI
        QrCodeImage = BitmapToImage(bmp);

        return code;
    }

    /// <summary>
    /// Generates QR code as Bitmap with Title and Content text overlay.
    /// </summary>
    private Bitmap GenerateQrCodeWithText(string qrText, string title, string content)
    {
        using (var generator = new QRCodeGenerator())
        using (var data = generator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q))
        using (var qrCode = new BitmapByteQRCode(data))
        {
            byte[] qrBytes = qrCode.GetGraphic(20);
            using (var ms = new MemoryStream(qrBytes))
            using (var bmp = new Bitmap(ms))
            {
                int extraHeight = 60;
                var finalBmp = new Bitmap(bmp.Width, bmp.Height + extraHeight);
                using (var g = Graphics.FromImage(finalBmp))
                {
                    g.Clear(Color.White);
                    g.DrawImage(bmp, 0, extraHeight / 2);

                    using (var titleFont = new Font("Arial", 12, FontStyle.Bold))
                    using (var brush = new SolidBrush(Color.Black))
                    {
                        var titleSize = g.MeasureString(title, titleFont);
                        g.DrawString(title, titleFont, brush, (finalBmp.Width - titleSize.Width) / 2, 0);
                    }

                    using (var contentFont = new Font("Arial", 10))
                    using (var brush = new SolidBrush(Color.Black))
                    {
                        var contentSize = g.MeasureString(content, contentFont);
                        g.DrawString(content, contentFont, brush, (finalBmp.Width - contentSize.Width) / 2, bmp.Height + extraHeight / 2);
                    }
                }

                return finalBmp;
            }
        }
    }

    /// <summary>
    /// Saves the Bitmap to a file with Title + incrementing number suffix
    /// </summary>
    private void SaveImage(Bitmap bmp, string title)
    {
        int fileIndex = (int)_fileCounter.Next();
        string safeTitle = MakeFileNameSafe(title);
        string fileName = $"{safeTitle}_{fileIndex}.png";

        string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        bmp.Save(savePath, ImageFormat.Png);
    }

    /// <summary>
    /// Converts Bitmap to BitmapImage for WPF binding
    /// </summary>
    private BitmapImage BitmapToImage(Bitmap bmp)
    {
        using (var ms = new MemoryStream())
        {
            bmp.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = ms;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();
            return image;
        }
    }

    /// <summary>
    /// Makes a string safe for file names
    /// </summary>
    private string MakeFileNameSafe(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}