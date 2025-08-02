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
        private readonly DispatcherTimer _connectionTimer;
        private bool? _previousConnectionState = null; // Track previous state for change detection

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
            set 
            { 
                if (_isConnected != value)
                {
                    var previousState = _isConnected;
                    _isConnected = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ConnectionStatus));
                    
                    // Notify about connection status changes (but not on initial startup)
                    if (_previousConnectionState.HasValue)
                    {
                        HandleConnectionStatusChange(previousState, value);
                    }
                    _previousConnectionState = value;
                }
            }
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

            NavigateBooksCommand = new RelayCommand(_ => NavigateToBooks());
            NavigateMembersCommand = new RelayCommand(_ => NavigateToMembers());
            NavigateLoansCommand = new RelayCommand(_ => NavigateToLoans());

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

            // Setup connection monitoring timer
            _connectionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5) // Check connection every 5 seconds
            };
            _connectionTimer.Tick += async (s, e) => await CheckDatabaseConnectionAsync();
            _connectionTimer.Start();

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

        private void NavigateToBooks()
        {
            var booksView = new BooksView();
            booksView.DataContext = new BooksViewModel();
            CurrentView = booksView;
        }

        private void NavigateToMembers()
        {
            var membersView = new MembersView();
            membersView.DataContext = new MembersViewModel();
            CurrentView = membersView;
        }

        private void NavigateToLoans()
        {
            var loansView = new LoansView();
            loansView.DataContext = new LoansViewModel();
            CurrentView = loansView;
        }

        private void HandleConnectionStatusChange(bool previousState, bool currentState)
        {
            if (currentState && !previousState)
            {
                // Connection restored
                NotificationService.ShowConnectionRestored();
            }
            else if (!currentState && previousState)
            {
                // Connection lost
                NotificationService.ShowConnectionLost();
            }
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
            IsConnected = false; // Start as disconnected, will be updated by connection timer
            CurrentTime = DateTime.Now;
            LastBackupText = BackupService.GetLastBackupDisplayText();

            // Initial connection check
            _ = CheckDatabaseConnectionAsync();
        }

        private async Task CheckDatabaseConnectionAsync()
        {
            try
            {
                using var context = new LibraFlowContext();
                
                // Test the connection by opening it and executing a simple query
                await context.Database.OpenConnectionAsync();
                var canConnect = await context.Database.CanConnectAsync();
                
                if (canConnect)
                {
                    // Update records summary while we have a good connection
                    var booksCount = await context.Books.CountAsync();
                    var membersCount = await context.Members.CountAsync();
                    var activeLoansCount = await context.Loans.CountAsync(l => l.ReturnDate == null);

                    // Update UI on the UI thread
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        RecordsSummary = $"Books: {booksCount} | Members: {membersCount} | Active Loans: {activeLoansCount}";
                        IsConnected = true;
                    });
                }
                else
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        RecordsSummary = "Unable to load records";
                        IsConnected = false;
                    });
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                System.Diagnostics.Debug.WriteLine($"Database connection check failed: {ex.Message}");
                
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    RecordsSummary = "Database connection failed";
                    IsConnected = false;
                });
            }
        }

        private async Task UpdateRecordsSummaryAsync()
        {
            // This method is now handled by CheckDatabaseConnectionAsync
            // Keep for backward compatibility but delegate to the connection check
            await CheckDatabaseConnectionAsync();
        }

        public async Task RefreshStatusAsync()
        {
            await CheckDatabaseConnectionAsync();
            LastBackupText = BackupService.GetLastBackupDisplayText();
        }

        public void Logout()
        {
            CurrentUsername = null;
            UserRole = null;
            
            // Stop the connection monitoring timer when logged out
            _connectionTimer?.Stop();
            
            // Reset connection state tracking
            _previousConnectionState = null;
            
            // Any additional cleanup
        }

        // Dispose method to clean up timers
        public void Dispose()
        {
            _timer?.Stop();
            _connectionTimer?.Stop();
        }

        public event PropertyChangedEventHandler? PropertyChanged; // Make nullable
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => // Make parameter nullable
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}