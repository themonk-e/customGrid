using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CustomGridControl.Models;

namespace CustomGridControl.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected async Task<SqlConnection> GetTenantConnectionAsync(String ConnectionString)
        {
            var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            await SetTenantContextAsync(connection);
            return connection;
        }


        protected async Task SetTenantContextAsync(SqlConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

           

            var command = new SqlCommand("EXEC sys.sp_set_session_context @key = N'tenant_id', @value = @ClientId, @read_only = 1;", connection);
            command.Parameters.AddWithValue("@ClientId",1);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<ComparisonRecord>> LoadComparisonRecordsAsync(int? pipelineExecutionId = null)
        {
            var records = new List<ComparisonRecord>();

            using var connection = await GetTenantConnectionAsync(_connectionString);
   
            var query = pipelineExecutionId.HasValue
                ? "SELECT * FROM TransactionalizeResultsMemberMaster WHERE PipelineExecutionId = @PipelineExecutionId"
                : "SELECT * FROM TransactionalizeResultsMemberMaster";

            using var command = new SqlCommand(query, connection);
            if (pipelineExecutionId.HasValue)
            {
                command.Parameters.AddWithValue("@PipelineExecutionId", pipelineExecutionId.Value);
            }

            using var reader = await command.ExecuteReaderAsync();
            var schemaTable = reader.GetSchemaTable();
            var fieldNames = GetFieldNamesFromSchema(schemaTable);

            while (await reader.ReadAsync())
            {
                var record = new ComparisonRecord
                {
                    RecordComparisonId = reader.GetInt64(reader.GetOrdinal("RecordComparisonId")),
                    PipelineExecutionId = reader.GetInt64(reader.GetOrdinal("PipelineExecutionId")),
                    SubscriberIdentifier = reader.GetString(reader.GetOrdinal("SubscriberIdentifier")),
                    TotalDifferences = reader.GetInt64(reader.GetOrdinal("TotalDifferences")),
                    ChangedFieldsCount = reader.IsDBNull(reader.GetOrdinal("ChangedFieldsCount"))
                                ? (int?)null
                                : reader.GetInt32(reader.GetOrdinal("ChangedFieldsCount")),
                    HasChanges = !reader.IsDBNull(reader.GetOrdinal("HasChanges"))
                 && reader.GetBoolean(reader.GetOrdinal("HasChanges")),
                    UserAcceptance = reader.IsDBNull(reader.GetOrdinal("UserAcceptance")) 
                        ? null 
                        : reader.GetString(reader.GetOrdinal("UserAcceptance"))
                };

                // Load all field comparisons
                foreach (var fieldName in fieldNames)
                {
                    var sourceCol = $"{fieldName}_Source";
                    var destCol = $"{fieldName}_Dest";
                    var selectedCol = $"{fieldName}_Selected";
                    var selectedSourceCol = $"{fieldName}_SelectedSource";

                    var sourceOrdinal = reader.GetOrdinal(sourceCol);
                    var destOrdinal = reader.GetOrdinal(destCol);
                    var selectedOrdinal = reader.GetOrdinal(selectedCol);
                    var selectedSourceOrdinal = reader.GetOrdinal(selectedSourceCol);

                    var sourceValue = reader.IsDBNull(sourceOrdinal) ? string.Empty : reader.GetValue(sourceOrdinal)?.ToString() ?? string.Empty;
                    var destValue = reader.IsDBNull(destOrdinal) ? string.Empty : reader.GetValue(destOrdinal)?.ToString() ?? string.Empty;
                    var selectedValue = reader.IsDBNull(selectedOrdinal) ? string.Empty : reader.GetValue(selectedOrdinal)?.ToString() ?? string.Empty;
                    var selectedSource = reader.IsDBNull(selectedSourceOrdinal) ? string.Empty : reader.GetString(selectedSourceOrdinal)?.Trim() ?? string.Empty;

                    var fieldComparison = new FieldComparison
                    {
                        FieldName = fieldName,
                        SourceValue = sourceValue,
                        DestValue = destValue,
                        SelectedValue = selectedValue,
                        SelectedSource = selectedSource
                    };

                    // Determine cell state based on selected source
                    // On initial load, ALL cells with differences should show as Pending (Yellow)
                    // even if database has SelectedSource = "Source"
                    // Only show Accepted/Rejected if there's a CONFIRMED user selection

                    if (fieldComparison.HasDifference)
                    {
                        // Check if this is a confirmed user selection vs initial database default
                        // If SelectedSource is set to something OTHER than the default "Source",
                        // it means user actively rejected it
                        if (selectedSource.Equals("Dest", StringComparison.OrdinalIgnoreCase))
                        {
                            // User explicitly rejected (clicked X)
                            fieldComparison.CellState = CellState.Rejected;
                        }
                        else if (selectedSource.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
                        {
                            // User explicitly accepted (clicked ✓) - use special marker
                            fieldComparison.CellState = CellState.Accepted;
                        }
                        else
                        {
                            // Initial load or SelectedSource = "Source" (default) -> Pending (Yellow)
                            fieldComparison.CellState = CellState.Pending;
                        }
                    }
                    // else: No difference -> stays as default ReadOnly (handled by converter)

                    record.Fields[fieldName] = fieldComparison;
                }

                records.Add(record);
            }

            return records;
        }

        private List<string> GetFieldNamesFromSchema(DataTable? schemaTable)
        {
            var fieldNames = new HashSet<string>();
            
            if (schemaTable == null)
                return new List<string>();
            
            foreach (DataRow row in schemaTable.Rows)
            {
                var columnName = row["ColumnName"]?.ToString();
                
                if (!string.IsNullOrEmpty(columnName) && columnName.EndsWith("_Source"))
                {
                    var fieldName = columnName.Substring(0, columnName.Length - "_Source".Length);
                    fieldNames.Add(fieldName);
                }
            }

            return [.. fieldNames.OrderBy(f => f)];
        }

        public async Task<List<string>> GetFieldNamesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT TOP 1 * FROM TransactionalizeResultsMemberMaster";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            var schemaTable = reader.GetSchemaTable();
            return GetFieldNamesFromSchema(schemaTable);
        }

        public async Task SaveUserSelectionsAsync(List<ComparisonRecord> records)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var record in records)
                {
                    var updateFields = new List<string>();
                    var command = new SqlCommand { Connection = connection, Transaction = transaction };

                    foreach (var field in record.Fields.Values.Where(f => f.HasDifference))
                    {
                        updateFields.Add($"{field.FieldName}_Selected = @{field.FieldName}_Selected");
                        updateFields.Add($"{field.FieldName}_SelectedSource = @{field.FieldName}_SelectedSource");
                        
                        command.Parameters.AddWithValue($"@{field.FieldName}_Selected", 
                            string.IsNullOrEmpty(field.SelectedValue) ? DBNull.Value : field.SelectedValue);
                        command.Parameters.AddWithValue($"@{field.FieldName}_SelectedSource", 
                            string.IsNullOrEmpty(field.SelectedSource) ? DBNull.Value : field.SelectedSource);
                    }

                    if (updateFields.Count > 0)
                    {
                        command.CommandText = $@"
                            UPDATE TransactionalizeResultsMemberMaster 
                            SET {string.Join(", ", updateFields)}
                            WHERE RecordComparisonId = @RecordComparisonId";
                        
                        command.Parameters.AddWithValue("@RecordComparisonId", record.RecordComparisonId);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
