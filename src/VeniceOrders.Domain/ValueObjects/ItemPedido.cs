namespace VeniceOrders.Domain.ValueObjects;

public sealed record ItemPedido(
    string Produto,
    int Quantidade,
    decimal PrecoUnitario
);
