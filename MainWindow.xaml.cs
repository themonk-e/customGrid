using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;

namespace CustomGridControl;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Test170ColumnsButton_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Loading 170 columns...";
        StatusText.Foreground = Brushes.Orange;
        
        // Use Dispatcher to update UI
        Dispatcher.BeginInvoke(new Action(() =>
        {
            try
            {
                MainGrid.SetColumnCount(170);
                StatusText.Text = "Current: 170 columns";
                StatusText.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
                StatusText.Foreground = Brushes.Red;
            }
        }), System.Windows.Threading.DispatcherPriority.Background);
    }

    private void Test5ColumnsButton_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Resetting to 5 columns...";
        StatusText.Foreground = Brushes.Orange;
        
        // Use Dispatcher to update UI
        Dispatcher.BeginInvoke(new Action(() =>
        {
            try
            {
                MainGrid.SetColumnCount(5);
                StatusText.Text = "Current: 5 columns";
                StatusText.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
                StatusText.Foreground = Brushes.Red;
            }
        }), System.Windows.Threading.DispatcherPriority.Background);
    }
}