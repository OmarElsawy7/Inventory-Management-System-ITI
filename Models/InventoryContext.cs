using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Models
{
    public class InventoryContext : DbContext
    {
        public InventoryContext()
        {
        }

        public InventoryContext(DbContextOptions<InventoryContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Purchase> Purchases { get; set; } = null!;
        public DbSet<PurchaseItem> PurchaseItems { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;
        public DbSet<SaleItem> SaleItems { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Data Source=(local);Initial Catalog=InventoryManagementDB;Integrated Security=True;TrustServerCertificate=True"
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Supplier>().ToTable("Suppliers");
            modelBuilder.Entity<Purchase>().ToTable("Purchases");
            modelBuilder.Entity<PurchaseItem>().ToTable("PurchaseItems");
            modelBuilder.Entity<Sale>().ToTable("Sales");
            modelBuilder.Entity<SaleItem>().ToTable("SalesItems");

            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.Purchases)
                .WithOne(p => p.Supplier)
                .HasForeignKey(p => p.SupplierID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Purchase>()
                .HasMany(p => p.PurchaseItems)
                .WithOne(pi => pi.Purchase)
                .HasForeignKey(pi => pi.PurchaseID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Product)
                .WithMany()
                .HasForeignKey(pi => pi.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sale>()
                .HasMany(s => s.SaleItems).WithOne(si => si.Sale)
                .HasForeignKey(si => si.SaleID).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.Product).WithMany()
                .HasForeignKey(si => si.ProductID).OnDelete(DeleteBehavior.Restrict);
        }
    }
}