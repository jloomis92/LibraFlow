using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LibraFlow.Data;
using LibraFlow.Helpers;
using LibraFlow.Models;

namespace LibraFlow.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _username;
        private string _password;
        private string _confirmPassword;
        private string _errorMessage;

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

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

        public string ConfirmPassword
        {
            private get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        public string ErrorMessage
        {
            private get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand RegisterCommand { get; }
        public ICommand BackToLoginCommand { get; }

        public event Action BackToLoginRequested;
        public event Action RegistrationSucceeded;
        public event PropertyChangedEventHandler PropertyChanged;

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(_ => Register());
            BackToLoginCommand = new RelayCommand(_ => BackToLoginRequested?.Invoke());
        }

        private void Register()
        {
            ErrorMessage = string.Empty;

            // Required fields
            if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "All fields are required.";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            // Email format validation
            if (!System.Text.RegularExpressions.Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ErrorMessage = "Please enter a valid email address.";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            // Password length (example: at least 6 characters)
            if (Password.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters.";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            // Password match
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            using var db = new LibraFlowContext();
            if (db.Users.Any(u => u.Username == Username))
            {
                ErrorMessage = "Username already exists.";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            if (db.Users.Any(u => u.Email == Email))
            {
                ErrorMessage = "Email already registered.";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            var user = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Username = Username,
                PasswordHash = PasswordHelper.HashPassword(Password)
            };

            db.Users.Add(user);
            db.SaveChanges();
            RegistrationSucceeded?.Invoke();
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(FirstName):
                        if (string.IsNullOrWhiteSpace(FirstName))
                            return "First name is required.";
                        break;
                    case nameof(LastName):
                        if (string.IsNullOrWhiteSpace(LastName))
                            return "Last name is required.";
                        break;
                    case nameof(Email):
                        if (string.IsNullOrWhiteSpace(Email))
                            return "Email is required.";
                        if (!System.Text.RegularExpressions.Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                            return "Invalid email format.";
                        break;
                    case nameof(Username):
                        if (string.IsNullOrWhiteSpace(Username))
                            return "Username is required.";
                        break;
                    case nameof(Password):
                        if (string.IsNullOrWhiteSpace(Password))
                            return "Password is required.";
                        if (Password != null && Password.Length < 6)
                            return "Password must be at least 6 characters.";
                        break;
                    case nameof(ConfirmPassword):
                        if (string.IsNullOrWhiteSpace(ConfirmPassword))
                            return "Please confirm your password.";
                        if (Password != ConfirmPassword)
                            return "Passwords do not match.";
                        break;
                }
                return null;
            }
        }

        public bool CanRegister =>
            string.IsNullOrEmpty(this[nameof(FirstName)]) &&
            string.IsNullOrEmpty(this[nameof(LastName)]) &&
            string.IsNullOrEmpty(this[nameof(Email)]) &&
            string.IsNullOrEmpty(this[nameof(Username)]) &&
            string.IsNullOrEmpty(this[nameof(Password)]) &&
            string.IsNullOrEmpty(this[nameof(ConfirmPassword)]);

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
