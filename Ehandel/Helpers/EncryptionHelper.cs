using System.Security.Cryptography;
using System.Text;

namespace Ehandel.Helpers;

/// <summary>
/// Handles encryption and decryption of customer email.
/// Encrypted data can be restored back to plain text.
/// </summary>
public static class EncryptionHelper
{
    private static readonly byte[] Key =
        Encoding.UTF8.GetBytes("12345678901234567890123456789012");

    private static readonly byte[] Iv =
        Encoding.UTF8.GetBytes("1234567890123456");

    /// <summary>
    /// Encrypts plain text before saving it to the database.
    /// </summary>
    public static string Encrypt(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = Iv;

        var bytes = Encoding.UTF8.GetBytes(text);
        var encryptor = aes.CreateEncryptor();
        var encryptedBytes = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

        return Convert.ToBase64String(encryptedBytes);
    }

    /// <summary>
    /// Decrypts text when reading it from the database.
    /// </summary>
    public static string Decrypt(string encryptedText)
    {
        if (string.IsNullOrWhiteSpace(encryptedText))
            return encryptedText;

        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = Iv;

        var bytes = Convert.FromBase64String(encryptedText);
        var decryptor = aes.CreateDecryptor();
        var decryptedBytes = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}