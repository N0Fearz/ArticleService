using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using ArticleService.Data;
using ArticleService.Factory;
using ArticleService.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArticleService.Services;

public class ArticleService : IArticleService
{
    private readonly RabbitMqSenderOrganization _rabbitMqSenderOrganization;
    public ArticleService(IArticleRepository articleRepository, RabbitMqSenderOrganization senderOrganization)
    {
        _rabbitMqSenderOrganization = senderOrganization;
    }

    public string GetTenantConnectionString(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var parsedJwt = handler.ReadJwtToken(token);
        var org = parsedJwt.Claims.First(c => c.Type == "organization").Value;

        var parsed = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(org);
        var id = parsed?.Values.FirstOrDefault()?.GetValueOrDefault("id");
        var message = _rabbitMqSenderOrganization.SendMessage(id);

        return message.Result;
    }
}