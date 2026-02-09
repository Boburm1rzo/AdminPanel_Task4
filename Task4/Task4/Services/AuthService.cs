using Microsoft.AspNetCore.Identity;
using Task4.Models;

namespace Task4.Services;

public sealed class AuthService(
    IPasswordHasher<User> passwordHasher,
    UserService userService)
{
    public async Task<User> Login(string email, string password)
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
        catch (Exception)
        {
            throw;
        }
    }

    public async Task Register(User user, string password)
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

    public Task ForgotPassword(string email)
    {
        return Task.CompletedTask;

    }
}
