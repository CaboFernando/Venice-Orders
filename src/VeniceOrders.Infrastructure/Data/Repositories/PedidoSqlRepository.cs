using Microsoft.EntityFrameworkCore;
using VeniceOrders.Domain.Entities;
using VeniceOrders.Domain.Repositories;
using VeniceOrders.Infrastructure.Data.Context;

namespace VeniceOrders.Infrastructure.Data.Repositories
{
    public sealed class PedidoSqlRepository : IPedidoSqlRepository
    {
        private readonly OrdersDbContext _ctx;

        public PedidoSqlRepository(OrdersDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task CreateAsync(Pedido pedido, CancellationToken ct)
        {
            await _ctx.Pedidos.AddAsync(pedido, ct);
            await _ctx.SaveChangesAsync(ct);
        }

        public Task<Pedido?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return _ctx.Pedidos.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }
    }
}


