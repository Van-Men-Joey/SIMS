using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Pages.Faculty_.Submissions
{
    public class IndexModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly IAssignmentService _assignmentService;
        private readonly ISubmissionService _submissionService;

        public IndexModel(ICourseService courseService, IAssignmentService assignmentService, ISubmissionService submissionService)
        {
            _courseService = courseService;
            _assignmentService = assignmentService;
            _submissionService = submissionService;
        }

        public List<Course> Courses { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SelectedCourseId { get; set; }

        public List<Assignment> Assignments { get; set; } = new();

        public List<Submission> Submissions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var facultyId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(facultyId))
            {
                return RedirectToPage("/Auth/Login");
            }

            Courses = (await _courseService.GetByFacultyIdAsync(facultyId)).ToList();

            if (!string.IsNullOrEmpty(SelectedCourseId))
            {
                Assignments = (await _assignmentService.GetByCourseIdAsync(SelectedCourseId)).ToList();

                // Lấy submissions đã có Student và Assignment được populate trong service
                var submissions = await _submissionService.GetByCourseIdAsync(SelectedCourseId);
                Submissions = submissions.ToList();
            }

            return Page();
        }
    }
}
