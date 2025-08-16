using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Interfaces;
using SIMS.Models;
using SIMS.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace SIMS.Pages.Faculty_.Assignments
{
    public class CreateModel : PageModel
    {
        private readonly IAssignmentService _assignmentService;
        private readonly ICourseService _courseService;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(IAssignmentService assignmentService, ICourseService courseService, IWebHostEnvironment environment)
        {
            _assignmentService = assignmentService;
            _courseService = courseService;
            _environment = environment;
        }

        [BindProperty]
        public AssignmentInputModel Assignment { get; set; } = new();

        public List<Course> Courses { get; set; } = new();

        public class AssignmentInputModel
        {
            [Required(ErrorMessage = "Tiêu đề không được để trống")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn file bài tập (ppt, doc, pdf, ...)")]
            public IFormFile? DescriptionFile { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn khóa học")]
            public string CourseId { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn hạn nộp")]
            [DataType(DataType.DateTime)]
            public DateTime DueDate { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var facultyId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(facultyId))
            {
                return RedirectToPage("/Auth/Login");
            }

            Courses = (await _courseService.GetByFacultyIdAsync(facultyId)).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var facultyId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(facultyId))
            {
                return RedirectToPage("/Auth/Login");
            }

            if (!ModelState.IsValid)
            {
                Courses = (await _courseService.GetByFacultyIdAsync(facultyId)).ToList();
                return Page();
            }

            // Xử lý upload file
            string? savedFilePath = null;
            if (Assignment.DescriptionFile != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "assignments");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Assignment.DescriptionFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Assignment.DescriptionFile.CopyToAsync(fileStream);
                }

                // Lưu đường dẫn tương đối (so với wwwroot) để lưu DB
                savedFilePath = Path.Combine("uploads", "assignments", uniqueFileName).Replace("\\", "/");
            }

            var newAssignment = new Assignment
            {
                Id = Guid.NewGuid().ToString(),
                Title = Assignment.Title,
                Description = savedFilePath ?? "",
                CourseId = Assignment.CourseId,
                CreatedDate = DateTime.Now,
                DueDate = Assignment.DueDate
            };

            await _assignmentService.AddAsync(newAssignment);

            return RedirectToPage("Index");
        }
    }
}
