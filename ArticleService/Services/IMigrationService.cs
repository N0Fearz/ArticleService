namespace ArticleService.Services;

public interface IMigrationService
{
    Task AddSchemaAsync(string schemaName);
    Task MigrateAsync(string schemaName);
}