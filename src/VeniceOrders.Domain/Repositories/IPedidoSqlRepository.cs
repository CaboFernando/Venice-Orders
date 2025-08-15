using VeniceOrders.Domain.Entities;

namespace VeniceOrders.Domain.Repositories
{
    public interface IPedidoSqlRepository
    {
        Task CreateAsync(Pedido pedido, CancellationToken ct);
        Task<Pedido?> GetByIdAsync(Guid id, CancellationToken ct);
    }
}


