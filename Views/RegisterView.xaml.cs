using System;
using System.Windows;
using System.Windows.Controls;
using LibraFlow.ViewModels;

namespace LibraFlow.Views
{
    public partial class RegisterView : Window
    {
        public RegisterView()
        {
            InitializeComponent();

            if (DataContext is RegisterViewModel vm)
            {
                AttachHandlers(vm);
            }
            else
            {
                this.DataContextChanged += (s, e) =>
                {
                    if (e.NewValue is RegisterViewModel vm2)
                        AttachHandlers(vm2);
                };
            }
        }

        private void AttachHandlers(RegisterViewModel vm)
        {
            vm.RegistrationSucceeded += () =>
            {
                MessageBoxResult result = MessageBox.Show(
                    "Registration successful! Return to login?",
                    "Success",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    vm.OnBackToLoginRequested();
                    this.Close();
                }
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm && sender is PasswordBox pb)
            {
                typeof(RegisterViewModel)
                    .GetProperty("Password", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)
                    ?.SetValue(vm, pb.Password);

                // Notify the UI that Password (and CanRegister) may have changed
                vm.GetType().GetMethod("OnPropertyChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.Invoke(vm, new object[] { "Password" });
                vm.GetType().GetMethod("OnPropertyChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.Invoke(vm, new object[] { "CanRegister" });
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm && sender is PasswordBox pb)
            {
                typeof(RegisterViewModel)
                    .GetProperty("ConfirmPassword", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)
                    ?.SetValue(vm, pb.Password);

                // Notify the UI that ConfirmPassword (and CanRegister) may have changed
                vm.GetType().GetMethod("OnPropertyChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.Invoke(vm, new object[] { "ConfirmPassword" });
                vm.GetType().GetMethod("OnPropertyChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.Invoke(vm, new object[] { "CanRegister" });
            }
        }
    }
}
