using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SIMS.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            // Nếu chưa login → vào trang Login
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Nếu đã login → hiển thị trang chính
            return Page();
        }
    }
}
