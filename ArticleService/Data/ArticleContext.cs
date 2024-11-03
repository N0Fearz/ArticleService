using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Data
{
    public class ArticleContext(DbContextOptions<ArticleContext> options) : DbContext(options)
    {
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>().HasData(
            new Article
            {
                ArticleId = 1,
                ArticleCode = "M11256",
                Description = "Electronic Items",
            },
            new Article
            {
                ArticleId = 2,
                ArticleCode = "M11256",
                Description = "test Items",
            },
            new Article
            {
                ArticleId = 3,
                ArticleCode = "M11256",
                Description = "other Items",
            }
        );
        }
    }
}
