using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ArticleService.ServiceCollection
{
    public static class ServiceCollectionExtensions
    {
        //public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
        //{
        //    // https://dev.to/kayesislam/integrating-openid-connect-to-your-application-stack-25ch
        //    services
        //        .AddAuthentication()
        //        .AddJwtBearer(x =>
        //        {
        //            x.RequireHttpsMetadata = Convert.ToBoolean($"{configuration["Keycloak1:require-https"]}");
        //            x.MetadataAddress = $"{configuration["Keycloak1:server-url"]}/realms/Organisations/.well-known/openid-configuration";
        //            x.TokenValidationParameters = new TokenValidationParameters
        //            {
        //                RoleClaimType = "groups",
        //                ValidAudience = $"{configuration["Keycloak1:audience"]}",
        //                // https://stackoverflow.com/questions/60306175/bearer-error-invalid-token-error-description-the-issuer-is-invalid
        //                ValidateIssuer = Convert.ToBoolean($"{configuration["Keycloak1:validate-issuer"]}"),
        //            };
        //        });

        //    services.AddAuthorization(o =>
        //    {
        //        o.DefaultPolicy = new AuthorizationPolicyBuilder()
        //            .RequireAuthenticatedUser()
        //            .RequireClaim("email_verified", "true")
        //            .Build();
        //    });

        //    return services;
        //}

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // must be lower case
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {securityScheme, Array.Empty<string>()}
            });
            });
            return services;
        }
    }
}
