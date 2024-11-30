using System.Configuration;
using ArticleService.Data;
using ArticleService.Factory;
using ArticleService.Repository;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Services;

public class ArticleService : IArticleService
{
    private IArticleRepository _articleRepository;
    private ArticleContext _articleContext;
    public ArticleService(IArticleRepository articleRepository, ArticleContext articleContext)
    {
        _articleRepository = articleRepository;
    }
    //public string InitiateTenant(string connectionString)
    //{
    //    return TenantConnectionFactory.GetConnectionString(connectionString);
    //}
}