using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System;
using LibraFlow.Models;
using LibraFlow.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraFlow.ViewModels
{
    public class LoansViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Loan> Loans { get; set; }
        public ICollectionView LoansView { get; }
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
                    LoansView.Refresh();
                }
            }
        }

        public LoansViewModel()
        {
            using var db = new LibraFlowContext();
            Loans = new ObservableCollection<Loan>(
                db.Loans.Include(l => l.Book).Include(l => l.Member).ToList()
            );
            LoansView = CollectionViewSource.GetDefaultView(Loans);
            LoansView.Filter = FilterLoans;
        }

        private bool FilterLoans(object obj)
        {
            if (obj is Loan loan)
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                    return true;

                // Filter by Book title, Member name, or LoanDate
                return (loan.Book?.Title?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (loan.Member?.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)
                    || loan.CheckedOutDate.ToString("d").Contains(SearchText, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
