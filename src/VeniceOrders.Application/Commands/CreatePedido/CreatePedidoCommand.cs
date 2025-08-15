using MediatR;
using VeniceOrders.Application.DTOs;

namespace VeniceOrders.Application.Commands.CreatePedido
{
    public sealed record CreatePedidoCommand(CreatePedidoRequestDto Dto) : IRequest<Guid>;
}


