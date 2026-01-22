using System.Windows;

namespace PhoneBookWPF
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ThemeManager.Initialize();

            // Utwórz główne okno
            MainWindow window = new MainWindow();
            window.Show();
        }
    }
}