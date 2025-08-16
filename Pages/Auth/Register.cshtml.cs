using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;

namespace SIMS.Pages.Auth
{
    [Authorize(Roles = "Admin")]
    public class RegisterModel : PageModel
    {
        private readonly IAuthenticationService _authService;
        public RegisterModel(IAuthenticationService authService) => _authService = authService;

        [BindProperty] public string Name { get; set; } = string.Empty;
        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string Password { get; set; } = string.Empty;
        [BindProperty] public UserRole Role { get; set; } = UserRole.Student;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "All fields are required.";
                return Page();
            }

            User user = Role switch
            {
                UserRole.Admin => new SIMS.Models.Admin(),
                UserRole.Faculty => new Faculty(),
                _ => new Student()
            };
            user.Name = Name;
            user.Email = Email;
            user.Role = Role;

            var result = await _authService.RegisterAsync(user, Password);
            if (!result)
            {
                ErrorMessage = "Email already exists.";
                return Page();
            }

            SuccessMessage = $"User '{Name}' with role '{Role}' created successfully.";
            return RedirectToPage("/Users/Index");
        }
    }
}
