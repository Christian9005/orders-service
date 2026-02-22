using Microsoft.EntityFrameworkCore;
using OrdersDomain.Entities;

namespace OrdersInfrastructure.Context;

public class OrderContext : DbContext
{
    public OrderContext(DbContextOptions<OrderContext> options) : base(options)
    {
    }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> DetailOrders { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
            .HasKey(o => o.Id);

        modelBuilder.Entity<Order>()
            .Property(o => o.Total)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.IdempotencyKey)
            .IsUnique();

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.RequestId);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Details)
            .WithOne(od => od.Order)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderDetail>()
            .HasKey(od => od.Id);

        modelBuilder.Entity<OrderDetail>()
            .Property(od => od.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<AuditLog>()
            .HasIndex(al => al.Id);

        modelBuilder.Entity<AuditLog>()
            .HasIndex(al => al.RequestId);

        modelBuilder.Entity<AuditLog>()
            .HasIndex(al => new { al.Date, al.Occurence });
    }
}
