using System.Windows;


namespace FinanceTracker.Graphics.Dialogs
{
    public partial class AddCategoryDialog : Window
    {
        public string? CategoryName { get; private set; }

        public AddCategoryDialog(MainWindow mainWindow)
        {
            InitializeComponent();
            this.Icon = mainWindow.Icon;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryName = CategoryNameTextBox.Text.Trim();
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
