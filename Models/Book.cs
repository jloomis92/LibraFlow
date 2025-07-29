using System;
using System.ComponentModel;

namespace LibraFlow.Models
{
    public class Book : INotifyPropertyChanged
    {
        private int _id;
        private string _title;
        private string _author;
        private string _isbn;
        private bool _isCheckedOut;

        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string Author
        {
            get => _author;
            set
            {
                if (_author != value)
                {
                    _author = value;
                    OnPropertyChanged(nameof(Author));
                }
            }
        }

        public string ISBN
        {
            get => _isbn;
            set
            {
                if (_isbn != value)
                {
                    _isbn = value;
                    OnPropertyChanged(nameof(ISBN));
                }
            }
        }

        public bool IsCheckedOut
        {
            get => _isCheckedOut;
            set
            {
                if (_isCheckedOut != value)
                {
                    _isCheckedOut = value;
                    OnPropertyChanged(nameof(IsCheckedOut));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
