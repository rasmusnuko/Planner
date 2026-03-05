using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomePlanner.Pages;

public class AdminLogoutModel : PageModel
{
    public IActionResult OnGet() => RedirectToPage("/AdminLogin");

    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync("AdminScheme");
        return Redirect("/");
    }
}
