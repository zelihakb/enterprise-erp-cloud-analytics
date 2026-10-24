using ErpCloud.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ErpCloud.Api.Data;

public class ErpDbContext : DbContext
{
    public ErpDbContext(DbContextOptions<ErpDbContext> options)
        : base(options)
    {
    }

    public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseRequest>()
            .Property(request => request.Id)
            .ValueGeneratedNever();
    }
}