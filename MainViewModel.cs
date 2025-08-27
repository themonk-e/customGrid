using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;

namespace CustomGridControl
{
    public class DynamicGridRowData : INotifyPropertyChanged
    {
        private Dictionary<string, GridCellData> _cells = new Dictionary<string, GridCellData>();
        
        public Dictionary<string, GridCellData> Cells => _cells;
        
        public GridCellData this[string columnName]
        {
            get => _cells.TryGetValue(columnName, out var cell) ? cell : new GridCellData { NewValue = "Empty", CellType = CellType.ReadOnly };
            set
            {
                _cells[columnName] = value;
                OnPropertyChanged($"Cells[{columnName}]");
            }
        }
        
        public void SetCell(string columnName, GridCellData cellData)
        {
            _cells[columnName] = cellData;
            OnPropertyChanged($"Cells[{columnName}]");
        }
        
        public GridCellData GetCell(string columnName)
        {
            return _cells.ContainsKey(columnName) ? _cells[columnName] : new GridCellData();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class GridColumnDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
        public double Width { get; set; } = 150;
        public string UniqueName { get; set; } = string.Empty;
    }

    // Keep original for backward compatibility
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
        public ObservableCollection<DynamicGridRowData> DynamicGridData { get; set; }
        public ObservableCollection<GridColumnDefinition> ColumnDefinitions { get; set; }

        public MainViewModel()
        {
            GridData = new ObservableCollection<GridRowData>();
            DynamicGridData = new ObservableCollection<DynamicGridRowData>();
            ColumnDefinitions = new ObservableCollection<GridColumnDefinition>();
            
            // Generate sample data with the original 5 columns for testing
            GenerateSampleData();
            GenerateSimpleTestData(); // Start with simple test data
        }

