using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;

namespace SIMS.Pages.Faculty_.Courses
{
    public class MyCoursesFacultyModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;

        public MyCoursesFacultyModel(
            ICourseService courseService,
            IEnrollmentService enrollmentService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
        }

        public List<CourseWithPendingCount> Courses { get; set; } = new();

        public async Task OnGetAsync()
        {
            var facultyId = HttpContext.Session.GetString("UserId");

            if (!string.IsNullOrEmpty(facultyId))
            {
                var courses = await _courseService.GetByFacultyIdAsync(facultyId);

                foreach (var course in courses)
                {
                    var enrollments = await _enrollmentService.GetByCourseIdAsync(course.Id);
                    int pendingCount = enrollments.Count(e => e.Status == EnrollmentStatus.Pending);

                    Courses.Add(new CourseWithPendingCount
                    {
                        Id = course.Id,
                        Title = course.Title,
                        Description = course.Description,
                        PendingCount = pendingCount
                    });
                }
            }
        }
    }

    public class CourseWithPendingCount
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PendingCount { get; set; }
    }
}
