using PAM.Domain.Models;

namespace PAM.PageModels;

public class UserOverviewPageModel
{
    public User[] Users { get; init; } = default!;
}
