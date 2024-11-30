namespace ArticleService.Factory;

public class TenantConnectionFactory
{
    private readonly string _connectionString;

    public TenantConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public string GetConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("Connection string not found for tenant.");
        }

        return connectionString;
    }
}