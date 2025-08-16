using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Pages.Student_.Courses
{
    public class MyCoursesStudentDetailModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IAssignmentService _assignmentService;
        private readonly ISubmissionService _submissionService;

        public MyCoursesStudentDetailModel(
            ICourseService courseService,
            IUserService userService,
            IEnrollmentService enrollmentService,
            IAssignmentService assignmentService,
            ISubmissionService submissionService)
        {
            _courseService = courseService;
            _userService = userService;
            _enrollmentService = enrollmentService;
            _assignmentService = assignmentService;
            _submissionService = submissionService;
        }

        public Course Course { get; set; }
        public User Instructor { get; set; }
        public Enrollment Enrollment { get; set; }
        public List<Assignment> Assignments { get; set; } = new();
        public List<Submission> Submissions { get; set; } = new();

        // Map AssignmentId -> trạng thái
        public Dictionary<string, string> AssignmentStatuses { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var studentId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(studentId))
                return RedirectToPage("/Auth/Login");

            Course = await _courseService.GetByIdAsync(id);
            if (Course == null) return NotFound();

            Instructor = await _userService.GetByIdAsync(Course.FacultyId);

            Enrollment = (await _enrollmentService.GetByCourseIdAsync(id))
                            .FirstOrDefault(e => e.StudentId == studentId);

            Assignments = (await _assignmentService.GetByCourseIdAsync(id)).ToList();

            var allStudentSubmissions = await _submissionService.GetByStudentIdAsync(studentId);
            Submissions = allStudentSubmissions
                            .Where(s => Assignments.Any(a => a.Id == s.AssignmentId))
                            .ToList();

            // Xác định trạng thái từng assignment
            foreach (var assignment in Assignments)
            {
                var hasSubmitted = Submissions.Any(s => s.AssignmentId == assignment.Id);
                if (hasSubmitted)
                {
                    AssignmentStatuses[assignment.Id] = "Submitted";
                }
                else if (assignment.DueDate < DateTime.Now)
                {
                    AssignmentStatuses[assignment.Id] = "Overdue";
                }
                else
                {
                    AssignmentStatuses[assignment.Id] = "Pending";
                }
            }

            return Page();
        }
    }
}
