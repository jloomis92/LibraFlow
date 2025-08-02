using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;
using LibraFlow.Helpers;
using LibraFlow.Models;
using LibraFlow.Data;
using System.Windows;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace LibraFlow.ViewModels
{
    public class BooksViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Book> Books { get; set; }
        public ICollectionView BooksView { get; }
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    BooksView.Refresh();
                }
            }
        }

        public ICommand AddBookCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand ImportCsvCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private Book _selectedBook;
        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                if (_selectedBook != value)
                {
                    _selectedBook = value;
                    OnPropertyChanged(nameof(SelectedBook));
                }
            }
        }

        public BooksViewModel()
        {
            using var db = new LibraFlowContext();
            Books = new ObservableCollection<Book>(db.Books.ToList());
            AddBookCommand = new RelayCommand(_ => OpenAddBookDialog());
            EditBookCommand = new RelayCommand(param => EditBook(param as Book));
            ImportCsvCommand = new RelayCommand(_ => OpenCsvImportDialog());

            // Populate Books collection
            BooksView = CollectionViewSource.GetDefaultView(Books);
            BooksView.Filter = FilterBooks;

            // Debug output to confirm ViewModel is created
            System.Diagnostics.Debug.WriteLine("BooksViewModel created with ImportCsvCommand");
        }

        private void OpenAddBookDialog()
        {
            var dialog = new Views.AddBookDialog();
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                using var db = new LibraFlowContext();
                db.Books.Add(dialog.Book);
                db.SaveChanges();

                // Add the new book to the ObservableCollection to refresh the UI
                Books.Add(dialog.Book);
            }
        }

        private void OpenCsvImportDialog()
        {
            // Debug output to confirm method is called
            System.Diagnostics.Debug.WriteLine("OpenCsvImportDialog called");
            
            try
            {
                var dialog = new Views.CsvImportDialog();
                dialog.Owner = System.Windows.Application.Current.MainWindow;
                
                System.Diagnostics.Debug.WriteLine("CsvImportDialog created, showing dialog");
                
                if (dialog.ShowDialog() == true && dialog.ImportedBooks?.Any() == true)
                {
                    System.Diagnostics.Debug.WriteLine($"Dialog returned true with {dialog.ImportedBooks.Count} books");
                    
                    ImportBooksAsync(dialog.ImportedBooks);
                    
                    System.Windows.MessageBox.Show(
                        $"Successfully imported {dialog.ImportedBooks.Count} books!",
                        "Import Complete",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Dialog was cancelled or no books imported");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OpenCsvImportDialog: {ex.Message}");
                System.Windows.MessageBox.Show(
                    $"Error importing books: {ex.Message}",
                    "Import Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async Task ImportBooksAsync(List<Book> booksToImport)
        {
            await Task.Run(() =>
            {
                using var db = new LibraFlowContext();
                
                // Add books to database
                db.Books.AddRange(booksToImport);
                db.SaveChanges();
                
                // Update the ObservableCollection on the UI thread
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var book in booksToImport)
                    {
                        Books.Add(book);
                    }
                });
            });
        }

        private void EditBook(Book book)
        {
            if (book == null) return;
            var dialog = new Views.AddBookDialog(book);
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                // Update the original book with dialog.Book's values
                book.Title = dialog.Book.Title;
                book.Author = dialog.Book.Author;
                book.ISBN = dialog.Book.ISBN;
                // Update other properties as needed
                SaveChanges(book);
                OnPropertyChanged(nameof(Books));
                ReloadBooks(); // Refresh the book list after editing
            }
        }

        private bool FilterBooks(object obj)
        {
            if (obj is Book book)
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                    return true;
                return book.Title.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)
                    || book.Author.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)
                    || book.ISBN.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private void ReloadBooks()
        {
            using var db = new LibraFlowContext();
            Books.Clear();
            foreach (var book in db.Books)
                Books.Add(book);
        }

        private void SaveChanges(Book book)
        {
            using var db = new LibraFlowContext();
            var dbBook = db.Books.FirstOrDefault(b => b.Id == book.Id);
            if (dbBook != null)
            {
                dbBook.Title = book.Title;
                dbBook.Author = book.Author;
                dbBook.ISBN = book.ISBN;
                // Update other properties as needed
                db.SaveChanges();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
