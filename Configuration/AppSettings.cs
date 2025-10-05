namespace CustomGridControl.Configuration
{
    public static class AppSettings
    {
        // Update this with your actual connection string
        public static string ConnectionString { get; set; } = 
            "Server=YOUR_SERVER_NAME;Database=YOUR_DATABASE_NAME;Integrated Security=true;TrustServerCertificate=true;";
        
        // Or use SQL Authentication
        // "Server=YOUR_SERVER_NAME;Database=YOUR_DATABASE_NAME;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=true;";
    }
}
