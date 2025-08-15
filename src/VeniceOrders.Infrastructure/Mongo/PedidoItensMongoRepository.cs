using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using VeniceOrders.Domain.Repositories;
using VeniceOrders.Domain.ValueObjects;

namespace VeniceOrders.Infrastructure.Mongo
{
    public sealed class PedidoItensMongoRepository : IPedidoItensMongoRepository
    {
        private readonly IMongoCollection<ItemPedidoDocument> _collection;

        public PedidoItensMongoRepository(IMongoClient client, IConfiguration cfg)
        {
            var dbName = cfg["Mongo:Database"] ?? "venice_orders";
            var db = client.GetDatabase(dbName);
            _collection = db.GetCollection<ItemPedidoDocument>("pedido_itens");
        }

        public async Task SaveItensAsync(Guid pedidoId, IReadOnlyCollection<ItemPedido> itens, CancellationToken ct)
        {
            var doc = new ItemPedidoDocument
            {
                PedidoId = pedidoId,
                Itens = itens.Select(i => new ItemRow
                {
                    Produto = i.Produto,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList()
            };

            await _collection.ReplaceOneAsync(
                f => f.PedidoId == pedidoId,
                doc,
                new ReplaceOptions { IsUpsert = true },
                ct
            );
        }

        public async Task<IReadOnlyCollection<ItemPedido>> GetItensAsync(Guid pedidoId, CancellationToken ct)
        {
            var doc = await _collection.Find(f => f.PedidoId == pedidoId).FirstOrDefaultAsync(ct);
            return doc?.Itens?.Select(i => new ItemPedido(i.Produto, i.Quantidade, i.PrecoUnitario)).ToList()
                   ?? new List<ItemPedido>();
        }

        private sealed class ItemPedidoDocument
        {
            public Guid PedidoId { get; set; }
            public List<ItemRow> Itens { get; set; } = new();
        }

        private sealed class ItemRow
        {
            public string Produto { get; set; } = string.Empty;
            public int Quantidade { get; set; }
            public decimal PrecoUnitario { get; set; }
        }
    }
}


