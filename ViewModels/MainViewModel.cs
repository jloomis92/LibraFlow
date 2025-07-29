using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using LibraFlow.Views;
using LibraFlow.Helpers;

namespace LibraFlow.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ICommand NavigateBooksCommand { get; }
        public ICommand NavigateMembersCommand { get; }
        public ICommand NavigateLoansCommand { get; }

        private UserControl _currentView;
        public UserControl CurrentView
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

        public string? CurrentUser { get; set; } // Make nullable

        public MainViewModel()
        {
            // Set default view to WelcomeView
            CurrentView = new Views.WelcomeView();

            NavigateBooksCommand = new RelayCommand(_ => CurrentView = new BooksView());
            NavigateMembersCommand = new RelayCommand(_ => CurrentView = new MembersView());
            NavigateLoansCommand = new RelayCommand(_ => CurrentView = new LoansView());

            // On startup:
            var loginVM = new LoginViewModel();
            loginVM.LoginSucceeded += () => CurrentUser = loginVM.Username;
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
            // Show LoginView as dialog or overlay
        }

        public event PropertyChangedEventHandler? PropertyChanged; // Make nullable
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => // Make parameter nullable
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
