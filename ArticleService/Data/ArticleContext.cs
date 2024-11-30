using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Data
{
    public class ArticleContext(DbContextOptions<ArticleContext> options, ITenantContext tenantContext) : DbContext(options)
    {
        private ITenantContext _tenantContext = tenantContext;
        public DbSet<Article> Articles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_tenantContext.ConnectionString))
            {
                optionsBuilder.UseNpgsql(_tenantContext.ConnectionString);
            }
            else
            {
                throw new Exception("Connection string not set for tenant.");
            }
            if (!string.IsNullOrEmpty(_tenantContext.ConnectionString))
            {
                optionsBuilder.UseNpgsql(_tenantContext.ConnectionString);

                // Run migrations dynamically
                using var dbContext = new ArticleContext(new DbContextOptionsBuilder<ArticleContext>()
                    .UseNpgsql(_tenantContext.ConnectionString).Options, _tenantContext);

                // Check and apply pending migrations
                var pendingMigrations = dbContext.Database.GetPendingMigrations();
                if (pendingMigrations.Any())
                {
                    dbContext.Database.Migrate(); // Apply migrations
                }
            }
            else
            {
                throw new Exception("Connection string not set for tenant.");
            }
        }

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
