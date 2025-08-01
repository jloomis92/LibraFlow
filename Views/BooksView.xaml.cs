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
using LibraFlow.Data;
using LibraFlow.Models;

namespace LibraFlow.Views
{
    /// <summary>
    /// Interaction logic for BooksView.xaml
    /// </summary>
    public partial class BooksView : System.Windows.Controls.UserControl
    {
        public BooksView()
        {
            InitializeComponent();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddBookDialog();
            if (dialog.ShowDialog() == true)
            {
                using (var db = new LibraFlowContext())
                {
                    db.Books.Add(dialog.Book);
                    db.SaveChanges();
                }
                // Optionally, refresh your books list here if you display it in the UI
            }
        }
    }
}
