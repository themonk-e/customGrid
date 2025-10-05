using System;
using System.Data.SqlClient;
using System.Windows;

namespace CustomGridControl.Views
{
    public partial class ConnectionConfigWindow : Window
    {
        public string ConnectionString { get; private set; } = string.Empty;

        public ConnectionConfigWindow()
        {
            InitializeComponent();
            
            // Set initial visibility after controls are loaded
            UpdateSqlAuthPanelVisibility();
        }
        
        private void UpdateSqlAuthPanelVisibility()
        {
            if (SqlAuthPanel != null && IntegratedSecurityCheckBox != null)
            {
                SqlAuthPanel.Visibility = IntegratedSecurityCheckBox.IsChecked == true 
                    ? Visibility.Collapsed 
                    : Visibility.Visible;
            }
        }

        private void IntegratedSecurityCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            UpdateSqlAuthPanelVisibility();
        }

        private string BuildConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ServerTextBox.Text,
                InitialCatalog = DatabaseTextBox.Text,
                TrustServerCertificate = true
            };

            if (IntegratedSecurityCheckBox.IsChecked == true)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.IntegratedSecurity = false;
                builder.UserID = UsernameTextBox.Text;
                builder.Password = PasswordBox.Password;
            }

            return builder.ConnectionString;
        }

        private async void TestConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var connectionString = BuildConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                
                MessageBox.Show("Connection successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ServerTextBox.Text) || string.IsNullOrWhiteSpace(DatabaseTextBox.Text))
            {
                MessageBox.Show("Please enter both server name and database name.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IntegratedSecurityCheckBox.IsChecked == false && 
                (string.IsNullOrWhiteSpace(UsernameTextBox.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password)))
            {
                MessageBox.Show("Please enter username and password for SQL Authentication.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ConnectionString = BuildConnectionString();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
