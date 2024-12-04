namespace ArticleService.Services;

public interface IMigrationService
{
    Task MigrateAsync(string connectionString);
}