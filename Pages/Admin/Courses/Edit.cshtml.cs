using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;

namespace SIMS.Pages.Admin.Courses
{
    public class EditModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly IUserService _facultyService;

        public EditModel(ICourseService courseService, IUserService facultyService)
        {
            _courseService = courseService;
            _facultyService = facultyService;
        }

        [BindProperty]
        public Course Course { get; set; }

        public List<Faculty> FacultyList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null) return NotFound();

            Course = course;

            // Chỉ lấy các user có role là Faculty
            FacultyList = (await _facultyService.GetAllAsync())
                          .Where(u => u.Role == UserRole.Faculty) // Hoặc RoleId == x
                          .Select(u => new Faculty
                          {
                              Id = u.Id,
                              Name = u.Name,
                              // Gán thêm thuộc tính nếu cần
                          }).ToList();

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Cũng cần lọc lại danh sách Faculty ở đây
                FacultyList = (await _facultyService.GetAllAsync())
                              .Where(u => u.Role == UserRole.Faculty)
                              .Select(u => new Faculty
                              {
                                  Id = u.Id,
                                  Name = u.Name
                                  // Thêm thuộc tính nếu cần
                              }).ToList();

                return Page();
            }

            await _courseService.UpdateAsync(Course);
            return RedirectToPage("Index");
        }
    }
}
