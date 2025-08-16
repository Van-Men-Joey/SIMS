using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;
using SIMS.Interfaces;

namespace SIMS.Pages.Faculty_.Assignments
{
    public class IndexModel : PageModel
    {
        private readonly IAssignmentService _assignmentService;
        private readonly ICourseService _courseService;

        public IndexModel(IAssignmentService assignmentService, ICourseService courseService)
        {
            _assignmentService = assignmentService;
            _courseService = courseService;
        }

        public List<Assignment> Assignments { get; set; } = new();

        public async Task OnGetAsync()
        {
            var facultyId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(facultyId))
            {
                Assignments = new List<Assignment>();
                return;
            }

            // Lấy tất cả khóa học của giảng viên
            var coursesOfFaculty = await _courseService.GetByFacultyIdAsync(facultyId);
            var assignments = new List<Assignment>();

            // Lấy bài tập theo từng khóa học
            foreach (var course in coursesOfFaculty)
            {
                var courseAssignments = await _assignmentService.GetByCourseIdAsync(course.Id);
                assignments.AddRange(courseAssignments);
            }

            Assignments = assignments;
        }

        // Xử lý xóa bài tập
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                await _assignmentService.DeleteAsync(id);
            }
            return RedirectToPage();
        }
    }
}
