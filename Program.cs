using HomePlanner.Components;
using HomePlanner.Data;
using HomePlanner.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ─── Database ─────────────────────────────────────────────────────────────────

var dbPath = Environment.GetEnvironmentVariable("DATABASE_PATH") ?? "./data/planner.db";
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(dbPath))!);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// ─── Authentication ────────────────────────────────────────────────────────────

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/login";
        o.ExpireTimeSpan = TimeSpan.FromDays(30);
        o.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// ─── Razor Pages + Blazor ──────────────────────────────────────────────────────

builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ─── Domain services ───────────────────────────────────────────────────────────

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CalendarService>();
builder.Services.AddScoped<ShoppingService>();
builder.Services.AddScoped<TodoService>();
builder.Services.AddScoped<MealService>();
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<MilestoneService>();

// ─── Build & seed ──────────────────────────────────────────────────────────────

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    await using var ctx = await factory.CreateDbContextAsync();
    await ctx.Database.EnsureCreatedAsync();
    DbInitializer.Seed(ctx);
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
