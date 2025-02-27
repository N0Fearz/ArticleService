﻿namespace ArticleService.Data;

public class TenantContext : ITenantContext
{
    private string _connectionString;
    private readonly IConfiguration _configuration;

    public string ConnectionString => _connectionString;

    public TenantContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void SetConnectionString(string schemaName)
    {
        var connectionString = _configuration.GetConnectionString("ArticleDB");
        var connectionStringWithSchema = $"{connectionString}SearchPath={schemaName};";
        _connectionString = connectionStringWithSchema;
    }
}