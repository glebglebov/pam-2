using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PAM.Core.Auth;

public class UserSessionInfo
{
    [JsonPropertyName("user_id")]
    public int UserId { get; init; }

    [JsonPropertyName("is_verified")]
    public bool IsVerified { get; init; }

    public UserSessionInfo(int userId, bool isVerified)
    {
        UserId = userId;
        IsVerified = isVerified;
    }

    public static UserSessionInfo? FromJson(string? json)
        => json == null ? null : JsonSerializer.Deserialize<UserSessionInfo>(json);

    public string Serialize()
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(this);
        return Encoding.UTF8.GetString(bytes);
    }
}
