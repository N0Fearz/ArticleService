using Microsoft.EntityFrameworkCore;

namespace ArticleService.Data
{
    public class TenantDbContext(DbContextOptions<TenantDbContext> options) : DbContext(options)
    {
        //public DbSet<Tenant> Tenants { get; set; } = null!;
    }
}
