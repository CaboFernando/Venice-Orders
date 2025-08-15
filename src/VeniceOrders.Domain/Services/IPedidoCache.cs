namespace VeniceOrders.Domain.Services;

public interface IPedidoCache
{
    Task<string?> GetAsync(string key, CancellationToken ct);
    Task SetAsync(string key, string payload, TimeSpan ttl, CancellationToken ct);
}
