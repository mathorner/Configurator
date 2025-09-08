using Hypr.ConfigApi.Domain;

namespace Hypr.ConfigApi.Data;

public interface IApplicationRepository
{
    Task<IReadOnlyList<Application>> GetAllAsync(CancellationToken ct = default);
}

