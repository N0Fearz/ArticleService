namespace ArticleService.Data;

public interface ITenantContext
{
    string ConnectionString { get; }
    void SetConnectionString(string connectionString);
}