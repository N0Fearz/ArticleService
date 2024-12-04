using ArticleService.Data;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Services;

public class MigrationService : IMigrationService
{
    private readonly IServiceProvider _serviceProvider;

    public MigrationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ArticleContext>();
        optionsBuilder.UseNpgsql(connectionString);
        
        using var scope = _serviceProvider.CreateScope();
        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.SetConnectionString(connectionString);
        var dbContext = new ArticleContext(optionsBuilder.Options, scope.ServiceProvider.GetRequiredService<ITenantContext>());

        // Check if the migrations are needed
        await dbContext.Database.EnsureCreatedAsync();
        await dbContext.Database.MigrateAsync();
    }
}