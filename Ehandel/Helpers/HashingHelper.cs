using System.Security.Cryptography;

namespace Ehandel.Helpers;

/// <summary>
/// Handles password hashing with salt.
/// Passwords are never stored as plain text.
/// </summary>
public static class HashingHelper
{
    /// <summary>
    /// Creates a random salt.
    /// The salt makes each password hash unique.
    /// </summary>
    public static string GenerateSalt(int size = 16)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(size);
        return Convert.ToBase64String(saltBytes);
    }

    /// <summary>
    /// Creates a secure hash from a password and salt.
    /// </summary>
    public static string HashWithSalt(string password, string base64Salt)
    {
        var saltBytes = Convert.FromBase64String(base64Salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            saltBytes,
            100_000,
            HashAlgorithmName.SHA256);

        var hashBytes = pbkdf2.GetBytes(32);
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Checks if the entered password matches the saved hash.
    /// </summary>
    public static bool Verify(string password, string salt, string savedHash)
    {
        var newHash = HashWithSalt(password, salt);
        return newHash == savedHash;
    }
}