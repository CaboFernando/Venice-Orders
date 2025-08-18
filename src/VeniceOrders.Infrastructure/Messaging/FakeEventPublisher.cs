using VeniceOrders.Domain.Services;

namespace VeniceOrders.Infrastructure.Messaging
{
    public sealed class FakeEventPublisher : IEventPublisher
    {
        public Task PublishPedidoCriadoAsync(Guid pedidoId, CancellationToken ct)
        {
            Console.WriteLine($"[FAKE SERVICE BUS] PedidoCriado -> {pedidoId}");
            return Task.CompletedTask;
        }
    }
}