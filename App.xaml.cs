using System.Configuration;
using System.Data;
using System.Windows;
using LibraFlow.Helpers;

namespace LibraFlow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // When toggling theme
            ThemeManager.ChangeTheme("Dark"); // or "Light"
            ThemeManager.ApplyTheme(null);    // Apply immediately

            ThemeManager.ApplyTheme(MainWindow);

            // Create the main window
            var mainWindow = new MainWindow();

            // Apply the saved theme before showing the window
            ThemeManager.ApplyTheme(mainWindow);

            mainWindow.Show();
        }
    }
}
