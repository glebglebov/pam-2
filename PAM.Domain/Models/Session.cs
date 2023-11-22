namespace PAM.Domain.Models;

public class Session
{
    public string Guid { get; init; } = default!;
    public int UserId { get; init; }
    public string UserIp { get; init; } = default!;
    public string ResourceName { get; init; } = default!;
    public DateTime BeginTimestamp { get; init; }
    public DateTime? EndTimestamp { get; init; }
}
