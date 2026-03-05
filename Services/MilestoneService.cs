using HomePlanner.Data;
using HomePlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace HomePlanner.Services;

public class MilestoneService(IDbContextFactory<AppDbContext> factory)
{
    public async Task<List<Milestone>> GetAllAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Milestones
            .Include(m => m.CreatedBy)
            .OrderByDescending(m => m.Date)
            .ThenByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Milestone>> GetRecentAsync(int count = 3)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Milestones
            .OrderByDescending(m => m.Date)
            .ThenByDescending(m => m.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task AddAsync(Milestone milestone)
    {
        await using var db = await factory.CreateDbContextAsync();
        db.Milestones.Add(milestone);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.Milestones.Where(m => m.Id == id).ExecuteDeleteAsync();
    }
}
