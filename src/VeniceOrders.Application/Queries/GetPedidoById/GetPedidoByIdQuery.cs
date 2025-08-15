using MediatR;
using VeniceOrders.Application.DTOs;

namespace VeniceOrders.Application.Queries.GetPedidoById
{
    public sealed record GetPedidoByIdQuery(Guid Id) : IRequest<PedidoResponseDto?>;
}


