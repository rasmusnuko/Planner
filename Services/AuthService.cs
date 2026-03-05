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
        return await db.Users.ToListAsync();
    }
}
