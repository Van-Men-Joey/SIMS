using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;

namespace SIMS.Pages.Faculty_
{
    public class IndexModel : PageModel
    {
        private readonly ICourseService _courseService;

        public IndexModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public IEnumerable<Course> Courses { get; set; } = new List<Course>();

        public async Task OnGetAsync()
        {
            // Lấy FacultyId từ session đăng nhập
            var facultyId = HttpContext.Session.GetString("UserId");
            // Giả sử UserId ở session là FacultyId của giáo viên

            if (!string.IsNullOrEmpty(facultyId))
            {
                Courses = await _courseService.GetByFacultyIdAsync(facultyId);
            }
        }
    }
}
