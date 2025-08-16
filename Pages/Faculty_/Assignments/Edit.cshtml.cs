using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;
using SIMS.Interfaces;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SIMS.Pages.Faculty_.Assignments
{
    public class EditModel : PageModel
    {
        private readonly IAssignmentService _assignmentService;
        private readonly ICourseService _courseService;
        private readonly IWebHostEnvironment _environment;

        public EditModel(IAssignmentService assignmentService, ICourseService courseService, IWebHostEnvironment environment)
        {
            _assignmentService = assignmentService;
            _courseService = courseService;
            _environment = environment;
        }

        [BindProperty]
        public AssignmentInputModel Assignment { get; set; } = new();

        public List<Course> Courses { get; set; } = new();

        public string? CurrentFileName { get; set; }

        public class AssignmentInputModel
        {
            public string Id { get; set; } = string.Empty;

            [Required(ErrorMessage = "Tiêu đề không được để trống")]
            public string Title { get; set; } = string.Empty;

            // Lưu tên file bài tập (ví dụ: "bai_tap_1.docx")
            public string? Description { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn khóa học")]
            public string CourseId { get; set; } = string.Empty;

            // File upload mới (không bắt buộc)
            public IFormFile? UploadedFile { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var assignment = await _assignmentService.GetByIdAsync(id);
            if (assignment == null)
                return NotFound();

            var facultyId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(facultyId))
                return RedirectToPage("/Auth/Login");

            Courses = (await _courseService.GetByFacultyIdAsync(facultyId)).ToList();

            // Map dữ liệu Assignment ra InputModel
            Assignment = new AssignmentInputModel
            {
                Id = assignment.Id!,
                Title = assignment.Title,
                Description = assignment.Description, // file name lưu ở đây
                CourseId = assignment.CourseId
            };

            CurrentFileName = assignment.Description;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var facultyId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(facultyId))
                return RedirectToPage("/Auth/Login");

            Courses = (await _courseService.GetByFacultyIdAsync(facultyId)).ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var assignmentToUpdate = await _assignmentService.GetByIdAsync(Assignment.Id);
            if (assignmentToUpdate == null)
            {
                return NotFound();
            }

            // Xử lý upload file nếu có
            if (Assignment.UploadedFile != null)
            {
                // Tạo thư mục lưu trữ nếu chưa tồn tại
                var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "assignments");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                // Tạo tên file mới (có thể thêm Guid để tránh trùng)
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Assignment.UploadedFile.FileName);
                var filePath = Path.Combine(uploadFolder, fileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await Assignment.UploadedFile.CopyToAsync(fileStream);

                // Cập nhật tên file vào Description (hoặc trường bạn dùng lưu tên file)
                assignmentToUpdate.Description = fileName;
            }

            assignmentToUpdate.Title = Assignment.Title;
            assignmentToUpdate.CourseId = Assignment.CourseId;

            await _assignmentService.UpdateAsync(assignmentToUpdate);

            return RedirectToPage("Index");
        }
    }
}
