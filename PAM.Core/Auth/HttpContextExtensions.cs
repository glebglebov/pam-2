using Microsoft.AspNetCore.Http;

namespace PAM.Core.Auth;

public static class HttpContextExtensions
{
    private const string UserInfoKey = "user_info";

    public static bool TryGetUserId(this HttpContext context, out int userId)
    {
        var value = context.Session.GetString(UserInfoKey);
        var info = UserSessionInfo.FromJson(value);
        if (info == null)
        {
            userId = -1;
            return false;
        }

        userId = info.UserId;
        return true;
    }

    public static int GetUserId(this HttpContext context)
    {
        var value = context.Session.GetString(UserInfoKey);
        var info = UserSessionInfo.FromJson(value);
        return info?.UserId ?? -1;
    }

    public static void SetUserId(this HttpContext context, int userId)
    {
        var info = new UserSessionInfo(userId, false);
        var json = info.Serialize();
        context.Session.SetString(UserInfoKey, json);
    }

    public static void SetIsVerifiedToTrue(this HttpContext context, int userId)
    {
        var info = new UserSessionInfo(userId, true);
        context.Session.SetString(UserInfoKey, info.Serialize());
    }

    public static bool IsVerified(this HttpContext context)
    {
        var value = context.Session.GetString(UserInfoKey);
        var info = UserSessionInfo.FromJson(value!);
        return info?.IsVerified ?? false;
    }

    public static void ResetUserInfo(this HttpContext context)
        => context.Session.Remove(UserInfoKey);

    public static string? TryGetIp(this HttpContext context)
        => context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
}
