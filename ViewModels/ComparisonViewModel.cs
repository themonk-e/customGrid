using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CustomGridControl.Models;
using CustomGridControl.Services;

namespace CustomGridControl.ViewModels
{
    public class ComparisonViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private bool _isLoading;
        private string _statusMessage = string.Empty;

        public ObservableCollection<ComparisonRecord> ComparisonRecords { get; set; }
        public ObservableCollection<string> FieldNames { get; set; }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public ComparisonViewModel(string connectionString)
        {
            _databaseService = new DatabaseService(connectionString);
            ComparisonRecords = new ObservableCollection<ComparisonRecord>();
            FieldNames = new ObservableCollection<string>();
        }

        public async Task LoadDataAsync(int? pipelineExecutionId = null)
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading comparison data...";

                // Load field names first
                var fieldNames = await _databaseService.GetFieldNamesAsync();
                FieldNames.Clear();
                foreach (var fieldName in fieldNames)
                {
                    FieldNames.Add(fieldName);
                }

                // Load records
                var records = await _databaseService.LoadComparisonRecordsAsync(pipelineExecutionId);
                ComparisonRecords.Clear();
                foreach (var record in records)
                {
                    ComparisonRecords.Add(record);
                }

                StatusMessage = $"Loaded {records.Count} records with {fieldNames.Count} fields";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading data: {ex.Message}";
                MessageBox.Show($"Failed to load data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Saving changes...";

                await _databaseService.SaveUserSelectionsAsync(ComparisonRecords.ToList());

                StatusMessage = "Changes saved successfully";
                MessageBox.Show("Changes saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving changes: {ex.Message}";
                MessageBox.Show($"Failed to save changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void AcceptChange(FieldComparison field)
        {
            field.SelectedValue = field.SourceValue;
            field.SelectedSource = "Source";
            field.CellState = CellState.Accepted;
        }

        public void RejectChange(FieldComparison field)
        {
            field.SelectedValue = field.DestValue;
            field.SelectedSource = "Dest";
            field.CellState = CellState.Rejected;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
