using dukkantek.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace dukkantek.Api.Infrastructure;

public class InventoryContext : DbContext
{
    public InventoryContext(DbContextOptions<InventoryContext> contextOptions) : base(contextOptions)
    {
    }

    public DbSet<ProductDbModel> Products { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductDbModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Products_pk");

            entity.Property(e => e.Id)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Barcode)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.CategoryName)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(256)
                .IsUnicode(false);
        });
    }
}