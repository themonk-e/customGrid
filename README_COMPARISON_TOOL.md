# Data Comparison Tool

## Overview
This WPF application compares source and destination data from the `TransactionalizeResultsMemberMasters` table and allows users to approve or reject changes through an interactive grid interface.

## Features

### 1. **Database Connection**
- Supports both Windows Authentication and SQL Authentication
- Connection configuration dialog on startup
- Test connection before proceeding

### 2. **Dynamic Column Generation**
- Automatically detects all fields from the database table
- Creates columns based on the `FieldName_Source`, `FieldName_Dest`, `FieldName_Selected` pattern
- Only shows comparison UI for fields with differences

### 3. **Comparison Workflow**
- **Yellow cells**: Pending changes (Source ≠ Dest)
- **Green cells**: Accepted changes (user clicked ✓)
- **Red cells**: Rejected changes (user clicked ✗)
- **White cells**: No changes (Source = Dest)

### 4. **User Actions**
- **✓ (Accept)**: Choose the source value, cell turns green
- **✗ (Reject)**: Keep the destination value, cell turns red
- Changes are tracked in `FieldName_Selected` and `FieldName_SelectedSource` columns

### 5. **Data Persistence**
- Save button updates all user selections back to the database
- Uses transactions for data integrity
- Only updates fields that have differences

## Architecture

### Key Components

**Models/**
- `ComparisonRecord.cs` - Represents a single row with metadata and field comparisons
- `FieldComparison.cs` - Represents a single field comparison (Source, Dest, Selected values)

**Services/**
- `DatabaseService.cs` - ADO.NET data access layer
  - `LoadComparisonRecordsAsync()` - Loads records from database
  - `GetFieldNamesAsync()` - Extracts field names from schema
  - `SaveUserSelectionsAsync()` - Persists user selections

**ViewModels/**
- `ComparisonViewModel.cs` - Business logic and data binding
  - `LoadDataAsync()` - Loads data and field names
  - `SaveChangesAsync()` - Saves changes to database
  - `AcceptChange()` - Marks field as accepted
  - `RejectChange()` - Marks field as rejected

**Views/**
- `ComparisonGridView.xaml` - Main grid UI with Telerik RadGridView
- `ConnectionConfigWindow.xaml` - Database connection configuration

**Configuration/**
- `AppSettings.cs` - Application settings (can be extended)

## Database Schema Requirements

The table must follow this naming convention:

```sql
CREATE TABLE TransactionalizeResultsMemberMasters (
    RecordComparisonId INT PRIMARY KEY,
    PipelineExecutionId INT,
    SubscriberIdentifier NVARCHAR(50),
    TotalDifferences INT,
    ChangedFieldsCount INT,
    HasChanges BIT,
    UserAcceptance NVARCHAR(50),
    
    -- For each field being compared:
    FieldName_Source NVARCHAR(MAX),
    FieldName_Dest NVARCHAR(MAX),
    FieldName_Selected NVARCHAR(MAX),
    FieldName_SelectedSource NVARCHAR(10),
    
    -- Repeat for all fields...
)
```

## Usage

### 1. **Configure Connection**
- Run the application
- Enter your SQL Server name and database name
- Choose authentication method
- Test connection
- Click "Save & Continue"

### 2. **Load Data**
- Click "Load Data" button
- Grid will populate with comparison records
- Columns are generated dynamically based on database schema

### 3. **Review Changes**
- Scroll through the grid
- Yellow cells indicate pending changes
- Old value is shown with strikethrough
- New value is shown below

### 4. **Approve/Reject Changes**
- Click ✓ to accept the new value (source)
- Click ✗ to reject and keep the old value (destination)
- Cell color changes to indicate your choice

### 5. **Save Changes**
- Click "Save Changes" button
- All selections are persisted to the database
- Success message confirms save

## Customization

### Filter by Pipeline Execution
Modify `ComparisonGridView_Loaded` to pass a specific pipeline ID:

```csharp
await _viewModel.LoadDataAsync(pipelineExecutionId: 12345);
```

### Add Custom Columns
Edit `GenerateColumns()` in `ComparisonGridView.xaml.cs` to add additional fixed columns.

### Change Cell Colors
Modify `CellStateToBackgroundConverter.cs` to customize colors:
- Pending: Light yellow
- Accepted: Light green
- Rejected: Light coral
- ReadOnly: White

### Exclude Fields
Modify `GetFieldNamesFromSchema()` in `DatabaseService.cs` to filter out specific fields.

## Error Handling

- Connection failures show error dialog
- Load failures display error message in status bar
- Save failures rollback transaction and show error
- All database operations are async for responsiveness

## Performance Considerations

- Uses ADO.NET for optimal performance
- Async/await for non-blocking UI
- Transaction-based saves for data integrity
- Dynamic column generation reduces memory footprint

## Future Enhancements

- [ ] Add filtering and sorting capabilities
- [ ] Export to Excel functionality
- [ ] Bulk accept/reject operations
- [ ] Audit log of user actions
- [ ] Undo/redo functionality
- [ ] Search across all fields
- [ ] Column visibility toggle
- [ ] Save connection string to config file
- [ ] Multi-user conflict resolution
- [ ] Real-time collaboration

## Troubleshooting

**Issue**: Columns not showing
- Verify table exists and has correct naming pattern
- Check that fields end with `_Source`, `_Dest`, `_Selected`, `_SelectedSource`

**Issue**: Connection fails
- Verify SQL Server is running
- Check firewall settings
- Ensure user has appropriate permissions
- Try using SQL Server Management Studio to test connection

**Issue**: Save fails
- Check user has UPDATE permissions on table
- Verify no other process has locked the records
- Check transaction log space

**Issue**: Slow performance
- Add indexes on RecordComparisonId and PipelineExecutionId
- Consider pagination for large datasets
- Optimize network connection to database
