using ArticleService.Data;
using ArticleService.Repository;
using ArticleService.ServiceCollection;
using ArticleService.Services;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddKeycloakWebApi(builder.Configuration,
    options =>
        options.Events.OnTokenValidated = async ctx =>
{
    // For some reason, the access token's claims are not getting added to the user in C#
    // So this method hooks into the TokenValidation and adds it manually...
    // This definitely seems like a bug to me.

    // First, let's just get the access token and read it as a JWT
    var token = ctx.SecurityToken;
    var handler = new JwtSecurityTokenHandler();
    var Jwt = handler.WriteToken(token);
    var parsedJwt = handler.ReadJwtToken(Jwt);
    var org = parsedJwt.Claims.First(c => c.Type == "organization").Value;
    Console.WriteLine(org);
}
    );

builder.Services
    .AddAuthorization()
    .AddKeycloakAuthorization()
    .AddAuthorizationServer(builder.Configuration);

builder.Services.AddScoped<IMigrationService, MigrationService>();
builder.Services.AddTransient<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddHostedService<RabbitMQConsumer>();
builder.Services.AddEndpointsApiExplorer().AddSwagger();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<ArticleContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("ArticleDB"),
        o => o
            .SetPostgresVersion(17, 0)));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(options => options.EnableTryItOutByDefault());

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapControllers();

app.Run();
