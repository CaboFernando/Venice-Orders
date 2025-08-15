using System.Text.Json;
using MediatR;
using VeniceOrders.Application.DTOs;
using VeniceOrders.Domain.Repositories;
using VeniceOrders.Domain.Services;

namespace VeniceOrders.Application.Queries.GetPedidoById
{
    public sealed class GetPedidoByIdHandler : IRequestHandler<GetPedidoByIdQuery, PedidoResponseDto?>
    {
        private readonly IPedidoSqlRepository _sqlRepository;
        private readonly IPedidoItensMongoRepository _mongoRepository;
        private readonly IPedidoCache _cache;

        public GetPedidoByIdHandler(
            IPedidoSqlRepository sqlRepository,
            IPedidoItensMongoRepository mongoRepository,
            IPedidoCache cache)
        {
            _sqlRepository = sqlRepository;
            _mongoRepository = mongoRepository;
            _cache = cache;
        }

        public async Task<PedidoResponseDto?> Handle(GetPedidoByIdQuery request, CancellationToken ct)
        {
            var cacheKey = $"pedido:{request.Id}";

            var cached = await _cache.GetAsync(cacheKey, ct);
            if (cached is not null)
                return JsonSerializer.Deserialize<PedidoResponseDto>(cached);

            var pedido = await _sqlRepository.GetByIdAsync(request.Id, ct);
            if (pedido is null) return null;

            var itens = await _mongoRepository.GetItensAsync(request.Id, ct);

            var dto = new PedidoResponseDto(
                pedido.Id,
                pedido.ClienteId,
                pedido.Data,
                pedido.Status.ToString(),
                itens.Select(i => new CreatePedidoItemDto(i.Produto, i.Quantidade, i.PrecoUnitario))
                     .ToList()
                     .AsReadOnly()
            );

            var payload = JsonSerializer.Serialize(dto);
            await _cache.SetAsync(cacheKey, payload, TimeSpan.FromMinutes(2), ct);

            return dto;
        }
    }
}


