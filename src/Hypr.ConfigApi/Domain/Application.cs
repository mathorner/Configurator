namespace Hypr.ConfigApi.Domain;

public sealed class Application
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime UpdatedUtc { get; init; }
}

