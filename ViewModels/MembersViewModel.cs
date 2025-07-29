using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;
using LibraFlow.Helpers;
using LibraFlow.Data;
using LibraFlow.Models;

namespace LibraFlow.ViewModels
{
    public class MembersViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Member> Members { get; set; }
        public ICollectionView MembersView { get; }

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
                    MembersView.Refresh();
                }
            }
        }

        public ICommand AddMemberCommand { get; }
        public ICommand EditMemberCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private Member _selectedMember;
        public Member SelectedMember
        {
            get => _selectedMember;
            set
            {
                if (_selectedMember != value)
                {
                    _selectedMember = value;
                    OnPropertyChanged(nameof(SelectedMember));
                }
            }
        }

        public MembersViewModel()
        {
            using var db = new LibraFlowContext();
            Members = new ObservableCollection<Member>(db.Members.ToList());
            MembersView = CollectionViewSource.GetDefaultView(Members);
            MembersView.Filter = FilterMembers;

            AddMemberCommand = new RelayCommand(_ => OpenAddMemberDialog());
            EditMemberCommand = new RelayCommand(param => EditMember(param as Member));
        }

        private void OpenAddMemberDialog()
        {
            var dialog = new Views.AddMemberDialog();
            dialog.Owner = Application.Current.MainWindow; // Set the owner
            if (dialog.ShowDialog() == true)
            {
                using var db = new LibraFlowContext();
                db.Members.Add(dialog.Member);
                db.SaveChanges();

                Members.Add(dialog.Member);
            }
        }

        private void EditMember(Member member)
        {
            if (member == null) return;
            var dialog = new Views.AddMemberDialog(member);
            dialog.Owner = Application.Current.MainWindow; // Set the owner
            if (dialog.ShowDialog() == true)
            {
                // Copy values from dialog.Member back to the original member
                member.Name = dialog.Member.Name;
                member.Email = dialog.Member.Email;
                SaveChanges();
                OnPropertyChanged(nameof(Members));
                MembersView.Refresh();
            }
        }

        private void ReloadMembers()
        {
            using var db = new LibraFlowContext();
            Members.Clear();
            foreach (var member in db.Members)
                Members.Add(member);
        }

        private void SaveChanges()
        {
            using var db = new LibraFlowContext();
            db.SaveChanges();
        }

        private bool FilterMembers(object obj)
        {
            if (obj is Member member)
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                    return true;

                // Filter by Name, Email, or other relevant fields
                return (member.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (member.Email?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            return false;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
