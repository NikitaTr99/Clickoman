using System.Windows;

namespace Clickoman.windows
{
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void NextButton_OnClick(object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow;

            var name = PlayerNameInput.Text;

            if (name != string.Empty) mainWindow = new MainWindow(name);
            else mainWindow = new MainWindow("Unknown player");

            mainWindow.Show();
            Close();
        }
    }
}