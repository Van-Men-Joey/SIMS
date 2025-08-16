using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Interfaces;
using SIMS.Models;
using SIMS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Pages.Admin.Assignments
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
        public List<Course> Courses { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = "";

        [BindProperty]
        public string? DeleteId { get; set; }

        public async Task OnGetAsync()
        {
            var allAssignments = await _assignmentService.GetAllAsync();
            var allCourses = await _courseService.GetAllAsync();

            Courses = allCourses.ToList();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                allAssignments = allAssignments.Where(a => a.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

            Assignments = allAssignments.OrderByDescending(a => a.CreatedDate).ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                await _assignmentService.DeleteAsync(id);
            }
            return RedirectToPage();
        }
    }
}
