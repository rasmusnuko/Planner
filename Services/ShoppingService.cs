using HomePlanner.Data;
using HomePlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace HomePlanner.Services;

public class ShoppingService(IDbContextFactory<AppDbContext> factory)
{
    public static readonly string[] Categories =
        ["produce", "dairy", "meat", "bakery", "frozen", "pantry", "drinks", "other"];

    public async Task<List<ShoppingItem>> GetItemsAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.ShoppingItems
            .Include(i => i.AddedBy)
            .OrderBy(i => i.Checked)
            .ThenBy(i => i.Category)
            .ThenBy(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUncheckedCountAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.ShoppingItems.CountAsync(i => !i.Checked);
    }

    public async Task AddAsync(ShoppingItem item)
    {
        await using var db = await factory.CreateDbContextAsync();
        db.ShoppingItems.Add(item);
        await db.SaveChangesAsync();
    }

    public async Task ToggleAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        var item = await db.ShoppingItems.FindAsync(id);
        if (item is not null)
        {
            item.Checked = !item.Checked;
            await db.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.ShoppingItems.Where(i => i.Id == id).ExecuteDeleteAsync();
    }

    public async Task ClearCheckedAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.ShoppingItems.Where(i => i.Checked).ExecuteDeleteAsync();
    }
}
