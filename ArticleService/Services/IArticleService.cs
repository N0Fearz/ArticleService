namespace ArticleService.Services;

public interface IArticleService
{
    
    public Task<string> GetTenantSchemaName(string token);
    public void SetConnectionString(string schemaName);
    
}