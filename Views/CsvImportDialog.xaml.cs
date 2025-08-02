using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LibraFlow.Helpers;
using LibraFlow.Models;

namespace LibraFlow.Views
{
    /// <summary>
    /// Interaction logic for CsvImportDialog.xaml
    /// </summary>
    public partial class CsvImportDialog : Window, INotifyPropertyChanged
    {
        private bool _isImporting;
        private bool _canImport;
        private string _selectedFilePath;

        public bool IsImporting
        {
            get => _isImporting;
            set
            {
                _isImporting = value;
                OnPropertyChanged(nameof(IsImporting));
                OnPropertyChanged(nameof(CanImport));
            }
        }

        public bool CanImport => !IsImporting && !string.IsNullOrEmpty(_selectedFilePath);

        public List<Book> ImportedBooks { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public CsvImportDialog()
        {
            InitializeComponent();
            DataContext = this;
            ThemeManager.ApplyTheme(this);
            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Select CSV file to import"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
                FilePathTextBox.Text = _selectedFilePath;
                OnPropertyChanged(nameof(CanImport));
            }
        }

        private void DownloadTemplate_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Save CSV Template",
                FileName = "books_template.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, CsvImportService.GetCsvTemplate());
                    System.Windows.MessageBox.Show("Template saved successfully!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error saving template: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedFilePath))
            {
                System.Windows.MessageBox.Show("Please select a CSV file first.", "No File Selected", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsImporting = true;

            try
            {
                await Task.Run(() =>
                {
                    ImportedBooks = CsvImportService.ParseBooksFromCsv(_selectedFilePath);
                });

                if (ImportedBooks == null || !ImportedBooks.Any())
                {
                    System.Windows.MessageBox.Show("No valid books found in the CSV file.", "No Data", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = System.Windows.MessageBox.Show(
                    $"Found {ImportedBooks.Count} books to import. Continue?", 
                    "Confirm Import", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error importing CSV: {ex.Message}", "Import Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsImporting = false;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnThemeChanged()
        {
            ThemeManager.ApplyTheme(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            ThemeManager.ThemeChanged -= OnThemeChanged;
            base.OnClosed(e);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
