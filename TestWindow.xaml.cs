using System;
using System.Windows;

namespace CustomGridControl
{
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        private void UpdateColumnsButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ColumnCountTextBox.Text, out int columnCount))
            {
                if (columnCount > 0 && columnCount <= 1000) // Reasonable limit
                {
                    StatusTextBlock.Text = $"Updating to {columnCount} columns...";
                    StatusTextBlock.Foreground = System.Windows.Media.Brushes.Orange;
                    
                    // Use Dispatcher to update UI
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            DynamicGrid.SetColumnCount(columnCount);
                            StatusTextBlock.Text = $"Successfully loaded {columnCount} columns";
                            StatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                        }
                        catch (Exception ex)
                        {
                            StatusTextBlock.Text = $"Error: {ex.Message}";
                            StatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                        }
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
                else
                {
                    StatusTextBlock.Text = "Column count must be between 1 and 1000";
                    StatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
            else
            {
                StatusTextBlock.Text = "Invalid column count";
                StatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
    }
}