using System;
using System.Windows;
using System.Windows.Controls;
using LibraFlow.Helpers;
using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;
using WinForms = System.Windows.Forms;

namespace LibraFlow.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : System.Windows.Controls.UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load theme setting
            var currentTheme = Properties.Settings.Default.AppTheme;
            if (string.IsNullOrEmpty(currentTheme) || currentTheme.Equals("Light", StringComparison.OrdinalIgnoreCase))
            {
                ThemeComboBox.SelectedIndex = 0; // Light
            }
            else
            {
                ThemeComboBox.SelectedIndex = 1; // Dark
            }

            // Load backup settings
            AutoBackupCheckBox.IsChecked = Properties.Settings.Default.AutoBackupEnabled;
            
            var backupDays = Properties.Settings.Default.AutoBackupDays;
            for (int i = 0; i < BackupFrequencyComboBox.Items.Count; i++)
            {
                if (BackupFrequencyComboBox.Items[i] is ComboBoxItem item && 
                    item.Tag != null &&
                    int.Parse(item.Tag.ToString()!) == backupDays)
                {
                    BackupFrequencyComboBox.SelectedIndex = i;
                    break;
                }
            }

            var backupLocation = Properties.Settings.Default.BackupDirectory;
            if (string.IsNullOrEmpty(backupLocation))
            {
                backupLocation = BackupService.GetDefaultBackupDirectory();
            }
            BackupLocationTextBox.Text = backupLocation;

            // Update last backup display
            UpdateLastBackupDisplay();
        }

        private void UpdateLastBackupDisplay()
        {
            LastBackupLabel.Text = $"Last backup: {BackupService.GetLastBackupDisplayText()}";
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                var newTheme = selectedItem.Content?.ToString();
                if (!string.IsNullOrEmpty(newTheme))
                {
                    ThemeManager.ChangeTheme(newTheme);
                    
                    // Apply theme immediately
                    var baseTheme = newTheme.Equals("Dark", StringComparison.OrdinalIgnoreCase)
                        ? MaterialDesignThemes.Wpf.BaseTheme.Dark
                        : MaterialDesignThemes.Wpf.BaseTheme.Light;
                    ThemeManager.SetBaseTheme(baseTheme);
                }
            }
        }

        private async void CreateBackupButton_Click(object sender, RoutedEventArgs e)
        {
            CreateBackupButton.IsEnabled = false;
            CreateBackupButton.Content = "Creating...";

            try
            {
                var success = await BackupService.CreateBackupAsync();
                if (success)
                {
                    System.Windows.MessageBox.Show("Backup created successfully!", "Backup Complete",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateLastBackupDisplay();
                }
            }
            finally
            {
                CreateBackupButton.IsEnabled = true;
                CreateBackupButton.Content = "Create Backup";
            }
        }

        private async void RestoreBackupButton_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.MessageBox.Show(
                "Restoring a backup will replace your current database. This action cannot be undone.\n\nDo you want to continue?",
                "Confirm Restore", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select Backup File",
                Filter = "Database files (*.db)|*.db|All files (*.*)|*.*",
                InitialDirectory = BackupService.GetDefaultBackupDirectory()
            };

            if (openFileDialog.ShowDialog() == true)
            {
                RestoreBackupButton.IsEnabled = false;
                RestoreBackupButton.Content = "Restoring...";

                try
                {
                    var success = await BackupService.RestoreBackupAsync(openFileDialog.FileName);
                    if (success)
                    {
                        System.Windows.MessageBox.Show("Database restored successfully! The application will restart.", 
                            "Restore Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Restart the application
                        System.Diagnostics.Process.Start(Environment.ProcessPath ?? "");
                        System.Windows.Application.Current.Shutdown();
                    }
                }
                finally
                {
                    RestoreBackupButton.IsEnabled = true;
                    RestoreBackupButton.Content = "Restore Backup";
                }
            }
        }

        private void AutoBackupCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoBackupEnabled = true;
            Properties.Settings.Default.Save();
        }

        private void AutoBackupCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoBackupEnabled = false;
            Properties.Settings.Default.Save();
        }

        private void BackupFrequencyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BackupFrequencyComboBox.SelectedItem is ComboBoxItem selectedItem && 
                selectedItem.Tag != null)
            {
                var days = int.Parse(selectedItem.Tag.ToString()!);
                Properties.Settings.Default.AutoBackupDays = days;
                Properties.Settings.Default.Save();
            }
        }

        private void BrowseBackupLocationButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new WinForms.FolderBrowserDialog
            {
                Description = "Select backup location",
                SelectedPath = BackupLocationTextBox.Text
            };

            if (folderDialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                BackupLocationTextBox.Text = folderDialog.SelectedPath;
                Properties.Settings.Default.BackupDirectory = folderDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }
    }
}
