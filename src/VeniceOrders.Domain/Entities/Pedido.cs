using VeniceOrders.Domain.Enums;

namespace VeniceOrders.Domain.Entities
{
    public sealed class Pedido
    {
        public Guid Id { get; private set; }
        public Guid ClienteId { get; private set; }
        public DateTime Data { get; private set; }
        public PedidoStatus Status { get; private set; }

        private Pedido() { }

        public Pedido(Guid clienteId)
        {
            Id = Guid.NewGuid();
            ClienteId = clienteId;
            Data = DateTime.UtcNow;
            Status = PedidoStatus.Criado;
        }

        public void AlterarStatus(PedidoStatus novoStatus)
        {
            Status = novoStatus;
        }
    }

}

