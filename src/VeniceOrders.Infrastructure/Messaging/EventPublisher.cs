using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using VeniceOrders.Domain.Services;

namespace VeniceOrders.Infrastructure.Messaging
{
    public sealed class EventPublisher : IEventPublisher, IAsyncDisposable
    {
        private readonly ServiceBusSender _sender;

        public EventPublisher(ServiceBusClient client, IConfiguration cfg)
        {
            var queueName = cfg["ServiceBus:QueueName"] ?? "pedido-criado";
            _sender = client.CreateSender(queueName);
        }

        public async Task PublishPedidoCriadoAsync(Guid pedidoId, CancellationToken ct)
        {
            var body = System.Text.Json.JsonSerializer.Serialize(new
            {
                PedidoId = pedidoId,
                Tipo = "PedidoCriado"
            });

            await _sender.SendMessageAsync(new ServiceBusMessage(body), ct);
        }

        public async ValueTask DisposeAsync()
        {
            await _sender.DisposeAsync();
        }
    }
}


