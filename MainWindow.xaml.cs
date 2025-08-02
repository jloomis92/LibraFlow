using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using LibraFlow.ViewModels;
using LibraFlow.Helpers;
using LibraFlow.Views;

namespace LibraFlow
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            
            // Ensure the window can receive keyboard focus
            this.Focusable = true;
            this.KeyDown += MainWindow_KeyDown;

            // Subscribe to notification events
            NotificationService.NotificationRequested += OnNotificationRequested;

            // Test notifications immediately on startup (for debugging)
            this.Loaded += (s, e) => {
                // Give the UI time to load, then test
                this.Dispatcher.BeginInvoke(new Action(() => {
                    // Uncomment this line to test notifications on startup
                    // TestSingleNotification();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            };

            // Debug: Check if Snackbar is properly initialized
            this.Loaded += (s, e) => {
                System.Diagnostics.Debug.WriteLine($"NotificationSnackbar: {NotificationSnackbar}");
                System.Diagnostics.Debug.WriteLine($"MessageQueue: {NotificationSnackbar?.MessageQueue}");
            };
        }

        private void OnNotificationRequested(string title, string message, NotificationService.NotificationType type)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Notification requested: {type} - {message}");
                
                // Simple approach - just show the message as text
                var simpleMessage = $"{GetIconForType(type)} {message}";
                
                // Ensure we're on the UI thread
                this.Dispatcher.Invoke(() => {
                    if (NotificationSnackbar?.MessageQueue != null)
                    {
                        NotificationSnackbar.MessageQueue.Enqueue(simpleMessage, null, null, null, false, true, TimeSpan.FromSeconds(4));
                        System.Diagnostics.Debug.WriteLine("Message enqueued successfully");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: NotificationSnackbar or MessageQueue is null");
                        
                        // Fallback: Use MessageBox for debugging
                        System.Windows.MessageBox.Show(message, $"Notification ({type})", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnNotificationRequested: {ex.Message}");
            }
        }

        private string GetIconForType(NotificationService.NotificationType type)
        {
            return type switch
            {
                NotificationService.NotificationType.Success => "✅",
                NotificationService.NotificationType.Warning => "⚠️",
                NotificationService.NotificationType.Error => "❌",
                _ => "ℹ️"
            };
        }

        // Simple method to test a single notification
        private void TestSingleNotification()
        {
            System.Diagnostics.Debug.WriteLine("TestSingleNotification called");
            NotificationService.ShowNotification("Test notification - this is working!", NotificationService.NotificationType.Success);
        }

        // Method to test all notification types
        private void TestAllNotifications()
        {
            System.Diagnostics.Debug.WriteLine("TestAllNotifications called");
            
            var timer = new System.Windows.Threading.DispatcherTimer();
            var notifications = new[]
            {
                () => NotificationService.ShowNotification("Database connection restored", NotificationService.NotificationType.Success),
                () => NotificationService.ShowNotification("Please check your settings", NotificationService.NotificationType.Warning),
                () => NotificationService.ShowNotification("Connection failed", NotificationService.NotificationType.Error),
                () => NotificationService.ShowNotification("System information updated", NotificationService.NotificationType.Info)
            };

            int index = 0;
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += (s, e) =>
            {
                if (index < notifications.Length)
                {
                    notifications[index]();
                    index++;
                }
                else
                {
                    timer.Stop();
                }
            };
            timer.Start();
        }

        private void SetBaseTheme(BaseTheme baseTheme)
        {
            foreach (var dict in System.Windows.Application.Current.Resources.MergedDictionaries)
            {
                if (dict is BundledTheme bundledTheme)
                {
                    bundledTheme.BaseTheme = baseTheme;
                    break;
                }
            }
        }

        private void OnChangeTheme(string newTheme)
        {
            ThemeManager.ChangeTheme(newTheme);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SettingsView();
        }

        private void BooksButton_Click(object sender, RoutedEventArgs e)
        {
            var booksView = new BooksView();
            booksView.DataContext = new BooksViewModel();
            MainContent.Content = booksView;
        }

        private void MembersButton_Click(object sender, RoutedEventArgs e)
        {
            var membersView = new MembersView();
            membersView.DataContext = new MembersViewModel();
            MainContent.Content = membersView;
        }

        private void LoansButton_Click(object sender, RoutedEventArgs e)
        {
            var loansView = new LoansView();
            loansView.DataContext = new LoansViewModel();
            MainContent.Content = loansView;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
                vm.Logout();

            NotificationService.NotificationRequested -= OnNotificationRequested;

            var loginWindow = new Views.LoginView();
            System.Windows.Application.Current.MainWindow = loginWindow;
            loginWindow.Show();
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            NotificationService.NotificationRequested -= OnNotificationRequested;
            base.OnClosed(e);
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Debug: Show what key was pressed
            System.Diagnostics.Debug.WriteLine($"Key pressed: {e.Key}, Ctrl: {Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)}, Shift: {Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)}");

            // Press F5 to test a single notification (easier to test)
            if (e.Key == Key.F5)
            {
                System.Diagnostics.Debug.WriteLine("F5 pressed - testing notification");
                TestSingleNotification();
                e.Handled = true;
            }

            // Press F6 to test all notifications
            if (e.Key == Key.F6)
            {
                System.Diagnostics.Debug.WriteLine("F6 pressed - testing all notifications");
                TestAllNotifications();
                e.Handled = true;
            }

            // Press Ctrl+Shift+T to test all notifications
            if (e.Key == Key.T && 
                (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) &&
                (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                TestAllNotifications();
                e.Handled = true;
            }
        }
    }
}