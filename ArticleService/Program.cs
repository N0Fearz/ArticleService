using ArticleService.Data;
using ArticleService.Repository;
using ArticleService.ServiceCollection;
using ArticleService.Services;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ArticleService.Handlers;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var disableAuth = Environment.GetEnvironmentVariable("DISABLE_AUTH") == "true";

if (disableAuth)
{
    // Schakel Keycloak-authenticatie uit
    builder.Services.AddAuthentication("Test")
        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });
}
else
{
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddKeycloakWebApi(builder.Configuration);
    
    builder.Services
        .AddAuthorization()
        .AddKeycloakAuthorization()
        .AddAuthorizationServer(builder.Configuration);
}

builder.Services.AddScoped<IMigrationService, MigrationService>();
builder.Services.AddTransient<IArticleRepository, ArticleRepository>();
builder.Services.AddSingleton<ITenantContext, TenantContext>();
builder.Services.AddTransient<IArticleService, ArticleService.Services.ArticleService>();
builder.Services.AddHostedService<RabbitMQConsumer>();
builder.Services.AddHostedService<RabbitMqConsumeDeleteOrganization>();
builder.Services.AddSingleton<RabbitMqSenderOrganization>();
builder.Services.AddEndpointsApiExplorer().AddSwagger();
builder.Services.AddHttpContextAccessor();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<ArticleContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("ArticleDB"),
        o => o
            .SetPostgresVersion(16, 0)));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});


var app = builder.Build();
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options => options.EnableTryItOutByDefault());

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapControllers();

await app.RunAsync();
