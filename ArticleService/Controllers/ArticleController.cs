using ArticleService.Repository;
using ArticleService.Services;
using AutoMapper;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            var schemaName = await _articleService.GetTenantSchemaName(token);
            _articleService.SetConnectionString(schemaName);
            
            var articles = _articleRepository.GetArticles();
            return Ok(articles);
        }
    }
}
