using Microsoft.EntityFrameworkCore;
using TopasContracts.Infrastructure;
using TopasDatabase.Models;

namespace TopasDatabase;

internal class TopasDbContext(IConfigurationDatabase configurationDatabase) : DbContext
{
    private readonly IConfigurationDatabase? _configurationDatabase = configurationDatabase;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configurationDatabase?.ConnectionString, o => o.SetPostgresVersion(12, 2));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Buyer>().HasIndex(x => x.PhoneNumber).IsUnique();
        modelBuilder.Entity<Manufacturer>().HasIndex(x => x.ManufacturerName).IsUnique();

        modelBuilder.Entity<Post>()
            .HasIndex(e => new { e.PostName, e.IsActual })
            .IsUnique()
            .HasFilter($"\"{nameof(Post.IsActual)}\" = TRUE");
        modelBuilder.Entity<Post>()
            .HasIndex(e => new { e.PostId, e.IsActual })
            .IsUnique()
            .HasFilter($"\"{nameof(Post.IsActual)}\" = TRUE");

        modelBuilder.Entity<Product>()
            .HasIndex(x => new { x.ProductName, x.IsDeleted })
            .IsUnique()
            .HasFilter($"\"{nameof(Product.IsDeleted)}\" = FALSE");

        modelBuilder.Entity<Product>()
            .HasOne(e => e.Manufacturer)
            .WithMany(e => e.Products)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SaleProduct>().HasKey(x => new { x.SaleId, x.ProductId });
    }

    public DbSet<Buyer> Buyers { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductHistory> ProductHistories { get; set; }
    public DbSet<Salary> Salaries { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleProduct> SaleProducts { get; set; }
    public DbSet<Worker> Workers { get; set; }
}
