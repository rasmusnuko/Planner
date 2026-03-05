using HomePlanner.Data;
using HomePlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace HomePlanner.Services;

public class AuthService(IDbContextFactory<AppDbContext> factory)
{
    public async Task<User?> VerifyPasswordAsync(string username, string password)
    {
        await using var db = await factory.CreateDbContextAsync();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;
        return user;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Users.OrderBy(u => u.Id).ToListAsync();
    }

    public async Task<User> CreateUserAsync(
        string username, string displayName, string password, string color)
    {
        await using var db = await factory.CreateDbContextAsync();
        if (await db.Users.AnyAsync(u => u.Username == username))
            throw new InvalidOperationException($"Username '{username}' is already taken.");

        var user = new User
        {
            Username     = username.Trim(),
            DisplayName  = displayName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Color        = color
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    public async Task DeleteUserAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.Users.Where(u => u.Id == id).ExecuteDeleteAsync();
    }

    public async Task ChangePasswordAsync(int id, string newPassword)
    {
        await using var db = await factory.CreateDbContextAsync();
        var user = await db.Users.FindAsync(id)
            ?? throw new KeyNotFoundException($"User {id} not found.");
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await db.SaveChangesAsync();
    }
}
