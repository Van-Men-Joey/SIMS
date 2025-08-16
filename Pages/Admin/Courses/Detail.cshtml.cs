using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;
using System.Threading.Tasks;

namespace SIMS.Pages.Admin.Courses
{
    public class DetailModel : PageModel
    {
        private readonly ICourseService _courseService;

        public DetailModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public Course Course { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            Course = await _courseService.GetByIdAsync(id);

            if (Course == null)
                return NotFound();

            return Page();
        }
    }
}
