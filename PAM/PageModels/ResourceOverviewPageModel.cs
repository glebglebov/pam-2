using PAM.Domain.Models;

namespace PAM.PageModels;

public class ResourceOverviewPageModel
{
    public bool CanCreateNew { get; init; }
    public TargetResource[] Resources { get; init; } = default!;
}
