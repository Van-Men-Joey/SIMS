using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;

namespace SIMS.Pages.Student_.Courses
{
    public class MyCoursesStudentModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;

        public MyCoursesStudentModel(
            ICourseService courseService,
            IEnrollmentService enrollmentService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
        }

        public IEnumerable<Course> Courses { get; set; } = new List<Course>();

        public async Task OnGetAsync()
        {
            var studentId = HttpContext.Session.GetString("UserId");

            if (!string.IsNullOrEmpty(studentId))
            {
                // Lấy tất cả enrollment của student
                var enrollments = await _enrollmentService.GetByStudentIdAsync(studentId);

                // Lọc những enrollment đã được chấp nhận
                var acceptedCourseIds = enrollments
                    .Where(e => e.Status == EnrollmentStatus.Accepted)
                    .Select(e => e.CourseId)
                    .ToList();

                // Lấy chi tiết course theo danh sách ID
                var allCourses = await _courseService.GetAllAsync();
                Courses = allCourses
                    .Where(c => acceptedCourseIds.Contains(c.Id))
                    .OrderBy(c => c.Title)
                    .ToList();
            }
        }
    }
}
