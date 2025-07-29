using System;
using System.Windows.Input;
using LibraFlow.Helpers;
using LibraFlow.Data;
using System.Linq;
using System.ComponentModel;
using LibraFlow.Models;
using System.Windows;
using LibraFlow.ViewModels;

namespace LibraFlow.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private string _errorMessage;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            private get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand ShowRegisterCommand { get; }

        public event Action<string> LoginSucceeded;
        public event Action ShowRegisterRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(_ => Login());
            ShowRegisterCommand = new RelayCommand(_ => ShowRegisterRequested?.Invoke());
        }

        private void Login()
        {
            ErrorMessage = string.Empty;

            using var db = new LibraFlowContext();
            var user = db.Users.FirstOrDefault(u => u.Username == Username);

            // Always show the same error message for both cases for security
            if (user == null || !PasswordHelper.VerifyPassword(Password, user.PasswordHash))
            {
                ErrorMessage = "Invalid username or password.";
                return;
            }

            // Raise event to notify view of successful login, passing the username
            LoginSucceeded?.Invoke(user.Username);
        }

        private void ShowRegister()
        {
            // Raise event to notify view to switch to register
            ShowRegisterRequested?.Invoke();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
