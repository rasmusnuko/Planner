using HomePlanner.Data;
using HomePlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace HomePlanner.Services;

public class TodoService(IDbContextFactory<AppDbContext> factory)
{
    public async Task<List<TodoList>> GetListsAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.TodoLists
            .Include(l => l.Owner)
            .Include(l => l.Items)
            .OrderByDescending(l => l.IsShared)
            .ThenBy(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<TodoItem>> GetItemsAsync(int listId)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.TodoItems
            .Include(i => i.AssignedTo)
            .Where(i => i.ListId == listId)
            .OrderBy(i => i.Done)
            .ThenBy(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetTotalOpenCountAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.TodoItems.CountAsync(i => !i.Done);
    }

    public async Task<TodoList> CreateListAsync(string name, bool isShared, int? ownerId)
    {
        await using var db = await factory.CreateDbContextAsync();
        var list = new TodoList
        {
            Name = name,
            IsShared = isShared,
            OwnerId = isShared ? null : ownerId
        };
        db.TodoLists.Add(list);
        await db.SaveChangesAsync();
        return list;
    }

    public async Task DeleteListAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.TodoLists.Where(l => l.Id == id).ExecuteDeleteAsync();
    }

    public async Task AddItemAsync(TodoItem item)
    {
        await using var db = await factory.CreateDbContextAsync();
        db.TodoItems.Add(item);
        await db.SaveChangesAsync();
    }

    public async Task ToggleItemAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        var item = await db.TodoItems.FindAsync(id);
        if (item is not null)
        {
            item.Done = !item.Done;
            await db.SaveChangesAsync();
        }
    }

    public async Task DeleteItemAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.TodoItems.Where(i => i.Id == id).ExecuteDeleteAsync();
    }
}
