using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomePlanner.Pages;

public class AdminLoginModel : PageModel
{
    [BindProperty] public string Password { get; set; } = string.Empty;
    public string? Error { get; private set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
        if (adminPassword is null)
        {
            Error = "ADMIN_PASSWORD is not configured on this server.";
            return Page();
        }

        if (Password != adminPassword)
        {
            Error = "Incorrect admin password.";
            return Page();
        }

        var identity  = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, "admin")],
            "AdminScheme");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("AdminScheme", principal,
            new AuthenticationProperties { IsPersistent = true });

        return Redirect("/admin");
    }
}
