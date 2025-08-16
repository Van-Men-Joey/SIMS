using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Interfaces;
using SIMS.Models;
using SIMS.Services;
using SIMS.ViewModels;

namespace SIMS.Pages.Admin.Users
{
    public class DeleteModel : PageModel
    {
        private readonly IUserService _userService;

        public DeleteModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public UserViewModel UserViewModel { get; set; } = new(); // Khởi tạo để tránh null

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Auth/Login");

            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            UserViewModel = new UserViewModel
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
            if (string.IsNullOrEmpty(UserViewModel.Id))
                return BadRequest();

            var userFromDb = await _userService.GetByIdAsync(UserViewModel.Id);
            if (userFromDb == null) return NotFound();

            await _userService.DeleteAsync(UserViewModel.Id);
            return RedirectToPage("Index");
        }
    }
}
