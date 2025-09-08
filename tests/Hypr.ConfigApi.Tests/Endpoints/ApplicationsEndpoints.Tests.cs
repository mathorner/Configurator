using Hypr.ConfigApi.Domain;
using Hypr.ConfigApi.Endpoints;
using Hypr.ConfigApi.Tests.Data.Fakes;
using Microsoft.AspNetCore.Http.HttpResults;
using Xunit;

namespace Hypr.ConfigApi.Tests.Endpoints;

public class ApplicationsEndpointsTests
{
    [Fact]
    public async Task GetAllAsync_Returns_Empty_List()
    {
        var repo = new FakeApplicationRepository();

        var result = await ApplicationsEndpoints.GetAllAsync(repo, CancellationToken.None);

        Assert.IsType<Ok<IReadOnlyList<Application>>>(result);
        var ok = (Ok<IReadOnlyList<Application>>)result;
        Assert.NotNull(ok.Value);
        Assert.Empty(ok.Value);
    }

    [Fact]
    public async Task GetAllAsync_Returns_Items()
    {
        var now = DateTime.UtcNow;
        var repo = new FakeApplicationRepository(
            new Application { Id = 1, Name = "App1", Description = "Desc1", CreatedUtc = now, UpdatedUtc = now },
            new Application { Id = 2, Name = "App2", Description = null, CreatedUtc = now, UpdatedUtc = now }
        );

        var result = await ApplicationsEndpoints.GetAllAsync(repo, CancellationToken.None);

        var ok = Assert.IsType<Ok<IReadOnlyList<Application>>>(result);
        Assert.Equal(2, ok.Value!.Count);
        Assert.Collection(ok.Value!,
            a => Assert.Equal("App1", a.Name),
            a => Assert.Equal("App2", a.Name));
    }
}

