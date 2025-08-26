using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace CustomGridControl
{
    public partial class TelerikCustomDataGrid : UserControl
    {
        public TelerikCustomDataGrid()
        {
            InitializeComponent();
            
            // Set DataContext to MainViewModel which has the sample data
            DataContext = new MainViewModel();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is GridCellData cellData)
            {
                cellData.CellState = CellState.Accepted;
            }
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is GridCellData cellData)
            {
                cellData.CellState = CellState.Rejected;
            }
        }
    }
}