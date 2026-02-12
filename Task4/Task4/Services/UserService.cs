using Microsoft.EntityFrameworkCore;
using Task4.Data;
using Task4.Models;

namespace Task4.Services;

public sealed class UserService(ApplicationDbContext context)
{
    public async Task PasswordRequestAsync(PasswordResetRequest request)
    {
        context.PasswordResetRequests.Add(request);
        await context.SaveChangesAsync();
    }

    public async Task<PasswordResetRequest?> GetLatestPasswordResetRequestAsync(int userId, string tokenHash)
        => await context.PasswordResetRequests
            .Where(r => r.UserId == userId && r.TokenHash == tokenHash)
            .OrderByDescending(r => r.CreatedAtUtc)
            .FirstOrDefaultAsync();

    public async Task MarkPasswordResetRequestUsedAsync(PasswordResetRequest request)
    {
        request.UsedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    public async Task UpdatePasswordHashAsync(User user, string newPasswordHash)
    {
        user.PasswordHash = newPasswordHash;
        await context.SaveChangesAsync();
    }

    public async Task<List<User>> GetAllUsers()
    {
        var users = await context.Users
            .OrderByDescending(u => u.LastSeen)
            .ToListAsync();

        return users;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
        => await context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User> GetUserByIdAsync(int id)
        => await CheckUserIsExistAsync(id);

    public async Task AddUserAsync(User user)
    {
        await context.Users.AddAsync(user);

        await context.SaveChangesAsync();
    }

    public async Task UpdateUsersStatusAsync(List<int> ids, UserStatus userStatus)
    {
        var users = await context.Users
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();

        foreach (var user in users)
        {
            user.Status = userStatus;
        }

        await context.SaveChangesAsync();
    }

    public async Task UpdateUserStatusAsync(int userId, UserStatus userStatus)
    {
        var user = await CheckUserIsExistAsync(userId);

        user.Status = userStatus;
        await context.SaveChangesAsync();
    }

    public async Task UpdateLastSeenAsync(int userId)
    {
        var user = await CheckUserIsExistAsync(userId);

        user.LastSeen = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    public async Task DeleteUnverifiedUsersAsync()
    {
        var unverifiedUsers = await context.Users
            .Where(u => u.Status == UserStatus.Unverified)
            .ToListAsync();

        context.Users.RemoveRange(unverifiedUsers);
        await context.SaveChangesAsync();
    }

    public async Task DeleteSelectedUsersAsync(List<int> ids)
    {
        var users = await context.Users
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();

        context.Users.RemoveRange(users);
        await context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await CheckUserIsExistAsync(id);

        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }

    public async Task<bool> IsUserActiveAsync(int id)
    {
        var user = await CheckUserIsExistAsync(id);

        return user.Status == UserStatus.Active;
    }

    private async Task<User> CheckUserIsExistAsync(int id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

        return user ?? throw new ArgumentNullException("User not found.");
    }
}
