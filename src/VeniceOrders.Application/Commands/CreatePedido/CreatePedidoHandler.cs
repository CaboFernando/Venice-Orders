using MediatR;
using VeniceOrders.Application.DTOs;
using VeniceOrders.Domain.Entities;
using VeniceOrders.Domain.Repositories;
using VeniceOrders.Domain.Services;
using VeniceOrders.Domain.ValueObjects;

namespace VeniceOrders.Application.Commands.CreatePedido
{
    public sealed class CreatePedidoHandler : IRequestHandler<CreatePedidoCommand, Guid>
    {
        private readonly IPedidoSqlRepository _sqlRepository;
        private readonly IPedidoItensMongoRepository _mongoRepository;
        private readonly IEventPublisher _eventPublisher;

        public CreatePedidoHandler(
            IPedidoSqlRepository sqlRepository,
            IPedidoItensMongoRepository mongoRepository,
            IEventPublisher eventPublisher)
        {
            _sqlRepository = sqlRepository;
            _mongoRepository = mongoRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<Guid> Handle(CreatePedidoCommand request, CancellationToken ct)
        {
            var pedido = new Pedido(request.Dto.ClienteId);
            await _sqlRepository.CreateAsync(pedido, ct);

            var itens = request.Dto.Itens
                .Select(i => new ItemPedido(i.Produto, i.Quantidade, i.PrecoUnitario))
                .ToList()
                .AsReadOnly();

            await _mongoRepository.SaveItensAsync(pedido.Id, itens, ct);

            await _eventPublisher.PublishPedidoCriadoAsync(pedido.Id, ct);

            return pedido.Id;
        }
    }
}


