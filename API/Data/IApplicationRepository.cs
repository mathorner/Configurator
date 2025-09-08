using Configurator.API.Domain;

namespace Configurator.API.Data;

public interface IApplicationRepository
{
    Task<IReadOnlyList<Application>> GetAllAsync(CancellationToken ct = default);
    Task<Application?> GetByIdAsync(int id, CancellationToken ct = default);
}

