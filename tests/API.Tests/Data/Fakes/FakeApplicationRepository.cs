using Configurator.API.Data;
using Configurator.API.Domain;

namespace API.Tests.Data.Fakes;

public sealed class FakeApplicationRepository : IApplicationRepository
{
    private readonly IReadOnlyList<Application> _apps;

    public FakeApplicationRepository(params Application[] apps)
    {
        _apps = apps;
    }

    public Task<IReadOnlyList<Application>> GetAllAsync(CancellationToken ct = default)
    {
        return Task.FromResult(_apps);
    }

    public Task<Application?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var app = _apps.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(app);
    }
}
