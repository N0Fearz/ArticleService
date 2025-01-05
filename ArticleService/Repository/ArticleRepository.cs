using ArticleService.Data;
using ArticleService.Models;

namespace ArticleService.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ArticleContext _articleContext;

        public ArticleRepository(ArticleContext articleContext)
        {
            _articleContext = articleContext;
        }
        public void DeleteArticle(int id)
        {
            var article = _articleContext.Articles.Find(id);
            _articleContext.Articles.Remove(article);
            Save();
        }

        public IEnumerable<Article> GetArticleByIds(IEnumerable<int>  ids)
        {
            return _articleContext.Articles.Where(a => ids.Contains(a.Id)).ToList();
        }

        public IEnumerable<Article> GetArticles()
        {
            return _articleContext.Articles.ToList();
        }

        public void InsertArticle(Article article)
        {
            _articleContext.Add(article);
            Save();
        }

        public void Save()
        {
            _articleContext.SaveChanges();
        }

        public void UpdateArticle(Article article)
        {
            _articleContext.Update(article);
            Save();
        }
    }
}
