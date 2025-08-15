using Microsoft.EntityFrameworkCore;
using VeniceOrders.Domain.Entities;
using VeniceOrders.Domain.Enums;

namespace VeniceOrders.Infrastructure.Data.Context
{
    public sealed class OrdersDbContext : DbContext
    {
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

        public DbSet<Pedido> Pedidos => Set<Pedido>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pedido>(e =>
            {
                e.ToTable("Pedidos");
                e.HasKey(p => p.Id);
                e.Property(p => p.ClienteId).IsRequired();
                e.Property(p => p.Data).IsRequired();
                e.Property(p => p.Status)
                    .HasConversion<int>()
                    .IsRequired();
            });
        }
    }
}


