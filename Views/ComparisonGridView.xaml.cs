using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CustomGridControl.Models;
using CustomGridControl.ViewModels;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace CustomGridControl.Views
{
    public partial class ComparisonGridView : UserControl
    {
        private ComparisonViewModel? _viewModel;

        public ComparisonGridView()
        {
            InitializeComponent();
            Loaded += ComparisonGridView_Loaded;
        }

        private void ComparisonGridView_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize with empty view model - user will click Load Data button
            // This prevents blocking the UI thread on startup
        }

        private void GenerateColumns()
        {
            if (_viewModel == null) return;

            // Clear existing columns to avoid duplication
            ComparisonGrid.Columns.Clear();

            // Add fixed columns
            ComparisonGrid.Columns.Add(new GridViewDataColumn
            {
                Header = "Record ID",
                DataMemberBinding = new Binding("RecordComparisonId"),
                Width = new GridViewLength(100),
                IsReadOnly = true
            });

            ComparisonGrid.Columns.Add(new GridViewDataColumn
            {
                Header = "Subscriber ID",
                DataMemberBinding = new Binding("SubscriberIdentifier"),
                Width = new GridViewLength(150),
                IsReadOnly = true
            });

            // Add dynamic field columns
            foreach (var fieldName in _viewModel.FieldNames)
            {
                var column = new GridViewDataColumn
                {
                    Header = FormatFieldName(fieldName),
                    Width = new GridViewLength(200),
                    IsReadOnly = true,
                    CellTemplate = CreateFieldCellTemplate(fieldName)
                };

                ComparisonGrid.Columns.Add(column);
            }
        }

        private DataTemplate CreateFieldCellTemplate(string fieldName)
        {
            var template = new DataTemplate();

            // Build the entire visual tree structure first
            var factory = new FrameworkElementFactory(typeof(Border));

            var gridFactory = new FrameworkElementFactory(typeof(Grid));

            // Row definitions
            var row0 = new FrameworkElementFactory(typeof(RowDefinition));
            var row1 = new FrameworkElementFactory(typeof(RowDefinition));

            // Old value (strikethrough)
            var oldValueFactory = new FrameworkElementFactory(typeof(TextBlock));

            // New value row with buttons
            var newValueGridFactory = new FrameworkElementFactory(typeof(Grid));

            var col0 = new FrameworkElementFactory(typeof(ColumnDefinition));
            var col1 = new FrameworkElementFactory(typeof(ColumnDefinition));

            // New value text
            var newValueFactory = new FrameworkElementFactory(typeof(TextBlock));

            // Buttons panel
            var buttonsPanelFactory = new FrameworkElementFactory(typeof(StackPanel));

            // Accept button
            var acceptButtonFactory = new FrameworkElementFactory(typeof(Button));

            // Reject button
            var rejectButtonFactory = new FrameworkElementFactory(typeof(Button));

            // Now set all properties BEFORE appending children
            factory.SetValue(Border.BorderBrushProperty, System.Windows.Media.Brushes.LightGray);
            factory.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            factory.SetValue(Border.PaddingProperty, new Thickness(5));

            row0.SetValue(RowDefinition.HeightProperty, new GridLength(1, GridUnitType.Auto));
            row1.SetValue(RowDefinition.HeightProperty, new GridLength(1, GridUnitType.Auto));

            oldValueFactory.SetValue(Grid.RowProperty, 0);
            oldValueFactory.SetValue(TextBlock.ForegroundProperty, System.Windows.Media.Brushes.Gray);
            oldValueFactory.SetValue(TextBlock.TextDecorationsProperty, TextDecorations.Strikethrough);
            oldValueFactory.SetValue(TextBlock.MarginProperty, new Thickness(2));

            newValueGridFactory.SetValue(Grid.RowProperty, 1);

            col0.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
            col1.SetValue(ColumnDefinition.WidthProperty, GridLength.Auto);

            newValueFactory.SetValue(Grid.ColumnProperty, 0);
            newValueFactory.SetValue(TextBlock.MarginProperty, new Thickness(2));
            newValueFactory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);

            buttonsPanelFactory.SetValue(Grid.ColumnProperty, 1);
            buttonsPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            buttonsPanelFactory.SetValue(StackPanel.VerticalAlignmentProperty, VerticalAlignment.Center);
            buttonsPanelFactory.SetValue(StackPanel.MarginProperty, new Thickness(5, 0, 0, 0));

            // Get RoundedButton style from resources
            var roundedButtonStyle = this.Resources["RoundedButton"] as Style;

            acceptButtonFactory.SetValue(Button.ContentProperty, "✓");
            acceptButtonFactory.SetValue(Button.StyleProperty, roundedButtonStyle);
            acceptButtonFactory.SetValue(Button.BackgroundProperty, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 175, 80)));
            acceptButtonFactory.SetValue(Button.ForegroundProperty, System.Windows.Media.Brushes.White);

            rejectButtonFactory.SetValue(Button.ContentProperty, "✗");
            rejectButtonFactory.SetValue(Button.StyleProperty, roundedButtonStyle);
            rejectButtonFactory.SetValue(Button.BackgroundProperty, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(244, 67, 54)));
            rejectButtonFactory.SetValue(Button.ForegroundProperty, System.Windows.Media.Brushes.White);

            // Set bindings BEFORE appending
            var bgConverter = this.Resources["CellStateToBackgroundConverter"] as IMultiValueConverter;
            var oldValueConverter = this.Resources["CellStateToOldValueConverter"] as IMultiValueConverter;
            var newValueConverter = this.Resources["CellStateToNewValueConverter"] as IMultiValueConverter;

            // Background binding based on state and cell type
            var backgroundBinding = new MultiBinding { Converter = bgConverter };
            backgroundBinding.Bindings.Add(new Binding($"Fields[{fieldName}].CellState"));
            backgroundBinding.Bindings.Add(new Binding($"Fields[{fieldName}].HasDifference") { Converter = new HasDifferenceToCellTypeConverter() });

            // Old value binding - shows Dest for Pending/Accepted, Source for Rejected
            var oldValueBinding = new MultiBinding { Converter = oldValueConverter };
            oldValueBinding.Bindings.Add(new Binding($"Fields[{fieldName}].CellState"));
            oldValueBinding.Bindings.Add(new Binding($"Fields[{fieldName}].DestValue"));
            oldValueBinding.Bindings.Add(new Binding($"Fields[{fieldName}].SourceValue"));

            // New value binding - shows Source for Pending/Accepted, Dest for Rejected
            var newValueBinding = new MultiBinding { Converter = newValueConverter };
            newValueBinding.Bindings.Add(new Binding($"Fields[{fieldName}].CellState"));
            newValueBinding.Bindings.Add(new Binding($"Fields[{fieldName}].DestValue"));
            newValueBinding.Bindings.Add(new Binding($"Fields[{fieldName}].SourceValue"));

            var visibilityBinding = new Binding($"Fields[{fieldName}].HasDifference") { Converter = new System.Windows.Controls.BooleanToVisibilityConverter() };

            factory.SetBinding(Border.BackgroundProperty, backgroundBinding);
            oldValueFactory.SetBinding(TextBlock.TextProperty, oldValueBinding);
            oldValueFactory.SetBinding(TextBlock.VisibilityProperty, visibilityBinding);
            newValueFactory.SetBinding(TextBlock.TextProperty, newValueBinding);
            buttonsPanelFactory.SetBinding(StackPanel.VisibilityProperty, visibilityBinding);
            // Bind CommandParameter to the FieldComparison object
            acceptButtonFactory.SetBinding(Button.CommandParameterProperty, new Binding($"Fields[{fieldName}]"));
            rejectButtonFactory.SetBinding(Button.CommandParameterProperty, new Binding($"Fields[{fieldName}]"));

            // Bind Command to ViewModel commands using RelativeSource to find the UserControl's DataContext
            var acceptCommandBinding = new Binding("DataContext.AcceptCommand")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1)
            };
            var rejectCommandBinding = new Binding("DataContext.RejectCommand")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1)
            };

            acceptButtonFactory.SetBinding(Button.CommandProperty, acceptCommandBinding);
            rejectButtonFactory.SetBinding(Button.CommandProperty, rejectCommandBinding);

            // NOW append children in the correct hierarchy
            factory.AppendChild(gridFactory);
            gridFactory.AppendChild(row0);
            gridFactory.AppendChild(row1);
            gridFactory.AppendChild(oldValueFactory);
            gridFactory.AppendChild(newValueGridFactory);
            newValueGridFactory.AppendChild(col0);
            newValueGridFactory.AppendChild(col1);
            newValueGridFactory.AppendChild(newValueFactory);
            newValueGridFactory.AppendChild(buttonsPanelFactory);
            buttonsPanelFactory.AppendChild(acceptButtonFactory);
            buttonsPanelFactory.AppendChild(rejectButtonFactory);

            // Set the visual tree and seal the template
            template.VisualTree = factory;
            template.Seal(); // Critical: Seal the template before using it
            return template;
        }

        private string FormatFieldName(string fieldName)
        {
            // Convert PascalCase or snake_case to readable format
            return System.Text.RegularExpressions.Regex.Replace(fieldName, "([a-z])([A-Z])", "$1 $2");
        }


        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Show connection dialog if not already configured
                if (_viewModel == null)
                {
                    var configWindow = new ConnectionConfigWindow();
                    if (configWindow.ShowDialog() != true)
                    {
                        return; // User cancelled
                    }

                    _viewModel = new ComparisonViewModel(configWindow.ConnectionString);
                    DataContext = _viewModel;
                }

                // Load data
                await _viewModel.LoadDataAsync();
                GenerateColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                    "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
            {
                MessageBox.Show("No data loaded to save.", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _viewModel.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}\n\nDetails:\n{ex.InnerException?.Message ?? ex.StackTrace}", 
                    "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class HasDifferenceToCellTypeConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool hasDifference)
            {
                return hasDifference ? CellType.Editable : CellType.ReadOnly;
            }
            return CellType.ReadOnly;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
