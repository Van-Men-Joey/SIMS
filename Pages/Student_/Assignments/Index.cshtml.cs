using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;
using SIMS.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SIMS.Pages.Student_.Assignments
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
            // Giả sử có UserId của student lưu trong session
            var studentId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(studentId))
            {
                Assignments = new List<Assignment>();
                return;
            }

            // Lấy tất cả khóa học mà student đã đăng ký (hoặc học)
            var coursesOfStudent = await _courseService.GetByStudentIdAsync(studentId);

            var assignments = new List<Assignment>();

            foreach (var course in coursesOfStudent)
            {
                var courseAssignments = await _assignmentService.GetByCourseIdAsync(course.Id);
                assignments.AddRange(courseAssignments);
            }

            // Sắp xếp theo deadline (due date) tăng dần
            Assignments = assignments.OrderBy(a => a.DueDate).ToList();
        }
    }
}
