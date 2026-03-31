using System.Windows;


namespace SimpleAuthSystem.QR
{
    public partial class QrWriterWindow : Window
    {
        private readonly MainViewModel _vm = new MainViewModel();

        public QrWriterWindow()
        {
            InitializeComponent();
            DataContext = _vm;
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(_vm.GenerateNew());
            _vm.GenerateNew();
        }
    }
}