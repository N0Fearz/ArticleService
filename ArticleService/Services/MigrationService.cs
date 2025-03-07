﻿using System.Runtime.CompilerServices;
using System.Text;
using ArticleService.Data;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Services;

public class MigrationService : IMigrationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    public MigrationService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public async Task AddSchemaAsync(string schemaName)
    {
        var connectionString = _configuration.GetConnectionString("ArticleDB");
        var optionsBuilder = new DbContextOptionsBuilder<ArticleContext>();
        optionsBuilder.UseNpgsql(connectionString);
        
        using var scope = _serviceProvider.CreateScope();
        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.SetConnectionString(connectionString);
        var dbContext = new ArticleContext(optionsBuilder.Options, scope.ServiceProvider.GetRequiredService<ITenantContext>());
        var cmd = new StringBuilder().Append("CREATE SCHEMA IF NOT EXISTS ").Append(schemaName).ToString();
        var formattableString = FormattableStringFactory.Create(cmd);
        
        await dbContext.Database.ExecuteSqlAsync(formattableString);
    }
    public async Task MigrateAsync(string schemaName)
    {
        var connectionString = _configuration.GetConnectionString("ArticleDB");
        var connectionStringWithSchema = $"{connectionString}SearchPath={schemaName};";
        
        Console.WriteLine(connectionStringWithSchema);
        
        var optionsBuilder = new DbContextOptionsBuilder<ArticleContext>();
        optionsBuilder.UseNpgsql(connectionStringWithSchema);
        
        using var scope = _serviceProvider.CreateScope();
        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.SetConnectionString(connectionStringWithSchema);
        var dbContext = new ArticleContext(optionsBuilder.Options, scope.ServiceProvider.GetRequiredService<ITenantContext>());

        // Check if the migrations are needed
        if (await dbContext.Database.GetPendingMigrationsAsync() is { } migrations && migrations.Any())
        {
            await dbContext.Database.MigrateAsync();
        }
    }
    
    public async Task RemoveSchemaAsync(string schemaName)
    {
        var connectionString = _configuration.GetConnectionString("OrderDB");
        var optionsBuilder = new DbContextOptionsBuilder<ArticleContext>();
        optionsBuilder.UseNpgsql(connectionString);

        using (var scope = _serviceProvider.CreateScope())
        {
            var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
            tenantContext.SetConnectionString(connectionString);
            var dbContext = new ArticleContext(optionsBuilder.Options, scope.ServiceProvider.GetRequiredService<ITenantContext>());
        
            // Query to drop the schema
            var cmd = new StringBuilder().Append("DROP SCHEMA IF EXISTS ").Append(schemaName).Append(" CASCADE").ToString();
            var formattableString = FormattableStringFactory.Create(cmd);

            await dbContext.Database.ExecuteSqlAsync(formattableString);
        }
    }
}