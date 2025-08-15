using System.Collections.Generic;

namespace VeniceOrders.Application.DTOs
{
    public sealed record CreatePedidoRequestDto(
        Guid ClienteId,
        IReadOnlyCollection<CreatePedidoItemDto> Itens
    );
}


