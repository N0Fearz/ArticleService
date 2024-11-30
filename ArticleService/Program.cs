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

var builder = WebApplication.CreateBuilder(args);

//ConnectionFactory connectionFactory = new();
//connectionFactory.UserName = "user";
//connectionFactory.Password = "jaLMcAGEyYm46Daj";
//connectionFactory.VirtualHost = "/";
//connectionFactory.HostName = "192.168.2.152";
////connectionFactory.Uri = new Uri("amqp://user:jaLMcAGEyYm46Daj@192.168.2.152:5672");
//connectionFactory.ClientProvidedName = "Article service";
//var connection = await connectionFactory.CreateConnectionAsync();
//using var channel = await connection.CreateChannelAsync();
//builder.Services.AddHostedService<RabbitMQConsumer>();

//channel.QueueDeclareAsync

var tokenHandler = new JwtSecurityTokenHandler();

//builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
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
    var org = parsedJwt.Claims.First(c => c.Type == "organization").Value; ;

}
    );

builder.Services
    .AddAuthorization()
    .AddKeycloakAuthorization()
    .AddAuthorizationServer(builder.Configuration);

builder.Services.AddTransient<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddHostedService<RabbitMQConsumer>();
builder.Services.AddEndpointsApiExplorer().AddSwagger();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContextPool<ArticleContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("ArticleDB"),
        o => o
            .SetPostgresVersion(17, 0)));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(options => options.EnableTryItOutByDefault());
//if (app.Environment.IsDevelopment())
//{
//app.UseSwagger();
//app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapControllers();

app.Run();
