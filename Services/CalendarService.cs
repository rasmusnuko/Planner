using HomePlanner.Data;
using HomePlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace HomePlanner.Services;

public class CalendarService(IDbContextFactory<AppDbContext> factory)
{
    public async Task<List<CalendarEvent>> GetEventsForMonthAsync(int year, int month)
    {
        await using var db = await factory.CreateDbContextAsync();
        var start = new DateOnly(year, month, 1);
        var end = start.AddMonths(1).AddDays(-1);
        return await db.Events
            .Include(e => e.AssignedTo)
            .Where(e => e.Date >= start && e.Date <= end)
            .OrderBy(e => e.Date).ThenBy(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<CalendarEvent>> GetEventsTodayAsync(DateOnly today)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Events
            .Include(e => e.AssignedTo)
            .Where(e => e.Date == today)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<CalendarEvent>> GetUpcomingAsync(DateOnly after, int limit = 5)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Events
            .Include(e => e.AssignedTo)
            .Where(e => e.Date > after)
            .OrderBy(e => e.Date)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddAsync(CalendarEvent ev)
    {
        await using var db = await factory.CreateDbContextAsync();
        db.Events.Add(ev);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.Events.Where(e => e.Id == id).ExecuteDeleteAsync();
    }
}
