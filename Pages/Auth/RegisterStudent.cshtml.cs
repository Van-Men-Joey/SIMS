using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;

namespace SIMS.Pages.Auth
{
    public class RegisterStudentModel : PageModel
    {
        private readonly IAuthenticationService _authService;
        public RegisterStudentModel(IAuthenticationService authService) => _authService = authService;

        [BindProperty] public string Name { get; set; } = string.Empty;
        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "All fields are required.";
                return Page();
            }

            var student = new Student
            {
                Name = Name,
                Email = Email,
                Role = UserRole.Student
            };

            var result = await _authService.RegisterAsync(student, Password);
            if (!result)
            {
                ErrorMessage = "Email already exists.";
                return Page();
            }

            SuccessMessage = "Registration successful! You can now login.";
            return RedirectToPage("/Auth/Login");
        }
    }
}
