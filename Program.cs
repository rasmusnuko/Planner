using HomePlanner.Components;
using HomePlanner.Data;
using HomePlanner.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ─── Database ─────────────────────────────────────────────────────────────────

var dbPath = Environment.GetEnvironmentVariable("DATABASE_PATH") ?? "./data/planner.db";
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(dbPath))!);

var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD")
    ?? throw new InvalidOperationException(
        "DB_PASSWORD environment variable is required. Set it in docker-compose.yml.");

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath};Password={dbPassword}"));

// ─── Authentication ────────────────────────────────────────────────────────────

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath      = "/login";
        o.ExpireTimeSpan = TimeSpan.FromDays(30);
        o.SlidingExpiration = true;
    })
    .AddCookie("AdminScheme", o =>
    {
        o.Cookie.Name       = "admin_auth";
        o.LoginPath         = "/admin-login";
        o.ExpireTimeSpan    = TimeSpan.FromHours(4);
        o.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(o =>
    o.AddPolicy("Admin", p =>
        p.AddAuthenticationSchemes("AdminScheme")
         .RequireAuthenticatedUser()));

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
builder.Services.AddScoped<SleepService>();
builder.Services.AddHttpClient<RecipeScraperService>();

// ─── Build & migrate ───────────────────────────────────────────────────────────

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    await using var ctx = await factory.CreateDbContextAsync();
    await ctx.Database.EnsureCreatedAsync();
    await DbInitializer.MigrateAsync(ctx);
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