        private void GenerateSampleData()
        {
            string[] firstNames = { "John", "Jane", "Bob", "Alice", "Charlie", "Diana", "Edward", "Fiona", "George", "Helen" };
            string[] lastNames = { "Smith", "Doe", "Wilson", "Johnson", "Brown", "Davis", "Miller", "Garcia", "Martinez", "Anderson" };
            string[] domains = { "gmail.com", "yahoo.com", "outlook.com", "company.com", "corp.com", "enterprise.com", "business.com" };
            string[] streets = { "Main St", "Oak Ave", "Pine St", "Elm Dr", "Cedar Ln", "Maple Ave", "First St", "Second Ave", "Park Rd", "Hill St" };
            string[] positions = { "Developer", "Manager", "Analyst", "Designer", "Engineer", "Architect", "Coordinator", "Specialist", "Director", "Consultant" };

            var random = new Random();

            for (int i = 0; i < 100; i++)
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

        private void GenerateDynamicSampleData(int columnCount)
        {
            // Define column types for realistic data generation
            var columnTypes = new string[] 
            {
                "Name", "Email", "Address", "Phone", "Position", "Department", "City", "Country", 
                "Salary", "Age", "Experience", "Skills", "Education", "Manager", "StartDate",
                "EndDate", "Status", "Rating", "Bonus", "Commission", "Territory", "Region",
                "Product", "Category", "Supplier", "Customer", "Order", "Quantity", "Price", "Discount"
            };

            var sampleValues = new Dictionary<string, string[]>
            {
                ["Name"] = new[] { "John Smith", "Jane Doe", "Bob Wilson", "Alice Johnson", "Charlie Brown" },
                ["Email"] = new[] { "john@company.com", "jane@enterprise.com", "bob@corp.com", "alice@business.com", "charlie@gmail.com" },
                ["Address"] = new[] { "123 Main St", "456 Oak Ave", "789 Pine Rd", "321 Elm Dr", "654 Cedar Ln" },
                ["Phone"] = new[] { "555-1234", "555-5678", "555-9012", "555-3456", "555-7890" },
                ["Position"] = new[] { "Developer", "Manager", "Analyst", "Designer", "Engineer" },
                ["Department"] = new[] { "IT", "Sales", "Marketing", "HR", "Finance" },
                ["City"] = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix" },
                ["Country"] = new[] { "USA", "Canada", "UK", "Germany", "France" },
                ["Default"] = new[] { "Value 1", "Value 2", "Value 3", "Value 4", "Value 5" }
            };

            // Generate column definitions
            ColumnDefinitions.Clear();
            for (int i = 0; i < columnCount; i++)
            {
                string columnType = columnTypes[i % columnTypes.Length];
                var columnDef = new GridColumnDefinition
                {
                    Name = $"Column{i}",
                    Header = i < 5 ? new[] { "Full Name", "Email Address", "Home Address", "Phone Number", "Job Title" }[i] : $"{columnType} {i - 4}",
                    Width = columnType == "Address" ? 250 : columnType == "Email" ? 200 : columnType == "Name" ? 180 : 150,
                    UniqueName = $"Column{i}"
                };
                ColumnDefinitions.Add(columnDef);
            }

            // Generate dynamic row data
            var random = new Random();
            DynamicGridData.Clear();

            for (int row = 0; row < 100; row++)
            {
                var rowData = new DynamicGridRowData();

                for (int col = 0; col < columnCount; col++)
                {
                    string columnType = columnTypes[col % columnTypes.Length];
                    var values = sampleValues.ContainsKey(columnType) ? sampleValues[columnType] : sampleValues["Default"];
                    
                    // Create a better mix: roughly 60% editable, 40% readonly
                    bool isEditable = (row * columnCount + col) % 5 != 0; // 4 out of 5 cells are editable
                    
                    var cellData = new GridCellData();
                    
                    if (isEditable)
                    {
                        // Editable cell with old and new values
                        cellData.OldValue = values[random.Next(values.Length)];
                        cellData.NewValue = GenerateNewValue(values[random.Next(values.Length)], columnType, random);
                        cellData.CellType = CellType.Editable;
                        
                        // Add some variety in cell states for testing
                        if (row == 0 && col < 3) // First row, first 3 columns
                        {
                            cellData.CellState = col switch
                            {
                                0 => CellState.Pending,  // First column: Pending (pale yellow)
                                1 => CellState.Accepted, // Second column: Accepted (green) 
                                2 => CellState.Rejected, // Third column: Rejected (red)
                                _ => CellState.Pending
                            };
                        }
                        else
                        {
                            cellData.CellState = CellState.Pending; // Default to pending
                        }
                    }
                    else
                    {
                        // ReadOnly cell - only new value, no old value
                        cellData.OldValue = "";
                        cellData.NewValue = values[random.Next(values.Length)];
                        cellData.CellType = CellType.ReadOnly;
                        cellData.CellState = CellState.Pending; // State doesn't matter for readonly
                    }

                    rowData.SetCell($"Column{col}", cellData);
                }

                DynamicGridData.Add(rowData);
            }
        }

        // Method to change column count dynamically (for testing with 170 columns)
        public void SetColumnCount(int columnCount)
        {
            if (columnCount <= 3)
            {
                GenerateSimpleTestData(); // Use simple test data for 3 columns
            }
            else
            {
                GenerateDynamicSampleData(columnCount); // Use full dynamic generation for larger datasets
            }
            
            OnPropertyChanged(nameof(DynamicGridData));
            OnPropertyChanged(nameof(ColumnDefinitions));
            
            // Debug output
            System.Diagnostics.Debug.WriteLine($"Generated {columnCount} columns with {DynamicGridData.Count} rows");
        }

        private void GenerateSimpleTestData()
        {
            // Create just 3 columns for testing
            ColumnDefinitions.Clear();
            ColumnDefinitions.Add(new GridColumnDefinition { Name = "Column0", Header = "Name", Width = 180 });
            ColumnDefinitions.Add(new GridColumnDefinition { Name = "Column1", Header = "Email", Width = 220 });
            ColumnDefinitions.Add(new GridColumnDefinition { Name = "Column2", Header = "Phone", Width = 140 });
            
            // Create simple test data
            DynamicGridData.Clear();
            
            for (int i = 0; i < 10; i++)
            {
                var row = new DynamicGridRowData();
                
                // Create different cell states for demonstration
                CellState nameState = i switch
                {
                    0 => CellState.Pending,   // First row - pending (pale yellow)
                    1 => CellState.Accepted,  // Second row - accepted (green)
                    2 => CellState.Rejected,  // Third row - rejected (red)
                    _ => CellState.Pending    // Others - pending
                };
                
                CellState emailState = (i + 1) switch
                {
                    1 => CellState.Pending,
                    2 => CellState.Accepted,
                    3 => CellState.Rejected,
                    _ => CellState.Pending
                };
                
                row.SetCell("Column0", new GridCellData 
                { 
                    OldValue = $"John Smith {i}", 
                    NewValue = $"Jonathan Smith {i}", 
                    CellType = CellType.Editable,
                    CellState = nameState
                });
                
                row.SetCell("Column1", new GridCellData 
                { 
                    OldValue = $"john{i}@old.com", 
                    NewValue = $"jonathan{i}@new.com", 
                    CellType = CellType.Editable,
                    CellState = emailState
                });
                
                // Some phone cells are readonly, some editable
                bool isPhoneEditable = i % 3 == 0; // Every 3rd phone is editable
                
                row.SetCell("Column2", new GridCellData 
                { 
                    OldValue = isPhoneEditable ? $"555-{i:D3}-OLD" : "", 
                    NewValue = isPhoneEditable ? $"555-{i:D3}-NEW" : $"555-{i:D4}", 
                    CellType = isPhoneEditable ? CellType.Editable : CellType.ReadOnly,
                    CellState = CellState.Pending
                });
                
                DynamicGridData.Add(row);
            }
        }

        private string GenerateNewValue(string baseValue, string columnType, Random random)
        {
            return columnType switch
            {
                "Name" => baseValue.Replace(" ", " ").Replace("John", "Jonathan").Replace("Jane", "Janet"),
                "Email" => baseValue.Replace("@", ".new@"),
                "Address" => baseValue.Replace("St", "Street").Replace("Ave", "Avenue").Replace("Dr", "Drive"),
                "Phone" => baseValue.Replace("555-", "555-NEW-"),
                "Position" => "Senior " + baseValue,
                "Department" => baseValue + " Dept",
                "City" => "New " + baseValue,
                _ => baseValue + " (Updated)"
            };
        }
    }
}