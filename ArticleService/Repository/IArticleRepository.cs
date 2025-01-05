using ArticleService.Models;

namespace ArticleService.Repository
{
    public interface IArticleRepository
    {
        IEnumerable<Article> GetArticles();
        IEnumerable<Article> GetArticleByIds(IEnumerable<int> ids);
        void DeleteArticle(int id);
        void UpdateArticle(Article article);
        void InsertArticle(Article article);
        void Save();
    }
}
