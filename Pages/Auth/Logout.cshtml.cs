using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SIMS.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Xóa toàn bộ Session
            HttpContext.Session.Clear();

            // Chuyển hướng về trang Login
            return RedirectToPage("/Auth/Login");
        }
    }
}
