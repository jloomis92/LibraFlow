using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibraFlow.ViewModels;

namespace LibraFlow.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private bool _loginSucceeded = false;

        public LoginView()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel();

            if (DataContext is LoginViewModel vm)
            {
                vm.LoginSucceeded += OnLoginSuccess;
            }
            else
            {
                this.DataContextChanged += (s, e) =>
                {
                    if (e.NewValue is LoginViewModel newVm)
                        newVm.LoginSucceeded += OnLoginSuccess;
                };
            }

            // Set focus to username field when window is loaded
            this.Loaded += (s, e) => 
            {
                UsernameTextBox.Focus();
                UsernameTextBox.SelectAll(); // Optional: select any existing text
            };
        }

        private void OnLoginSuccess(string username)
        {
            _loginSucceeded = true;
            this.DialogResult = true;
            this.Close();
        }
    }
}
