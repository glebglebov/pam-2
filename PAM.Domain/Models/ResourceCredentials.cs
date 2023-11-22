namespace PAM.Domain.Models;

public class ResourceCredentials
{
    public string Address { get; init; } = default!;
    public string Login { get; init; } = default!;
    public string Password { get; init; } = default!;
}
