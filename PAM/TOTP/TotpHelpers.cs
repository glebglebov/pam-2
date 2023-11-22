using QRCoder;

namespace PAM.TOTP;

public static class TotpHelpers
{
    private const string Issuer = "PAM SYSTEM";

    public static string GetTotpQrCodeAsBase64(string secretKey, string accountName)
    {
        var uri = GetTotpUri(secretKey, Issuer, accountName);
        var qrCodeBytes = GetQrCodeAsBytes(uri);
        return Convert.ToBase64String(qrCodeBytes);
    }

    private static string GetTotpUri(string secretKey, string issuer, string accountName)
        => $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(accountName)}?secret={secretKey}&issuer={Uri.EscapeDataString(issuer)}";

    private static byte[] GetQrCodeAsBytes(string data)
    {
        var generator = new QRCodeGenerator();
        var qrCodeData = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(10);
    }
}
