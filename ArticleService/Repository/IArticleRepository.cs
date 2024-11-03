using ArticleService.Models;

namespace ArticleService.Repository
{
    public interface IArticleRepository
    {
        IEnumerable<Article> GetArticles();
        Article GetArticleById(int id);
        void DeleteArticle(int id);
        void UpdateArticle(Article article);
        void InsertArticle(Article article);
        void Save();
    }
}
