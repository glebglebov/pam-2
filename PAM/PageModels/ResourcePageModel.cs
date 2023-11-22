using PAM.Domain.Models;

namespace PAM.PageModels;

public class ResourcePageModel
{
    public bool HasAccess { get; init; }
    public TargetResource Resource { get; init; } = default!;
}
