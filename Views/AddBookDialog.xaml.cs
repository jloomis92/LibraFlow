using System;
using System.Windows;
using LibraFlow.Models;
using LibraFlow.Helpers;

namespace LibraFlow.Views
{
    /// <summary>
    /// Interaction logic for AddBookDialog.xaml
    /// </summary>
    public partial class AddBookDialog : Window
    {
        public Book Book { get; private set; }

        public AddBookDialog()
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this); // Apply the current theme
            ThemeManager.ThemeChanged += OnThemeChanged; // Listen for theme changes
            Book = new Book();
            DataContext = Book;
        }

        public AddBookDialog(Book existingBook)
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this); // Apply the current theme
            ThemeManager.ThemeChanged += OnThemeChanged; // Listen for theme changes
            if (existingBook != null)
            {
                Book = new Book
                {
                    Id = existingBook.Id,
                    Title = existingBook.Title,
                    Author = existingBook.Author,
                    ISBN = existingBook.ISBN,
                    IsCheckedOut = existingBook.IsCheckedOut
                };
                DataContext = Book;
            }
            else
            {
                Book = new Book();
                DataContext = Book;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Book = new Book
            {
                Title = TitleBox.Text,
                Author = AuthorBox.Text,
                ISBN = ISBNBox.Text,
                IsCheckedOut = false
            };
            DialogResult = true;
            Close();
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
    }
}
