using HomePlanner.Data;
using HomePlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace HomePlanner.Services;

public class MealService(IDbContextFactory<AppDbContext> factory)
{
    public async Task<List<MealPlan>> GetWeekAsync(DateOnly weekStart)
    {
        await using var db = await factory.CreateDbContextAsync();
        var weekEnd = weekStart.AddDays(6);
        return await db.MealPlans
            .Include(m => m.Recipe)
            .Where(m => m.Date >= weekStart && m.Date <= weekEnd)
            .ToListAsync();
    }

    public async Task<MealPlan?> GetTodayAsync(DateOnly today)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.MealPlans
            .Include(m => m.Recipe)
            .FirstOrDefaultAsync(m => m.Date == today);
    }

    public async Task SetMealAsync(DateOnly date, int? recipeId, string? customMeal, string? notes)
    {
        await using var db = await factory.CreateDbContextAsync();
        var existing = await db.MealPlans.FirstOrDefaultAsync(m => m.Date == date);
        if (existing is null)
        {
            db.MealPlans.Add(new MealPlan
            {
                Date = date, RecipeId = recipeId, CustomMeal = customMeal, Notes = notes
            });
        }
        else
        {
            existing.RecipeId = recipeId;
            existing.CustomMeal = customMeal;
            existing.Notes = notes;
        }
        await db.SaveChangesAsync();
    }

    public async Task ClearMealAsync(DateOnly date)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.MealPlans.Where(m => m.Date == date).ExecuteDeleteAsync();
    }
}
