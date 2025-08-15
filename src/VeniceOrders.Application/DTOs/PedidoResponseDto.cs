using System;
using System.Collections.Generic;

namespace VeniceOrders.Application.DTOs
{
    public sealed record PedidoResponseDto(
        Guid Id,
        Guid ClienteId,
        DateTime Data,
        string Status,
        IReadOnlyCollection<CreatePedidoItemDto> Itens
    );
}


