using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
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

        public string CurrentUser { get; set; }

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
            // Show LoginView as dialog or overlay
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
