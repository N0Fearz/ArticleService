using ArticleService.Models;
using ArticleService.Repository;
using ArticleService.Services;
using AutoMapper;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;

namespace ArticleService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IArticleService _articleService;
        private readonly ILogPublisher _logPublisher;
        public ArticleController(IArticleRepository articleRepository, IHttpContextAccessor httpContextAccessor, IArticleService articleService, ILogPublisher logPublisher)
        {
            _articleRepository = articleRepository;
            _httpContextAccessor = httpContextAccessor;
            _articleService = articleService;
            _logPublisher = logPublisher;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", string.Empty);
            var schemaName = await _articleService.GetTenantSchemaName(accessToken);
            _articleService.SetConnectionString(schemaName);
            
            var articles = _articleRepository.GetArticles();
            return Ok(articles);
        }
        
        [HttpGet("by-ids")]
        public async Task<IActionResult> GetByIds([FromQuery] IEnumerable<int> ids)
        {
            var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", string.Empty);
            var schemaName = await _articleService.GetTenantSchemaName(accessToken);
            _articleService.SetConnectionString(schemaName);

            var articles = _articleRepository.GetArticleByIds(ids);
            if (articles == null || !articles.Any())
            {
                return NotFound("No articles found for the provided IDs.");
            }

            return Ok(articles);
        }
        
        [HttpPut("update/{articleId}")]
        public async Task<IActionResult> Update(Article article)
        {
            var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", string.Empty);
            var schemaName = await _articleService.GetTenantSchemaName(accessToken);
            _articleService.SetConnectionString(schemaName);
            
            _articleRepository.UpdateArticle(article);
            _logPublisher.SendMessage(new LogMessage
            {
                ServiceName = "ArticleService",
                LogLevel = "Information",
                Message = "Article updated successfully.",
                Timestamp = DateTime.Now,
                Metadata = new Dictionary<string, string>
                {
                    { "ArticleCode", article.ArticleCode }
                }
            });
            return Ok(article);
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] List<Article> articles)
        {
            var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", string.Empty);
            var schemaName = await _articleService.GetTenantSchemaName(accessToken);
            _articleService.SetConnectionString(schemaName);

            foreach (var article in articles)
            {
                article.Id = 0;
                _articleRepository.InsertArticle(article);
            }
            _logPublisher.SendMessage(new LogMessage
            {
                ServiceName = "ArticleService",
                LogLevel = "Information",
                Message = "Articles created successfully.",
                Timestamp = DateTime.Now,
                Metadata = new Dictionary<string, string>
                {
                    { "ArticleCount", articles.Count.ToString() }
                }
            });
            return Ok();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Article article)
        {
            var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", string.Empty);
            var schemaName = await _articleService.GetTenantSchemaName(accessToken);
            _articleService.SetConnectionString(schemaName);

            article.Id = 0;
            _articleRepository.InsertArticle(article);
            _logPublisher.SendMessage(new LogMessage
            {
                ServiceName = "ArticleService",
                LogLevel = "Information",
                Message = "Article created successfully.",
                Timestamp = DateTime.Now,
                Metadata = new Dictionary<string, string>
                {
                    { "ArticleCode", article.ArticleCode }
                }
            });
            return Ok(article);
        }
        
        [HttpDelete("delete/{articleId}")]
        public async Task<IActionResult> Delete(int articleId)
        {
            var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", string.Empty);
            var schemaName = await _articleService.GetTenantSchemaName(accessToken);
            _articleService.SetConnectionString(schemaName);
            
            _articleRepository.DeleteArticle(articleId);
            _logPublisher.SendMessage(new LogMessage
            {
                ServiceName = "ArticleService",
                LogLevel = "Information",
                Message = "Article deleted successfully.",
                Timestamp = DateTime.Now,
                Metadata = new Dictionary<string, string>
                {
                    { "ArticleCode", articleId.ToString() }
                }
            });
            return Ok();
        }
    }
}
