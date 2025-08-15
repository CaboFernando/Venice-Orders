using VeniceOrders.Domain.ValueObjects;

namespace VeniceOrders.Domain.Repositories
{
    public interface IPedidoItensMongoRepository
    {
        Task SaveItensAsync(Guid pedidoId, IReadOnlyCollection<ItemPedido> itens, CancellationToken ct);
        Task<IReadOnlyCollection<ItemPedido>> GetItensAsync(Guid pedidoId, CancellationToken ct);
    }
}


