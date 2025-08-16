using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;

namespace SIMS.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IAuthenticationService _authService;
        public LoginModel(IAuthenticationService authService) => _authService = authService;

        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string Password { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(role))
            {
                if (role == "Admin") Response.Redirect("/Admin/Dashboard");
                else if (role == "Faculty") Response.Redirect("/Faculty_/Dashboard");
                else Response.Redirect("/Student_/Dashboard");
            }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both Email and Password.";
                return Page();
            }

            var user = await _authService.LoginAsync(Email, Password);
            if (user == null)
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Role", user.Role.ToString());
            HttpContext.Session.SetString("UserName", user.Name); // ✅ THÊM DÒNG NÀY


            return user.Role switch
            {
                UserRole.Admin => RedirectToPage("/Admin/Dashboard"),
                UserRole.Faculty => RedirectToPage("/Faculty_/Index"),
                _ => RedirectToPage("/Student_/Index")
            };
        }
    }
}
