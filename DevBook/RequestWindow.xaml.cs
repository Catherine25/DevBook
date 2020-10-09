using System.Windows;

namespace DevBook
{
    /// <summary>
    /// Interaction logic for RequestWindow.xaml
    /// </summary>
    public partial class RequestWindow : Window
    {
        public RequestWindow()
        {
            InitializeComponent();

            Title = "Enter file path";
            xOk.Click += XOk_Click;
            xNok.Click += XNok_Click;
        }

        private void XNok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void XOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
