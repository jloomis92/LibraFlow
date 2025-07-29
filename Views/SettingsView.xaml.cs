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
using LibraFlow.Helpers;

namespace LibraFlow.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();

            // Set ComboBox to current theme
            string currentTheme = ThemeManager.GetCurrentTheme(); // Adjust property name as needed
            if (!string.IsNullOrEmpty(currentTheme))
            {
                foreach (ComboBoxItem item in ThemeComboBox.Items)
                {
                    if (string.Equals(item.Content?.ToString(), currentTheme, StringComparison.OrdinalIgnoreCase))
                    {
                        ThemeComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTheme = (ThemeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (!string.IsNullOrEmpty(selectedTheme))
            {
                ThemeManager.ChangeTheme(selectedTheme);
                ThemeManager.ApplyTheme(Application.Current.MainWindow);
            }
        }
    }
}
