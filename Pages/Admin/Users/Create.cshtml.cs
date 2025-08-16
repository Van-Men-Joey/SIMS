using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Interfaces;
using SIMS.Models;
using SIMS.Services;
using SIMS.ViewModels;

namespace SIMS.Pages.Admin.Users
{
    public class CreateModel : PageModel
    {
        private readonly IUserService _userService;

        public CreateModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public UserViewModel Input { get; set; } = new();

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Auth/Login");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = CreateUserFromInput(Input);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid user role.");
                return Page();
            }

            var existingUser = await _userService.GetByEmailAsync(Input.Email.Trim().ToLower());
            if (existingUser != null)
            {
                ModelState.AddModelError("Input.Email", "Email đã tồn tại trong hệ thống.");
                return Page();
            }

            // Gọi phương thức tạo ID và thêm user mới
            await _userService.AddNewUserAsync(user);

            return RedirectToPage("Index");
        }

        private User? CreateUserFromInput(UserViewModel input)
        {
            User user = input.Role switch
            {
                UserRole.Admin => new Models.Admin(),
                UserRole.Faculty => new Faculty(),
                UserRole.Student => new Student(),
                _ => null
            };

            if (user == null) return null;

            // Bỏ gán Id ở đây, vì ID sẽ được tạo trong service AddNewUserAsync
            // user.Id = Guid.NewGuid().ToString();

            user.Name = input.Name.Trim();
            user.Email = input.Email.Trim().ToLower();
            user.Role = input.Role;
            user.Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim();

            if (string.IsNullOrWhiteSpace(input.Password))
            {
                ModelState.AddModelError("Input.Password", "Mật khẩu không được để trống.");
                return null;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.Password);

            return user;
        }
    }
}
