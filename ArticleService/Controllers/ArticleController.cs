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
        public ArticleController(IArticleRepository articleRepository, IHttpContextAccessor httpContextAccessor, IArticleService articleService)
        {
            _articleRepository = articleRepository;
            _httpContextAccessor = httpContextAccessor;
            _articleService = articleService;
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
            return Ok();
        }
    }
}
