using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIMS.Data;

namespace SIMS.Pages.Student_
{
    public class DashboardModel : PageModel
    {
        private readonly SIMSContext _context;

        public DashboardModel(SIMSContext context)
        {
            _context = context;
        }

        // Các thuộc tính để hiển thị lên dashboard
        public int TotalCourses { get; set; }
        public int TotalAssignments { get; set; }
        public int TotalSubmissions { get; set; }
        public int TotalReports { get; set; }
        public List<string> ChartLabels { get; set; } = new List<string>();
        public List<int> ChartData { get; set; } = new List<int>();

        public async Task<IActionResult> OnGetAsync()
        {
            var facultyId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(facultyId))
            {
                return RedirectToPage("/Login");
            }

            // 1. Đếm số khóa học thuộc về Faculty
            TotalCourses = await _context.Courses
                .Where(c => c.FacultyId == facultyId)
                .CountAsync();

            // 2. Đếm số bài tập trong các khóa học thuộc Faculty
            TotalAssignments = await _context.Assignments
                .Include(a => a.Course)
                .Where(a => a.Course != null && a.Course.FacultyId == facultyId)
                .CountAsync();

            // 3. Đếm số bài nộp liên quan tới các bài tập của Faculty
            TotalSubmissions = await _context.Submissions
                .Include(s => s.Assignment)
                .ThenInclude(a => a.Course)
                .Where(s => s.Assignment != null && s.Assignment.Course != null && s.Assignment.Course.FacultyId == facultyId)
                .CountAsync();

            // 4. Đếm số báo cáo trong các khóa học của Faculty
            TotalReports = await _context.Reports
                .Include(r => r.Course)
                .Where(r => r.Course != null && r.Course.FacultyId == facultyId)
                .CountAsync();

            return Page();
        }
    }
}
