using Configurator.API.Domain;
using Configurator.API.Endpoints;
using API.Tests.Data.Fakes;
using Microsoft.AspNetCore.Http.HttpResults;
using Xunit;

namespace API.Tests.Endpoints;

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

    [Fact]
    public async Task GetByIdAsync_Returns_NotFound_When_Missing()
    {
        var repo = new FakeApplicationRepository();

        var result = await ApplicationsEndpoints.GetByIdAsync(123, repo, CancellationToken.None);

        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Item_When_Found()
    {
        var now = DateTime.UtcNow;
        var repo = new FakeApplicationRepository(
            new Application { Id = 10, Name = "X", Description = null, CreatedUtc = now, UpdatedUtc = now }
        );

        var result = await ApplicationsEndpoints.GetByIdAsync(10, repo, CancellationToken.None);

        var ok = Assert.IsType<Ok<Application>>(result.Result);
        Assert.Equal(10, ok.Value!.Id);
        Assert.Equal("X", ok.Value!.Name);
    }
}
