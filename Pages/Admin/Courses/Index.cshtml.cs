using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Interfaces;
using SIMS.Models;
using SIMS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Pages.Admin.Courses
{
    public class IndexModel : PageModel
    {
        private readonly ICourseService _courseService;

        public IndexModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public IList<Course> Courses { get; set; } = new List<Course>();

        // Search + Sort + Paging
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; set; }
        private const int PageSize = 5;

        public async Task OnGetAsync()
        {
            
            try
            {
                var courses = await _courseService.GetAllAsync(); // <-- gọi qua service
                                                                  // Search (case-insensitive)
                if (!string.IsNullOrEmpty(SearchString))
                {
                    courses = courses.Where(c =>
                        (!string.IsNullOrEmpty(c.Title) && c.Title.Contains(SearchString, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(c.Description) && c.Description.Contains(SearchString, StringComparison.OrdinalIgnoreCase)) ||
                        (c.Faculty != null && !string.IsNullOrEmpty(c.Faculty.Name) && c.Faculty.Name.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
                    );
                }

                // Sort
                SortOrder = string.IsNullOrEmpty(SortOrder) ? "title_asc" : SortOrder;
                courses = SortOrder switch
                {
                    "title_desc" => courses.OrderByDescending(c => c.Title),
                    "faculty_asc" => courses.OrderBy(c => c.Faculty?.Name),
                    "faculty_desc" => courses.OrderByDescending(c => c.Faculty?.Name),
                    _ => courses.OrderBy(c => c.Title),
                };

                // Paging
                var totalItems = courses.Count();
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

                Courses = courses
                    .Skip((PageNumber - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc hiển thị message
                Console.WriteLine(ex.Message);
            }
           
        }
    }
}
