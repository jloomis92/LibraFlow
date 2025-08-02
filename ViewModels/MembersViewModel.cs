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
        public ICommand ImportCsvCommand { get; }

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
            ImportCsvCommand = new RelayCommand(_ => OpenCsvImportDialog());
        }

        private void OpenAddMemberDialog()
        {
            var dialog = new Views.AddMemberDialog();
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                using var db = new LibraFlowContext();
                db.Members.Add(dialog.Member);
                db.SaveChanges();

                Members.Add(dialog.Member);
            }
        }

        private void OpenCsvImportDialog()
        {
            try
            {
                var dialog = new Views.MembersCsvImportDialog();
                dialog.Owner = System.Windows.Application.Current.MainWindow;
                
                if (dialog.ShowDialog() == true && dialog.ImportedMembers?.Any() == true)
                {
                    ImportMembersAsync(dialog.ImportedMembers);
                    
                    System.Windows.MessageBox.Show(
                        $"Successfully imported {dialog.ImportedMembers.Count} members!",
                        "Import Complete",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Error importing members: {ex.Message}",
                    "Import Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async Task ImportMembersAsync(List<Member> membersToImport)
        {
            await Task.Run(() =>
            {
                using var db = new LibraFlowContext();
                
                // Add members to database
                db.Members.AddRange(membersToImport);
                db.SaveChanges();
                
                // Update the ObservableCollection on the UI thread
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var member in membersToImport)
                    {
                        Members.Add(member);
                    }
                });
            });
        }

        private void EditMember(Member member)
        {
            if (member == null) return;
            var dialog = new Views.AddMemberDialog(member);
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                using var db = new LibraFlowContext();
                var dbMember = db.Members.Find(member.Id);
                if (dbMember != null)
                {
                    dbMember.Name = dialog.Member.Name;
                    dbMember.Email = dialog.Member.Email;
                    db.SaveChanges();
                }
                // Update the in-memory collection
                member.Name = dialog.Member.Name;
                member.Email = dialog.Member.Email;
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
