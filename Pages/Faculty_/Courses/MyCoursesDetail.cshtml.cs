using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Pages.Faculty_.Courses
{
    public class MyCoursesDetailModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IAssignmentService _assignmentService;
        private readonly ISubmissionService _submissionService;
        private readonly IUserService _userService;

        public MyCoursesDetailModel(
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            IAssignmentService assignmentService,
            ISubmissionService submissionService,
            IUserService userService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _assignmentService = assignmentService;
            _submissionService = submissionService;
            _userService = userService;
        }

        public Course Course { get; set; }
        public int StudentCount { get; set; }
        public int AssignmentCount { get; set; }
        public int SubmissionCount { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public IEnumerable<User> AllStudents { get; set; } = new List<User>();

        [BindProperty(SupportsGet = true)]
        public string CourseId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty]
        public string StudentId { get; set; }

        [BindProperty]
        public string EnrollmentId { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            // Lấy course
            Course = await _courseService.GetByIdAsync(id);
            if (Course == null)
                return NotFound();

            // Lấy tất cả users và lọc ra student
            var allUsers = await _userService.GetAllAsync();
            var students = allUsers
                .Where(u => u.Role == UserRole.Student)
                .Select(u => new Student
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email
                    // nếu Student có thuộc tính khác thì map thêm
                })
                .ToList();

            // Lấy enrollments kèm thông tin student
            var enrollments = await _enrollmentService.GetByCourseIdAsync(id);
            Enrollments = enrollments
                .Select(e =>
                {
                    e.Student = students.FirstOrDefault(s => s.Id == e.StudentId);
                    return e;
                })
                .Where(e =>
                    e.Student != null && (
                        string.IsNullOrWhiteSpace(SearchTerm) ||
                        e.Student.Id.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        e.Student.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        e.Student.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                    ))
                .ToList();

            StudentCount = Enrollments.Count(e => e.Status == EnrollmentStatus.Accepted);

            // Đếm assignments
            var assignments = await _assignmentService.GetByCourseIdAsync(id);
            AssignmentCount = assignments.Count();

            // Đếm submissions
            var allSubmissions = await _submissionService.GetAllAsync();
            SubmissionCount = allSubmissions
                .Count(s => assignments.Any(a => a.Id == s.AssignmentId));

            // Danh sách tất cả sinh viên
            AllStudents = allUsers
                .Where(u => u.Role == UserRole.Student)
                .OrderBy(u => u.Name)
                .ToList();

            return Page();
        }


        public async Task<IActionResult> OnPostAddEnrollmentAsync()
        {
            if (string.IsNullOrEmpty(CourseId) || string.IsNullOrEmpty(StudentId))
            {
                ErrorMessage = "Vui lòng chọn hoặc nhập sinh viên.";
                await ReloadPageDataAsync(CourseId);
                return Page();
            }

            var existingEnrollments = await _enrollmentService.GetByCourseIdAsync(CourseId);
            if (existingEnrollments.Any(e => e.StudentId == StudentId))
            {
                ErrorMessage = "Sinh viên này đã đăng ký hoặc đang chờ duyệt.";
                await ReloadPageDataAsync(CourseId);
                return Page();
            }

            var newEnrollment = new Enrollment
            {
                Id = Guid.NewGuid().ToString(),
                CourseId = CourseId,
                StudentId = StudentId,
                EnrolledDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Pending
            };

            await _enrollmentService.AddAsync(newEnrollment);

            return RedirectToPage(new { id = CourseId });
        }

        public async Task<IActionResult> OnPostDeleteEnrollmentAsync()
        {
            if (string.IsNullOrEmpty(EnrollmentId))
                return BadRequest();

            var enrollment = await _enrollmentService.GetByIdAsync(EnrollmentId);
            if (enrollment == null)
                return NotFound();

            var courseId = enrollment.CourseId;

            await _enrollmentService.DeleteAsync(EnrollmentId);

            return RedirectToPage(new { id = courseId });
        }

        public async Task<IActionResult> OnPostApproveEnrollmentAsync()
        {
            if (string.IsNullOrEmpty(EnrollmentId))
                return BadRequest();

            var enrollment = await _enrollmentService.GetByIdAsync(EnrollmentId);
            if (enrollment == null)
                return NotFound();

            enrollment.Status = EnrollmentStatus.Accepted;
            await _enrollmentService.UpdateAsync(enrollment);

            return RedirectToPage(new { id = enrollment.CourseId });
        }

        private async Task ReloadPageDataAsync(string courseId)
        {
            // Lấy tất cả user
            var allUsers = await _userService.GetAllAsync();

            // Lọc ra danh sách Student để gán vào enrollment
            var studentList = allUsers
                .Where(u => u.Role == UserRole.Student)
                .Select(u => new Student
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email
                    // Nếu Student có thêm thuộc tính khác, map thêm ở đây
                })
                .ToList();

            // Lấy thông tin khóa học
            Course = await _courseService.GetByIdAsync(courseId);

            // Lấy enrollment và gán Student vào
            var enrollments = await _enrollmentService.GetByCourseIdAsync(courseId);
            Enrollments = enrollments
                .Select(e =>
                {
                    e.Student = studentList.FirstOrDefault(s => s.Id == e.StudentId);
                    return e;
                })
                .ToList();

            // Đếm số lượng student đã được accept
            StudentCount = Enrollments.Count(e => e.Status == EnrollmentStatus.Accepted);

            // Lấy assignment và đếm
            var assignments = await _assignmentService.GetByCourseIdAsync(courseId);
            AssignmentCount = assignments.Count();

            // Đếm số submission thuộc các assignment
            var allSubmissions = await _submissionService.GetAllAsync();
            SubmissionCount = allSubmissions
                .Count(s => assignments.Any(a => a.Id == s.AssignmentId));

            // Danh sách tất cả sinh viên
            AllStudents = allUsers
                .Where(u => u.Role == UserRole.Student)
                .OrderBy(u => u.Name)
                .ToList();
        }

    }
}
