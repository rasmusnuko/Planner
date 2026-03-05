using HomePlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace HomePlanner.Data;

public static class DbInitializer
{
    public static async Task MigrateAsync(AppDbContext db)
    {
        // Add SourceUrl column to Recipes if it doesn't exist yet
        try
        {
            await db.Database.ExecuteSqlRawAsync(
                "ALTER TABLE Recipes ADD COLUMN SourceUrl TEXT");
        }
        catch { /* column already exists on an existing database */ }

        // Create sleep tracking tables (idempotent)
        await db.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS SleepLogs (
                Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                Date     TEXT NOT NULL UNIQUE,
                WakeTime TEXT,
                BedTime  TEXT,
                Notes    TEXT
            )
            """);

        await db.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS SleepNaps (
                Id         INTEGER PRIMARY KEY AUTOINCREMENT,
                SleepLogId INTEGER NOT NULL REFERENCES SleepLogs(Id) ON DELETE CASCADE,
                StartTime  TEXT NOT NULL,
                EndTime    TEXT
            )
            """);
    }

    public static void Seed(AppDbContext db)
    {
        if (db.Users.Any()) return;

        var users = new[]
        {
            new User
            {
                Username     = Env("USER1_USERNAME",     "user1"),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Env("USER1_PASSWORD", "password1")),
                DisplayName  = Env("USER1_DISPLAY_NAME", "User 1"),
                Color        = Env("USER1_COLOR",        "#4f46e5")
            },
            new User
            {
                Username     = Env("USER2_USERNAME",     "user2"),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Env("USER2_PASSWORD", "password2")),
                DisplayName  = Env("USER2_DISPLAY_NAME", "User 2"),
                Color        = Env("USER2_COLOR",        "#ec4899")
            }
        };

        db.Users.AddRange(users);
        db.SaveChanges();
        Console.WriteLine($"[HomePlanner] Seeded users: {users[0].Username}, {users[1].Username}");
    }

    private static string Env(string key, string fallback) =>
        Environment.GetEnvironmentVariable(key) ?? fallback;
}
