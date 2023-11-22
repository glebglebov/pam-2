namespace PAM.Domain.Models;

public record TargetResource
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string Address { get; init; } = default!;
}
