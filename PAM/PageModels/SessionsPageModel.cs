using PAM.Domain.Models;

namespace PAM.PageModels;

public class SessionsPageModel
{
    public Session[] Sessions { get; init; } = default!;
}
