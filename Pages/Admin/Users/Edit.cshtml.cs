using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Interfaces;
using SIMS.Models;
using SIMS.Services;
using SIMS.ViewModels;

namespace SIMS.Pages.Admin.Users
{
    public class EditModel : PageModel
    {
        private readonly IUserService _userService;

        public EditModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public UserViewModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Auth/Login");

            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            Input = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Description = user.Description
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userService.GetByIdAsync(Input.Id);
            if (user == null)
                return NotFound();

            user.Name = Input.Name.Trim();
            user.Email = Input.Email.Trim().ToLower();
            user.Role = Input.Role;
            user.Description = string.IsNullOrWhiteSpace(Input.Description) ? null : Input.Description.Trim();

            if (!string.IsNullOrWhiteSpace(Input.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Input.Password.Trim());
            }

            await _userService.UpdateAsync(user);
            return RedirectToPage("Index");
        }
    }
}
