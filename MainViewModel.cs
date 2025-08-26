using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CustomGridControl
{
    public class GridRowData : INotifyPropertyChanged
    {
        public GridCellData Name { get; set; } = new();
        public GridCellData Email { get; set; } = new();
        public GridCellData Address { get; set; } = new();
        public GridCellData Phone { get; set; } = new();
        public GridCellData Position { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<GridRowData> GridData { get; set; }

        public MainViewModel()
        {
            GridData = new ObservableCollection<GridRowData>();
            GenerateSampleData();
        }

        private void GenerateSampleData()
        {
            string[] firstNames = { "John", "Jane", "Bob", "Alice", "Charlie", "Diana", "Edward", "Fiona", "George", "Helen" };
            string[] lastNames = { "Smith", "Doe", "Wilson", "Johnson", "Brown", "Davis", "Miller", "Garcia", "Martinez", "Anderson" };
            string[] domains = { "gmail.com", "yahoo.com", "outlook.com", "company.com", "corp.com", "enterprise.com", "business.com" };
            string[] streets = { "Main St", "Oak Ave", "Pine St", "Elm Dr", "Cedar Ln", "Maple Ave", "First St", "Second Ave", "Park Rd", "Hill St" };
            string[] positions = { "Developer", "Manager", "Analyst", "Designer", "Engineer", "Architect", "Coordinator", "Specialist", "Director", "Consultant" };

            var random = new Random();

            for (int i = 0; i < 50; i++)
            {
                string firstName = firstNames[random.Next(firstNames.Length)];
                string lastName = lastNames[random.Next(lastNames.Length)];
                string domain = domains[random.Next(domains.Length)];
                string street = streets[random.Next(streets.Length)];
                string position = positions[random.Next(positions.Length)];
                
                // Mix of editable and readonly cells
                bool isNameEditable = i % 3 == 0; // Every 3rd row has editable name
                bool isEmailEditable = i % 4 == 0; // Every 4th row has editable email
                bool isAddressEditable = i % 5 == 0; // Every 5th row has editable address
                bool isPhoneEditable = i % 2 == 0; // Every 2nd row has editable phone
                bool isPositionEditable = i % 6 == 0; // Every 6th row has editable position

                var row = new GridRowData
                {
                    Name = new GridCellData
                    {
                        OldValue = isNameEditable ? $"{firstName} {lastName}" : "",
                        NewValue = isNameEditable ? $"{firstName} {lastName[0]}. {lastName}" : $"{firstName} {lastName}",
                        CellType = isNameEditable ? CellType.Editable : CellType.ReadOnly
                    },
                    Email = new GridCellData
                    {
                        OldValue = isEmailEditable ? $"{firstName.ToLower()}@old{domain}" : "",
                        NewValue = isEmailEditable ? $"{firstName.ToLower()}.{lastName.ToLower()}@{domain}" : $"{firstName.ToLower()}.{lastName.ToLower()}@{domain}",
                        CellType = isEmailEditable ? CellType.Editable : CellType.ReadOnly
                    },
                    Address = new GridCellData
                    {
                        OldValue = isAddressEditable ? $"{random.Next(100, 999)} {street}" : "",
                        NewValue = isAddressEditable ? $"{random.Next(100, 999)} {street.Replace("St", "Street").Replace("Ave", "Avenue").Replace("Dr", "Drive").Replace("Ln", "Lane").Replace("Rd", "Road")}" : $"{random.Next(100, 999)} {street}",
                        CellType = isAddressEditable ? CellType.Editable : CellType.ReadOnly
                    },
                    Phone = new GridCellData
                    {
                        OldValue = isPhoneEditable ? $"555-{random.Next(1000, 9999)}" : "",
                        NewValue = isPhoneEditable ? $"555-{random.Next(100, 999)}-{random.Next(1000, 9999)}" : $"555-{random.Next(100, 999)}-{random.Next(1000, 9999)}",
                        CellType = isPhoneEditable ? CellType.Editable : CellType.ReadOnly
                    },
                    Position = new GridCellData
                    {
                        OldValue = isPositionEditable ? position : "",
                        NewValue = isPositionEditable ? $"Senior {position}" : position,
                        CellType = isPositionEditable ? CellType.Editable : CellType.ReadOnly
                    }
                };

                GridData.Add(row);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}