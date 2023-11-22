namespace PAM.Domain.Models;

public class User
{
    public int Id { get; init; }
    public string Login { get; init; } = default!;
    public int Level { get; init; }
    public string SecretKey { get; init; } = default!;
    public bool Is2FaEnabled { get; init; }
}
