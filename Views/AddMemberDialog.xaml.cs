using System.Windows;
using LibraFlow.Models;

namespace LibraFlow.Views
{
    /// <summary>
    /// Interaction logic for AddMemberDialog.xaml
    /// </summary>
    public partial class AddMemberDialog : Window
    {
        public Member Member { get; private set; }

        // For adding a new member
        public AddMemberDialog()
        {
            InitializeComponent();
            Member = new Member();
            DataContext = Member;
        }

        // For editing an existing member
        public AddMemberDialog(Member member)
        {
            InitializeComponent();
            // Create a copy so changes can be cancelled if needed
            Member = new Member
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email
            };
            DataContext = Member;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
