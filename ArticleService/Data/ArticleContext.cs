using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Data
{
    public class ArticleContext(DbContextOptions<ArticleContext> options, ITenantContext tenantContext) : DbContext(options)
    {
        private readonly ITenantContext _tenantContext = tenantContext;
        public DbSet<Article> Articles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_tenantContext.ConnectionString))
            {
                optionsBuilder.UseNpgsql(_tenantContext.ConnectionString);
            }
            else
            {
                throw new ArgumentNullException(_tenantContext.ConnectionString, "Connection string not set for tenant.");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(entity =>
            {
                entity.Property(o => o.ArticleCode)
                    .IsRequired()
                    .HasMaxLength(50);
            });
            
            modelBuilder.Entity<Article>().HasData(
            new Article
            {
                Id = 1,
                ArticleCode = "M11256",
                Description = "Electronic Items",
            },
            new Article
            {
                Id = 2,
                ArticleCode = "M11256",
                Description = "test Items",
            },
            new Article
            {
                Id = 3,
                ArticleCode = "M11256",
                Description = "other Items",
            }
        );
        }
    }
}
