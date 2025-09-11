using Configurator.API.Data;
using Configurator.API.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Configurator.API.Endpoints;

public static class ApplicationsEndpoints
{
    public static IEndpointRouteBuilder MapApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/applications");

        group.MapGet("", GetAllAsync)
             .WithName("GetApplications");

        group.MapGet("/{id:int}", GetByIdAsync)
             .WithName("GetApplicationById");

        return app;
    }

    // Handler kept public to allow direct unit testing without hosting.
    public static async Task<Ok<IReadOnlyList<Application>>> GetAllAsync(IApplicationRepository repo, CancellationToken ct)
    {
        var apps = await repo.GetAllAsync(ct);
        return TypedResults.Ok(apps);
    }

    public static async Task<Results<Ok<Application>, NotFound>> GetByIdAsync(int id, IApplicationRepository repo, CancellationToken ct)
    {
        var app = await repo.GetByIdAsync(id, ct);

        // Temporary hardcoded item for dev UI: return a sample application when id == 1.
        if (app is null && id == 1)
        {
            app = new Application
            {
                Id = 1,
                Name = "Demo Application",
                Description = "Hardcoded sample returned for dev",
                CreatedUtc = DateTime.UtcNow.AddDays(-1),
                UpdatedUtc = DateTime.UtcNow
            };
        }
        return app is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(app);
    }
}
