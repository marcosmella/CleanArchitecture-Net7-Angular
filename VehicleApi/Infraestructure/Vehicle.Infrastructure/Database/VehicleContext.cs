using Microsoft.EntityFrameworkCore;
using Vehicle.Domain.Entities;

namespace Vehicle.Infrastructure.Database;
public partial class VehicleContext : DbContext
{
    public VehicleContext()
    {
    }

    public VehicleContext(DbContextOptions<VehicleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Domain.Entities.Vehicle> Vehicles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC07BE03D1A7");

            entity.ToTable("Category");

            entity.Property(e => e.IconUrl).HasMaxLength(255);
            entity.Property(e => e.MaxWeightKg).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinWeightKg).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Domain.Entities.Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vehicle__3214EC07BAD47410");

            entity.ToTable("Vehicle");

            entity.Property(e => e.Manufacturer).HasMaxLength(255);
            entity.Property(e => e.OwnerName).HasMaxLength(255);
            entity.Property(e => e.WeightKg).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_Category");
        });

        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
