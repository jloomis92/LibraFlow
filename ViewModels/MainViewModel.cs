using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using LibraFlow.Views;
using LibraFlow.Helpers;
using LibraFlow.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.IO;
using WpfControls = System.Windows.Controls;

namespace LibraFlow.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _timer;

        public ICommand NavigateBooksCommand { get; }
        public ICommand NavigateMembersCommand { get; }
        public ICommand NavigateLoansCommand { get; }

        private WpfControls.UserControl _currentView;
        public WpfControls.UserControl CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set { _isDarkTheme = value; OnPropertyChanged(); Helpers.ThemeManager.SetBaseTheme(_isDarkTheme ? MaterialDesignThemes.Wpf.BaseTheme.Dark : MaterialDesignThemes.Wpf.BaseTheme.Light); }
        }

        private string? _currentUsername;
        public string? CurrentUsername
        {
            get => _currentUsername;
            set
            {
                if (_currentUsername != value)
                {
                    _currentUsername = value;
                    OnPropertyChanged(nameof(CurrentUsername));
                }
            }
        }

        // Status Bar Properties
        private string? _databaseName;
        public string? DatabaseName
        {
            get => _databaseName;
            set { _databaseName = value; OnPropertyChanged(); }
        }

        private string? _userRole;
        public string? UserRole
        {
            get => _userRole;
            set { _userRole = value; OnPropertyChanged(); }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set { _isConnected = value; OnPropertyChanged(); OnPropertyChanged(nameof(ConnectionStatus)); }
        }

        public string ConnectionStatus => IsConnected ? "Connected" : "Disconnected";

        private string? _recordsSummary;
        public string? RecordsSummary
        {
            get => _recordsSummary;
            set { _recordsSummary = value; OnPropertyChanged(); }
        }

        private DateTime _currentTime;
        public DateTime CurrentTime
        {
            get => _currentTime;
            set { _currentTime = value; OnPropertyChanged(); }
        }

        private string? _lastBackupText;
        public string? LastBackupText
        {
            get => _lastBackupText;
            set { _lastBackupText = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            // Set default view to WelcomeView
            CurrentView = new Views.WelcomeView();

            NavigateBooksCommand = new RelayCommand(_ => CurrentView = new BooksView());
            NavigateMembersCommand = new RelayCommand(_ => CurrentView = new MembersView());
            NavigateLoansCommand = new RelayCommand(_ => CurrentView = new LoansView());

            // Initialize status bar properties
            InitializeStatusBar();

            // Setup timer for current time updates and backup checks
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30) // Check every 30 seconds
            };
            _timer.Tick += async (s, e) =>
            {
                CurrentTime = DateTime.Now;
                LastBackupText = BackupService.GetLastBackupDisplayText();
            };
            _timer.Start();

            // Subscribe to backup events
            BackupService.BackupCompleted += OnBackupCompleted;

            // On startup:
            var loginVM = new LoginViewModel();
            loginVM.LoginSucceeded += username =>
            {
                CurrentUsername = username;
                UserRole = "Administrator"; // Placeholder - will be replaced with actual RBAC

                // Check for auto backup after login
                _ = Task.Run(async () => await BackupService.CheckAutoBackupAsync());
            };
            loginVM.ShowRegisterRequested += () =>
            {
                // Open RegisterView as a new Window instead of setting it as CurrentView
                var registerWindow = new Window
                {
                    Content = new RegisterView(),
                    Title = "Register",
                    Width = 400,
                    Height = 600,
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner,
                    Owner = System.Windows.Application.Current.MainWindow
                };
                registerWindow.ShowDialog();
            };
        }

        private void OnBackupCompleted(DateTime backupTime)
        {
            // Update the backup text immediately when backup completes
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                LastBackupText = BackupService.GetLastBackupDisplayText();
            });
        }

        private void InitializeStatusBar()
        {
            // Set database name from the LibraFlowContext configuration
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LibraFlow", "library.db");
            DatabaseName = $"SQLite: {Path.GetFileName(dbPath)}";

            // Set default values
            UserRole = "Administrator"; // Placeholder for RBAC
            IsConnected = true; // Will be updated based on actual connection status
            CurrentTime = DateTime.Now;
            LastBackupText = BackupService.GetLastBackupDisplayText();

            // Load records summary
            _ = UpdateRecordsSummaryAsync();
        }

        private async Task UpdateRecordsSummaryAsync()
        {
            try
            {
                using var context = new LibraFlowContext();

                var booksCount = await context.Books.CountAsync();
                var membersCount = await context.Members.CountAsync();
                var activeLoansCount = await context.Loans.CountAsync(l => l.ReturnDate == null);

                RecordsSummary = $"Books: {booksCount} | Members: {membersCount} | Active Loans: {activeLoansCount}";
                IsConnected = true;
            }
            catch (Exception)
            {
                RecordsSummary = "Unable to load records";
                IsConnected = false;
            }
        }

        public async Task RefreshStatusAsync()
        {
            await UpdateRecordsSummaryAsync();
            LastBackupText = BackupService.GetLastBackupDisplayText();
        }

        public void Logout()
        {
            CurrentUsername = null;
            UserRole = null;
            // Any additional cleanup
        }

        public event PropertyChangedEventHandler? PropertyChanged; // Make nullable
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => // Make parameter nullable
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}