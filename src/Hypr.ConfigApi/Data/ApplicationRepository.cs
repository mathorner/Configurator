using Hypr.ConfigApi.Domain;

namespace Hypr.ConfigApi.Data;

// Temporary in-memory repository to bootstrap the first endpoint.
// This will be replaced with a MySQL-backed implementation later.
public sealed class ApplicationRepository : IApplicationRepository
{
    private readonly List<Application> _apps = new();

    public Task<IReadOnlyList<Application>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyList<Application> result = _apps;
        return Task.FromResult(result);
    }

    public Task<Application?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var app = _apps.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(app);
    }
}
