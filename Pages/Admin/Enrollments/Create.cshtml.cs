using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Interfaces;
using SIMS.Models;
using SIMS.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIMS.Pages.Admin.Enrollments
{
    public class CreateModel : PageModel
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;

        public CreateModel(IEnrollmentService enrollmentService, IUserService userService, ICourseService courseService)
        {
            _enrollmentService = enrollmentService;
            _userService = userService;
            _courseService = courseService;
        }

        [BindProperty]
        public string SelectedStudentId { get; set; }

        [BindProperty]
        public string SelectedCourseId { get; set; }

        public List<User> Students { get; set; } = new();
        public List<Course> Courses { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var allUsers = await _userService.GetAllAsync();
            Students = new List<User>();
            foreach (var u in allUsers)
            {
                if (u.Role == UserRole.Student)
                    Students.Add(u);
            }

            Courses = new List<Course>(await _courseService.GetAllAsync());
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(SelectedStudentId) || string.IsNullOrEmpty(SelectedCourseId))
            {
                ModelState.AddModelError(string.Empty, "Bạn phải chọn sinh viên và khóa học.");
                await OnGetAsync(); // reload data
                return Page();
            }

            // Kiểm tra nếu sinh viên đã đăng ký khóa học
            var enrollments = await _enrollmentService.GetByStudentIdAsync(SelectedStudentId);
            foreach (var e in enrollments)
            {
                if (e.CourseId == SelectedCourseId)
                {
                    ModelState.AddModelError(string.Empty, "Sinh viên này đã đăng ký khóa học này.");
                    await OnGetAsync();
                    return Page();
                }
            }

            var enrollment = new Enrollment
            {
                StudentId = SelectedStudentId,
                CourseId = SelectedCourseId,
                EnrolledDate = DateTime.UtcNow
            };

            await _enrollmentService.AddAsync(enrollment);

            return RedirectToPage("Index");
        }
    }
}
