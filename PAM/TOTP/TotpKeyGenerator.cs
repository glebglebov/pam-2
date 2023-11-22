using System.Security.Cryptography;
using System.Text;

namespace PAM.TOTP;

public static class TotpKeyGenerator
{
    private const int SecretKeyLength = 16;

    public static string GenerateSecretKey()
    {
#pragma warning disable SYSLIB0023
        using var rng = new RNGCryptoServiceProvider();
#pragma warning restore SYSLIB0023
        var secretKeyBytes = new byte[SecretKeyLength];
        rng.GetBytes(secretKeyBytes);
        return Base32Encode(secretKeyBytes);
    }
    
    private static string Base32Encode(IReadOnlyList<byte> bytes)
    {
        const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var sb = new StringBuilder();

        var bitOffset = 0;
        var currentByte = 0;

        while (currentByte < bytes.Count)
        {
            var bitsAvailableInByte = 8 - bitOffset;
            var bitsRequested = Math.Min(5, bitsAvailableInByte);

            var mask = (1 << bitsRequested) - 1;
            var maskedByte = (bytes[currentByte] >> bitOffset) & mask;

            bitOffset += bitsRequested;

            if (bitOffset >= 8)
            {
                currentByte++;
                bitOffset -= 8;
            }

            sb.Append(base32Chars[maskedByte]);
        }

        return sb.ToString();
    }
}
