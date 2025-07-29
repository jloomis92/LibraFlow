using System;
using System.Linq;
using System.Windows;
using MaterialDesignThemes.Wpf; // Add this using directive
using LibraFlow.ViewModels;
using LibraFlow.Helpers;
using LibraFlow.Views;

namespace LibraFlow
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            // Subscribe to theme changes
            ThemeManager.ThemeChanged += () => ThemeManager.ApplyTheme(this);
        }

        private void SetBaseTheme(BaseTheme baseTheme)
        {
            foreach (var dict in Application.Current.Resources.MergedDictionaries)
            {
                if (dict is BundledTheme bundledTheme)
                {
                    bundledTheme.BaseTheme = baseTheme;
                    break;
                }
            }
        }

        // Example: Call this method from a button/menu to change the theme
        private void OnChangeTheme(string newTheme)
        {
            ThemeManager.ChangeTheme(newTheme);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SettingsView();
        }

        private void BooksButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new BooksView();
        }

        private void MembersButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new MembersView();
        }

        private void LoansButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new LoansView();
        }
    }
}