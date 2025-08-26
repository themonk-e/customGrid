using System.ComponentModel;

namespace CustomGridControl
{
    public class GridCellData : INotifyPropertyChanged
    {
        private string _oldValue = string.Empty;
        private string _newValue = string.Empty;
        private CellState _cellState = CellState.Pending;
        private CellType _cellType = CellType.Editable;

        public string OldValue
        {
            get => _oldValue;
            set
            {
                _oldValue = value;
                OnPropertyChanged(nameof(OldValue));
            }
        }

        public string NewValue
        {
            get => _newValue;
            set
            {
                _newValue = value;
                OnPropertyChanged(nameof(NewValue));
            }
        }

        public CellState CellState
        {
            get => _cellState;
            set
            {
                _cellState = value;
                OnPropertyChanged(nameof(CellState));
            }
        }

        public CellType CellType
        {
            get => _cellType;
            set
            {
                _cellType = value;
                OnPropertyChanged(nameof(CellType));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum CellState
    {
        Pending,
        Accepted,
        Rejected
    }

    public enum CellType
    {
        Editable,      // Has old/new values and buttons - pale yellow
        ReadOnly       // Simple value only - white
    }
}