using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using PAM.Core.Auth;
using PAM.Domain.Contracts;
// ReSharper disable StringLiteralTypo

namespace PAM.Core.Sessions;

public class SessionManager
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IResourceRepository _resourceRepository;
    private readonly ISessionRepository _sessionRepository;
    
    private readonly IHttpContextAccessor _accessor;
    private readonly ActivityLogger _logger;

    public SessionManager(
        IPermissionRepository permissionRepository,
        IResourceRepository resourceRepository,
        ISessionRepository sessionRepository,
        IHttpContextAccessor accessor,
        ActivityLogger logger)
    {
        _permissionRepository = permissionRepository;
        _resourceRepository = resourceRepository;
        _sessionRepository = sessionRepository;
        _accessor = accessor;
        _logger = logger;
    }

    public async Task CreateSession(int resourceId, WebSocket ws, CancellationToken cancellationToken)
    {
        var userId = _accessor.HttpContext!.GetUserId();
        var socket = new WsWrapper(ws);

        var count = await _permissionRepository.GetPermissionCount(userId, resourceId, cancellationToken);
        if (count < 1)
        {
            await socket.Close("Доступ запрещён");
            return;
        }

        var cred = await _resourceRepository.GetCredentials(resourceId, cancellationToken);
        if (cred == null)
        {
            await socket.Close("Реквизиты не найдены");
            return;
        }

        var guid = Guid.NewGuid();
        var userIp = _accessor.HttpContext!.TryGetIp() ?? "unknown";
        var start = DateTime.Now;
        
        await _sessionRepository.CreateSession(guid, userId, userIp, resourceId, start, cancellationToken);
        await _logger.LogCritical(
            "Начата сессия",
            new Dictionary<string, string>
            {
                { "GUID", guid.ToString() },
                { "ID ресурса", resourceId.ToString() },
                { "ID пользователя", userId.ToString() },
                { "IP пользователя", userIp }
            });

        using var handler = new SessionHandler(socket, cred);
        await handler.Run(cancellationToken);

        var end = DateTime.Now;
        await _sessionRepository.UpdateSession(guid, end);
    }
}
