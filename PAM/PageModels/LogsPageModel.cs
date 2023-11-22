using PAM.Domain.Models;

namespace PAM.PageModels;

public class LogsPageModel
{
    public int CurrentPage { get; init; }
    public int PageCount { get; init; }
    public Log[] Logs { get; init; } = default!;
}
