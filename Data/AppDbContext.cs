using HomePlanner.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HomePlanner.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<CalendarEvent> Events => Set<CalendarEvent>();
    public DbSet<ShoppingItem> ShoppingItems => Set<ShoppingItem>();
    public DbSet<TodoList> TodoLists => Set<TodoList>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<MealPlan> MealPlans => Set<MealPlan>();
    public DbSet<Milestone> Milestones => Set<Milestone>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        // Unique date per meal plan entry
        model.Entity<MealPlan>()
            .HasIndex(m => m.Date)
            .IsUnique();

        // Store List<string> ingredients as a JSON column
        model.Entity<Recipe>()
            .Property(r => r.Ingredients)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            );
    }
}
