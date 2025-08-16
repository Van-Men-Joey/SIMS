using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;

namespace SIMS.Pages.Admin.Courses
{
    public class DeleteModel : PageModel
    {
        private readonly ICourseService _service;

        public DeleteModel(ICourseService service)
        {
            _service = service;
        }

        [BindProperty]
        public Course Course { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var course = await _service.GetByIdAsync(id);
            if (course == null) return NotFound();

            Course = course;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            await _service.DeleteAsync(id);
            return RedirectToPage("Index");
        }
    }
}
