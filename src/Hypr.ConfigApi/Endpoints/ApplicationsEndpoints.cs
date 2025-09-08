using Hypr.ConfigApi.Data;
using Hypr.ConfigApi.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Hypr.ConfigApi.Endpoints;

public static class ApplicationsEndpoints
{
    public static IEndpointRouteBuilder MapApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/applications");

        group.MapGet("", GetAllAsync)
             .WithName("GetApplications");

        return app;
    }

    // Handler kept public to allow direct unit testing without hosting.
    public static async Task<Ok<IReadOnlyList<Application>>> GetAllAsync(IApplicationRepository repo, CancellationToken ct)
    {
        var apps = await repo.GetAllAsync(ct);
        return TypedResults.Ok(apps);
    }
}
