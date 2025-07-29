using System.Configuration;
using System.Data;
using System.Windows;
using LibraFlow.Helpers;
using LibraFlow.Views;
using LibraFlow.ViewModels;

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

            // Apply the saved theme at startup
            ThemeManager.ApplyTheme(Current.MainWindow ?? new Window());

            var loginVM = new LoginViewModel();
            var loginView = new LoginView { DataContext = loginVM };

            loginVM.ShowRegisterRequested += () =>
            {
                var registerWindow = new RegisterView();
                registerWindow.Owner = loginView;
                // Apply theme to the register window
                ThemeManager.ApplyTheme(registerWindow);
                registerWindow.ShowDialog();
            };

            bool? result = loginView.ShowDialog();

            if (result == true)
            {
                var mainWindow = new MainWindow();
                // Apply theme to the main window
                ThemeManager.ApplyTheme(mainWindow);
                mainWindow.Show();
            }
            else
            {
                Shutdown();
            }
        }
    }
}
