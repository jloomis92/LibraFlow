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
    public partial class LoginView : Window // Change from Page to Window
    {
        public LoginView()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel(); // <-- Add this line

            if (DataContext is LoginViewModel vm)
            {
                vm.LoginSucceeded += OnLoginSuccess;
            }
            else
            {
                // If DataContext is set later, subscribe then
                this.DataContextChanged += (s, e) =>
                {
                    if (e.NewValue is LoginViewModel newVm)
                        newVm.LoginSucceeded += OnLoginSuccess;
                };
            }
        }

        private void OnLoginSuccess()
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
