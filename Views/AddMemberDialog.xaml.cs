using System.Windows;
using LibraFlow.Models;
using LibraFlow.Helpers; // Add this

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
            ThemeManager.ApplyTheme(this); // Apply theme
            ThemeManager.ThemeChanged += OnThemeChanged; // Listen for theme changes
            Member = new Member();
            DataContext = Member;
        }

        // For editing an existing member
        public AddMemberDialog(Member member)
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this); // Apply theme
            ThemeManager.ThemeChanged += OnThemeChanged; // Listen for theme changes
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

        private void OnThemeChanged()
        {
            ThemeManager.ApplyTheme(this);
        }

        protected override void OnClosed(System.EventArgs e)
        {
            ThemeManager.ThemeChanged -= OnThemeChanged;
            base.OnClosed(e);
        }
    }
}
