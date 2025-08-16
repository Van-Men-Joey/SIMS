using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;


namespace SIMS.Pages.Admin.Courses
{
    public class CreateModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;

        public CreateModel(ICourseService courseService, IUserService userService)
        {
            _courseService = courseService;
            _userService = userService;
        }

        [BindProperty]
        public Course Course { get; set; } = new();

        public List<Faculty> FacultyList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            FacultyList = (await _userService.GetAllFacultiesAsync()).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                FacultyList = (await _userService.GetAllFacultiesAsync()).ToList();
                return Page();
            }

            await _courseService.AddAsync(Course);
            TempData["SuccessMessage"] = "Course created successfully!";
            return RedirectToPage("Index");
        }
    }
}
