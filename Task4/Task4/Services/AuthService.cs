using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;
using Task4.Models;

namespace Task4.Services;

public sealed class AuthService(
    IPasswordHasher<User> passwordHasher,
    UserService userService,
    EmailService emailService)
{
    public async Task<User> LoginAsync(string email, string password)
    {
        try
        {
            var user = await userService.GetUserByEmailAsync(email);

            if (user is null)
                throw new UnauthorizedAccessException("No users found with this email address.");

            if (user.Status == UserStatus.Blocked)
                throw new UnauthorizedAccessException("User is blocked.");

            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result != PasswordVerificationResult.Success)
                throw new UnauthorizedAccessException("Invalid email or password.");

            return user;
        }
        catch
        {
            throw;
        }
    }

    public async Task RegisterAsync(User user, string password)
    {
        try
        {
            user.PasswordHash = passwordHasher.HashPassword(user, password);

            await userService.AddUserAsync(user);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<string?> CreateResetTokenAsync(string email)
    {
        var user = await userService.GetUserByEmailAsync(email);
        if (user is null)
            return null;

        var token = GenerateSecureToken();
        var tokenHash = Sha256(token);

        var req = new PasswordResetRequest
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
            UsedAtUtc = null
        };

        await userService.PasswordRequestAsync(req);

        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
    }

    public async Task<bool> ResetPasswordAsync(string email, string tokenFromQuery, string newPassword)
    {
        var user = await userService.GetUserByEmailAsync(email);
        if (user is null) return false;

        if (!TryDecodeToken(tokenFromQuery, out var token))
            return false;

        var tokenHash = Sha256(token);

        var req = await userService.GetLatestPasswordResetRequestAsync(user.Id, tokenHash);
        if (req is null) return false;
        if (req.UsedAtUtc is not null) return false;
        if (req.ExpiresAtUtc < DateTime.UtcNow) return false;

        var newHash = passwordHasher.HashPassword(user, newPassword);

        await userService.UpdatePasswordHashAsync(user, newHash);
        await userService.MarkPasswordResetRequestUsedAsync(req);

        return true;
    }

    private static bool TryDecodeToken(string tokenFromQuery, out string token)
    {
        token = string.Empty;

        try
        {
            var raw = WebEncoders.Base64UrlDecode(tokenFromQuery);
            token = Encoding.UTF8.GetString(raw);
            return !string.IsNullOrWhiteSpace(token);
        }
        catch
        {
            return false;
        }
    }

    private static string GenerateSecureToken()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes);
    }

    private static string Sha256(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}
