namespace VeniceOrders.Application.DTOs;

public sealed record CreatePedidoItemDto(
    string Produto,
    int Quantidade,
    decimal PrecoUnitario
);
