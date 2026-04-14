using System.Windows;
using WineTracker.ViewModels;

namespace WineTracker.Views
{
    public partial class WineEditWindow : Window
    {
        public WineEditWindow(WineEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
