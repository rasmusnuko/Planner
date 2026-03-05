using HomePlanner.Data;
using HomePlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace HomePlanner.Services;

public class RecipeService(IDbContextFactory<AppDbContext> factory)
{
    public async Task<List<Recipe>> GetAllAsync(string? query = null)
    {
        await using var db = await factory.CreateDbContextAsync();
        var q = db.Recipes.Include(r => r.CreatedBy).AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(r => r.Title.Contains(query));
        return await q.OrderBy(r => r.Title).ToListAsync();
    }

    public async Task<Recipe?> GetByIdAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Recipes.Include(r => r.CreatedBy).FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Recipe> CreateAsync(Recipe recipe)
    {
        await using var db = await factory.CreateDbContextAsync();
        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();
        return recipe;
    }

    public async Task UpdateAsync(Recipe recipe)
    {
        await using var db = await factory.CreateDbContextAsync();
        db.Recipes.Update(recipe);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.Recipes.Where(r => r.Id == id).ExecuteDeleteAsync();
    }

    public async Task<List<Recipe>> GetAllSimpleAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Recipes.OrderBy(r => r.Title).ToListAsync();
    }
}
