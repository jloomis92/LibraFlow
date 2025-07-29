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
        public App()
        {
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Apply the saved theme at startup
            ThemeManager.ApplyTheme(Current.MainWindow ?? new Window());

            var loginVM = new LoginViewModel();
            var loginView = new LoginView { DataContext = loginVM };

            loginVM.ShowRegisterRequested += () =>
            {
                var registerVM = new RegisterViewModel();
                var registerWindow = new RegisterView { DataContext = registerVM };
                registerWindow.Owner = loginView;
                ThemeManager.ApplyTheme(registerWindow);

                // Hide the login window while registration is open
                loginView.Hide();

                registerVM.BackToLoginRequested += () =>
                {
                    // This will be called when registration is successful and user wants to return
                    registerWindow.Close();
                    loginView.Show();
                };

                registerWindow.ShowDialog();

                // If the user closes the register window without registering, show login again
                if (!loginView.IsVisible)
                    loginView.Show();
            };

            loginVM.LoginSucceeded += (username) =>
            {
                var mainWindow = new MainWindow();
                // Set the username on the MainViewModel
                if (mainWindow.DataContext is MainViewModel mainVM)
                {
                    mainVM.CurrentUsername = username;
                }
                ThemeManager.ApplyTheme(mainWindow);

                // Show the main window first, then close the login window
                mainWindow.Show();
                this.MainWindow = mainWindow; // Set MainWindow AFTER showing

                // Detach loginView from MainWindow before closing it
                if (this.Windows.OfType<Window>().Contains(loginView))
                {
                    loginView.Hide();
                    loginView.Owner = null;
                }
                loginView.Close();
            };

            // Set loginView as MainWindow initially, so closing it before login will shut down the app
            this.MainWindow = loginView;
            loginView.Show();
        }
    }
}
