using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using POS.Shared.Entities;
using System.Data.Common;

namespace POS.Server.Data
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<CashClosing> CashClosing => Set<CashClosing>();
        public DbSet<Category> Category => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Sale> Sale => Set<Sale>();
        public DbSet<SaleDetail> SaleDetails => Set<SaleDetail>();
        public DbSet<Stock> Stocks => Set<Stock>();
        public DbSet<User> Users => Set<User>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            });
            modelBuilder.Entity<SaleDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
                entity.HasOne(d => d.Sale)
                    .WithMany(s => s.SaleDatails)
                    .HasForeignKey(d => d.SaleId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<CashClosing>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InitialBalance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalCash).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalCard).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalTransfer).HasColumnType("decimal(18,2)");
                entity.Property(e => e.FinalBalance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.FinalCash).HasColumnType("decimal(18,2)");

            });
        }

        
    }
}
