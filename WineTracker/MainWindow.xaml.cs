using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace WineTracker;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        }
        catch { /* ignore navigation errors */ }
        e.Handled = true;
    }
}
