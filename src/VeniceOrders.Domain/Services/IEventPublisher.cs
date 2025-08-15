namespace VeniceOrders.Domain.Services;

public interface IEventPublisher
{
    Task PublishPedidoCriadoAsync(Guid pedidoId, CancellationToken ct);
}
