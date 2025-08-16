using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;

namespace SIMS.Pages.Student_.Enrollments
{
    public class EnrollmentStudentModel : PageModel
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseService _courseService;

        public EnrollmentStudentModel(IEnrollmentService enrollmentService, ICourseService courseService)
        {
            _enrollmentService = enrollmentService;
            _courseService = courseService;
        }

        public IEnumerable<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public IEnumerable<Course> AvailableCourses { get; set; } = new List<Course>();

        [BindProperty]
        public string SelectedCourseId { get; set; }

        public async Task OnGetAsync()
        {
            var studentId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(studentId))
            {
                Enrollments = await _enrollmentService.GetByStudentIdAsync(studentId);
                var allCourses = await _courseService.GetAllAsync();
                AvailableCourses = allCourses.Where(c => !Enrollments.Any(e => e.CourseId == c.Id));
            }
        }

        public async Task<IActionResult> OnPostEnrollAsync()
        {
            var studentId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(SelectedCourseId))
            {
                TempData["ErrorMessage"] = "Invalid enrollment data.";
                return RedirectToPage();
            }

            var matchedCourse = (await _courseService.GetAllAsync()).FirstOrDefault(c => c.Id == SelectedCourseId);
            if (matchedCourse == null)
            {
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToPage();
            }

            var existing = await _enrollmentService.GetByStudentIdAsync(studentId);
            if (existing.Any(e => e.CourseId == matchedCourse.Id))
            {
                TempData["ErrorMessage"] = "You have already requested enrollment for this course.";
                return RedirectToPage();
            }

            var newEnrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = matchedCourse.Id,
                EnrolledDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Pending
            };

            await _enrollmentService.AddAsync(newEnrollment);
            TempData["SuccessMessage"] = "Enrollment request sent. Waiting for teacher approval.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnenrollAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var enrollment = await _enrollmentService.GetByIdAsync(id);
                if (enrollment != null && enrollment.Status != EnrollmentStatus.Accepted)
                {
                    await _enrollmentService.DeleteAsync(id);
                    TempData["SuccessMessage"] = "Enrollment request canceled.";
                }
                else
                {
                    TempData["ErrorMessage"] = "You cannot cancel an approved enrollment.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid enrollment ID.";
            }

            return RedirectToPage();
        }
    }
}
