using System.Windows;
using System.Windows.Controls;

namespace CustomGridControl
{
    public partial class CustomDataGrid : UserControl
    {
        public CustomDataGrid()
        {
            InitializeComponent();
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