using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public List<User> Users { get; set; } = new();

        // AllRoles là danh sách enum UserRole (distinct từ users)
        public List<UserRole> AllRoles { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        // Nullable để cho phép không chọn Role nào (tức All Roles)
        [BindProperty(SupportsGet = true)]
        public UserRole? SelectedRole { get; set; }

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public async Task OnGetAsync()
        {
            var users = await _userService.GetAllAsync();

            // Lấy danh sách role enum có trong users (distinct)
            AllRoles = users
                .Where(u => u.Role != null)
                .Select(u => u.Role)
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            // Filter tìm kiếm theo term
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                users = users.Where(u =>
                    (u.Id != null && u.Id.ToLower().Contains(term)) ||
                    (u.Name != null && u.Name.ToLower().Contains(term)) ||
                    (u.Email != null && u.Email.ToLower().Contains(term))
                );
            }

            // Filter theo Role nếu có chọn
            if (SelectedRole.HasValue)
            {
                users = users.Where(u => u.Role == SelectedRole.Value);
            }

            Users = users.ToList();
        }

        // API autocomplete tìm kiếm
        public async Task<IActionResult> OnGetSearchUsers(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                return new JsonResult(new List<object>());
            }

            term = term.Trim().ToLower();

            var users = await _userService.GetAllAsync();

            var results = users
                .Where(u =>
                    (u.Id != null && u.Id.ToLower().Contains(term)) ||
                    (u.Name != null && u.Name.ToLower().Contains(term)) ||
                    (u.Email != null && u.Email.ToLower().Contains(term))
                )
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    email = u.Email
                })
                .Take(10);

            return new JsonResult(results);
        }
    }
}
