using System.Security.Cryptography;
using System.Text;

namespace PAM.Services;

public static class HashHelpers
{
    public static string GetHash(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(bytes);
        
        return Convert
            .ToHexString(hash)
            .ToLower();
    }
}
