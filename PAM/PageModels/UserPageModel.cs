using PAM.Domain.Models;

namespace PAM.PageModels;

public class UserPageModel
{
    public User User { get; init; } = default!;
    public TargetResource[] ResourcePermissions { get; init; } = default!;
    public TargetResource[] AvailableResources { get; init; } = default!;
    public string? Base64QrCode { get; init; }
}
