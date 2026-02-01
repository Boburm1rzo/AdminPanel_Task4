using Microsoft.EntityFrameworkCore;
using Task4.Data;
using Task4.Models;

namespace Task4.Services;

public sealed class UserService(ApplicationDbContext context)
{
    public async Task<List<User>> GetAllUsers()
    {
        var users = await context.Users
            .OrderByDescending(u => u.LastSeen)
            .ToListAsync();

        return users;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user is null)
        {
            throw new ArgumentNullException("User not found.");
        }

        return user;
    }

    public async Task AddUserAsync(User user)
    {
        await context.Users.AddAsync(user);

        await context.SaveChangesAsync();
    }

    public async Task UpdateUserStatusAsync(List<int> ids, UserStatus userStatus)
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

    public async Task DeleteUnverifiedUsersAsync()
    {
        var unverifiedUsers = await context.Users
            .Where(u => u.Status == UserStatus.Unverified)
            .ToListAsync();

        context.Users.RemoveRange(unverifiedUsers);
        await context.SaveChangesAsync();
    }

    public async Task DeleteUser(int id)
    {
        var user = await CheckUserIsExistAsync(id);

        context.Users.Remove(user);
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
