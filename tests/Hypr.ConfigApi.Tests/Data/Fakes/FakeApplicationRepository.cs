using Hypr.ConfigApi.Data;
using Hypr.ConfigApi.Domain;

namespace Hypr.ConfigApi.Tests.Data.Fakes;

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
}

