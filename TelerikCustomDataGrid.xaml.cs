using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls.GridView;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;

namespace CustomGridControl
{
    public partial class TelerikCustomDataGrid : UserControl
    {
        private MainViewModel _viewModel;

        public TelerikCustomDataGrid()
        {
            InitializeComponent();
            
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            
            // Generate dynamic columns after the control is loaded
            Loaded += TelerikCustomDataGrid_Loaded;
        }

        private void TelerikCustomDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateDynamicColumns();
        }

        private void GenerateDynamicColumns()
        {
            // Clear existing columns
            MainRadGridView.Columns.Clear();
            
            // Create columns dynamically based on ColumnDefinitions
            foreach (var columnDef in _viewModel.ColumnDefinitions)
            {
                var column = new GridViewDataColumn
                {
                    Header = columnDef.Header,
                    Width = new GridViewLength(columnDef.Width),
                    UniqueName = columnDef.UniqueName,
                    DataMemberBinding = new Binding($"Cells[{columnDef.Name}].NewValue") // Simple binding for now to avoid crashes
                };
                
                MainRadGridView.Columns.Add(column);
            }
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

        // Method to dynamically change column count (for testing)
        public void SetColumnCount(int columnCount)
        {
            _viewModel.SetColumnCount(columnCount);
            GenerateDynamicColumns();
        }
    }
}