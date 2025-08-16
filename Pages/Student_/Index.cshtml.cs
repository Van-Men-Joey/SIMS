using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SIMS.Pages.Student_
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Kiểm tra đăng nhập
            var studentId = HttpContext.Session.GetString("UserId");
            if (studentId == null)
            {
                return RedirectToPage("/Login");
            }

            // Nếu cần thêm logic, bạn có thể thêm vào đây

            return Page();
        }
    }
}
