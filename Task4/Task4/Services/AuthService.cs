using Microsoft.AspNetCore.Identity;
using Task4.Models;

namespace Task4.Services;

public sealed class AuthService(
    EmailService emailService,
    IPasswordHasher<User> passwordHasher,
    UserService userService)
{
    public async Task Login(string email, string password)
    {
        try
        {
            var user = await userService.GetUserByEmailAsync(email);

            var verificatoin = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (verificatoin == PasswordVerificationResult.Success)
            {

            }
            else
            {
                // Invalid password
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
        }
        catch (Exception ex)
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
        catch (Exception ex)
        {
            throw;
        }
    }

    public Task LogOut()
    {
        return Task.CompletedTask;
    }

    public Task ForgotPassword(string email)
    {
        return Task.CompletedTask;

    }
}
