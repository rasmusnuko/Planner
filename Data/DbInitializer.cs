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

}
