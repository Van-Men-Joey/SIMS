using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Interfaces;
using SIMS.Models;
using SIMS.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IAssignmentService _assignmentService;
        private readonly ISubmissionService _submissionService;

        public string AdminName { get; set; } = "";

        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public int TotalAssignments { get; set; }

        public List<string> StudentsPerCourseLabels { get; set; } = new();
        public List<int> StudentsPerCourseData { get; set; } = new();

        public List<string> AssignmentsPerCourseLabels { get; set; } = new();
        public List<int> AssignmentsPerCourseData { get; set; } = new();

        public List<string> SubmissionsPerAssignmentLabels { get; set; } = new();
        public List<int> SubmissionsPerAssignmentData { get; set; } = new();

        public List<double> AverageScorePerAssignmentData { get; set; } = new();

        public List<string> EnrollmentsOverTimeLabels { get; set; } = new();
        public List<int> EnrollmentsOverTimeData { get; set; } = new();

        public List<string> TopCoursesLabels { get; set; } = new();
        public List<int> TopCoursesData { get; set; } = new();

        public DashboardModel(
            IUserService userService,
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            IAssignmentService assignmentService,
            ISubmissionService submissionService)
        {
            _userService = userService;
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _assignmentService = assignmentService;
            _submissionService = submissionService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Auth/Login");

            AdminName = HttpContext.Session.GetString("UserName") ?? "Admin";

            var users = await _userService.GetAllAsync();
            var courses = await _courseService.GetAllAsync();
            var enrollments = await _enrollmentService.GetAllAsync();
            var assignments = await _assignmentService.GetAllAsync();
            var submissions = await _submissionService.GetAllAsync();

            TotalUsers = users.Count();
            TotalCourses = courses.Count();
            TotalEnrollments = enrollments.Count();
            TotalAssignments = assignments.Count();

            // Students per Course
            var studentsPerCourse = courses
                .Select(c => new
                {
                    c.Title,
                    StudentCount = enrollments.Count(e => e.CourseId == c.Id)
                })
                .ToList();
            StudentsPerCourseLabels = studentsPerCourse.Select(x => x.Title ?? "N/A").ToList();
            StudentsPerCourseData = studentsPerCourse.Select(x => x.StudentCount).ToList();

            // Assignments per Course
            var assignmentsPerCourse = courses
                .Select(c => new
                {
                    c.Title,
                    AssignmentCount = assignments.Count(a => a.CourseId == c.Id)
                })
                .ToList();
            AssignmentsPerCourseLabels = assignmentsPerCourse.Select(x => x.Title ?? "N/A").ToList();
            AssignmentsPerCourseData = assignmentsPerCourse.Select(x => x.AssignmentCount).ToList();

            // Submissions per Assignment
            var submissionsPerAssignment = assignments
                .Select(a => new
                {
                    a.Title,
                    SubmissionCount = submissions.Count(s => s.AssignmentId == a.Id)
                })
                .ToList();
            SubmissionsPerAssignmentLabels = submissionsPerAssignment.Select(x => x.Title ?? "N/A").ToList();
            SubmissionsPerAssignmentData = submissionsPerAssignment.Select(x => x.SubmissionCount).ToList();

            // Average Score per Assignment
            var avgScorePerAssignment = assignments
                .Select(a => new
                {
                    a.Title,
                    AverageScore = submissions
                        .Where(s => s.AssignmentId == a.Id && s.Score.HasValue)
                        .Average(s => (double?)s.Score) ?? 0
                })
                .ToList();
            AverageScorePerAssignmentData = avgScorePerAssignment.Select(x => x.AverageScore).ToList();

            // Enrollments Over Time (last 12 months)
            var now = DateTime.UtcNow;
            var startMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-11);

            var enrollmentGroups = enrollments
                .Where(e => e.EnrolledDate >= startMonth)
                .GroupBy(e => new { e.EnrolledDate.Year, e.EnrolledDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToList();

            EnrollmentsOverTimeLabels.Clear();
            EnrollmentsOverTimeData.Clear();
            for (int i = 0; i < 12; i++)
            {
                var date = startMonth.AddMonths(i);
                string label = date.ToString("MMM yyyy", CultureInfo.InvariantCulture);

                var group = enrollmentGroups.FirstOrDefault(g => g.Year == date.Year && g.Month == date.Month);
                int count = group?.Count ?? 0;

                EnrollmentsOverTimeLabels.Add(label);
                EnrollmentsOverTimeData.Add(count);
            }

            // Top 5 Courses by Enrollment
            var topCourses = courses
                .Select(c => new
                {
                    c.Title,
                    EnrollmentCount = enrollments.Count(e => e.CourseId == c.Id)
                })
                .OrderByDescending(c => c.EnrollmentCount)
                .Take(5)
                .ToList();
            TopCoursesLabels = topCourses.Select(c => c.Title ?? "N/A").ToList();
            TopCoursesData = topCourses.Select(c => c.EnrollmentCount).ToList();

            return Page();
        }
    }
}
