using HomePlanner.Data;
using HomePlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace HomePlanner.Services;

public class SleepService(IDbContextFactory<AppDbContext> factory)
{
    public async Task<List<SleepLog>> GetRecentAsync(int count = 60)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.SleepLogs
            .Include(l => l.Naps)
            .OrderByDescending(l => l.Date)
            .Take(count)
            .ToListAsync();
    }

    public async Task<SleepLog?> GetByDateAsync(DateOnly date)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.SleepLogs
            .Include(l => l.Naps)
            .FirstOrDefaultAsync(l => l.Date == date);
    }

    public async Task<SleepLog> UpsertLogAsync(DateOnly date, TimeOnly? wake, TimeOnly? bed, string? notes)
    {
        await using var db = await factory.CreateDbContextAsync();
        var log = await db.SleepLogs.FirstOrDefaultAsync(l => l.Date == date);
        if (log is null)
        {
            log = new SleepLog { Date = date };
            db.SleepLogs.Add(log);
        }
        log.WakeTime = wake;
        log.BedTime  = bed;
        log.Notes    = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        await db.SaveChangesAsync();
        return log;
    }

    public async Task AddNapAsync(int logId, TimeOnly start, TimeOnly? end)
    {
        await using var db = await factory.CreateDbContextAsync();
        db.SleepNaps.Add(new SleepNap
        {
            SleepLogId = logId,
            StartTime  = start,
            EndTime    = end
        });
        await db.SaveChangesAsync();
    }

    public async Task DeleteNapAsync(int napId)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.SleepNaps.Where(n => n.Id == napId).ExecuteDeleteAsync();
    }

    public async Task DeleteLogAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.SleepLogs.Where(l => l.Id == id).ExecuteDeleteAsync();
    }
}
