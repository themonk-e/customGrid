using System.Collections.Generic;
using System.ComponentModel;

namespace CustomGridControl.Models
{
    public class ComparisonRecord : INotifyPropertyChanged
    {
        public long RecordComparisonId { get; set; }
        public long PipelineExecutionId { get; set; }
        public string SubscriberIdentifier { get; set; } = string.Empty;
        public long TotalDifferences { get; set; }
        public int? ChangedFieldsCount { get; set; }
        public bool? HasChanges { get; set; }
        public string? UserAcceptance { get; set; }
        
        // Dictionary to hold all field comparisons dynamically
        public Dictionary<string, FieldComparison> Fields { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FieldComparison : INotifyPropertyChanged
    {
        private string _sourceValue = string.Empty;
        private string _destValue = string.Empty;
        private string _selectedValue = string.Empty;
        private string _selectedSource = string.Empty;
        private CellState _cellState = CellState.Pending;

        public string FieldName { get; set; } = string.Empty;
        
        public string SourceValue
        {
            get => _sourceValue;
            set
            {
                _sourceValue = value ?? string.Empty;
                OnPropertyChanged(nameof(SourceValue));
            }
        }

        public string DestValue
        {
            get => _destValue;
            set
            {
                _destValue = value ?? string.Empty;
                OnPropertyChanged(nameof(DestValue));
            }
        }

        public string SelectedValue
        {
            get => _selectedValue;
            set
            {
                _selectedValue = value ?? string.Empty;
                OnPropertyChanged(nameof(SelectedValue));
            }
        }

        public string SelectedSource
        {
            get => _selectedSource;
            set
            {
                _selectedSource = value ?? string.Empty;
                OnPropertyChanged(nameof(SelectedSource));
            }
        }

        public bool HasDifference => !string.Equals(SourceValue, DestValue, System.StringComparison.Ordinal);

        public CellState CellState
        {
            get => _cellState;
            set
            {
                _cellState = value;
                OnPropertyChanged(nameof(CellState));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
