namespace PAM.Domain.Models;

public class Log
{
    public string Title { get; init; } = default!;
    public LogLevel Level { get; init; }
    public LogType Type { get; init; }
    public DateTime Timestamp { get; init; }
    public string Data { get; init; } = default!;
}
