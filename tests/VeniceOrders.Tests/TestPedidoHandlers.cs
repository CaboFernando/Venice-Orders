using FluentAssertions;
using Moq;
using VeniceOrders.Application.Commands.CreatePedido;
using VeniceOrders.Application.DTOs;
using VeniceOrders.Application.Queries.GetPedidoById;
using VeniceOrders.Domain.Entities;
using VeniceOrders.Domain.Enums;
using VeniceOrders.Domain.Repositories;
using VeniceOrders.Domain.Services;
using VeniceOrders.Domain.ValueObjects;

namespace VeniceOrders.Tests;

public class TestPedidoHandlers
{
    [Fact]
    public async Task CreatePedido_Deve_Publicar_Evento()
    {
        var sql = new Mock<IPedidoSqlRepository>();
        var mongo = new Mock<IPedidoItensMongoRepository>();
        var publisher = new Mock<IEventPublisher>();

        var handler = new CreatePedidoHandler(sql.Object, mongo.Object, publisher.Object);
        var dto = new CreatePedidoRequestDto(Guid.NewGuid(),
            new[] { new CreatePedidoItemDto("Produto", 1, 10m) });

        var id = await handler.Handle(new CreatePedidoCommand(dto), CancellationToken.None);

        publisher.Verify(p => p.PublishPedidoCriadoAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_Usa_Cache_Quando_Disponivel()
    {
        var sql = new Mock<IPedidoSqlRepository>();
        var mongo = new Mock<IPedidoItensMongoRepository>();
        var cache = new Mock<IPedidoCache>();

        var pedido = new Pedido(Guid.NewGuid());
        sql.Setup(r => r.GetByIdAsync(pedido.Id, It.IsAny<CancellationToken>())).ReturnsAsync(pedido);
        mongo.Setup(r => r.GetItensAsync(pedido.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(new[] { new ItemPedido("X", 1, 1m) });

        
        cache.SetupSequence(c => c.GetAsync($"pedido:{pedido.Id}", It.IsAny<CancellationToken>()))
             .ReturnsAsync((string?)null)
             .ReturnsAsync(System.Text.Json.JsonSerializer.Serialize(
                 new PedidoResponseDto(pedido.Id, pedido.ClienteId, pedido.Data, "Criado",
                 new[] { new CreatePedidoItemDto("X", 1, 1m) })));

        var handler = new GetPedidoByIdHandler(sql.Object, mongo.Object, cache.Object);

        var first = await handler.Handle(new GetPedidoByIdQuery(pedido.Id), CancellationToken.None);
        var second = await handler.Handle(new GetPedidoByIdQuery(pedido.Id), CancellationToken.None);

        first.Should().NotBeNull();
        second.Should().NotBeNull();
        sql.Verify(r => r.GetByIdAsync(pedido.Id, It.IsAny<CancellationToken>()), Times.Once); // segunda veio do cache
    }

}
