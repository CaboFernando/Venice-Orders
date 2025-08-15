using Microsoft.Extensions.Caching.Distributed;
using VeniceOrders.Domain.Services;

namespace VeniceOrders.Infrastructure.Cache
{
    public sealed class PedidoCache : IPedidoCache
    {
        private readonly IDistributedCache _cache;

        public PedidoCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        public Task<string?> GetAsync(string key, CancellationToken ct)
            => _cache.GetStringAsync(key, ct);

        public Task SetAsync(string key, string payload, TimeSpan ttl, CancellationToken ct)
            => _cache.SetStringAsync(
                key,
                payload,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ttl
                },
                ct
            );
    }
}


