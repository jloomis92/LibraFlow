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
            ThemeManager.ApplyTheme(this);
            ThemeManager.ThemeChanged += OnThemeChanged;
            Book = new Book();
            DataContext = Book;
        }

        public AddBookDialog(Book existingBook) : this()
        {
            if (existingBook != null)
            {
                Book = new Book
                {
                    Id = existingBook.Id,
                    Title = existingBook.Title,
                    Author = existingBook.Author,
                    ISBN = existingBook.ISBN, // Ensure ISBN is copied
                    IsCheckedOut = existingBook.IsCheckedOut
                };
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
