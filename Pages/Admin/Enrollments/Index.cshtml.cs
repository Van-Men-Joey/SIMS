using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Interfaces;
using SIMS.Models;
using SIMS.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Pages.Admin.Enrollments
{
    public class IndexModel : PageModel
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;

        public IndexModel(
            IEnrollmentService enrollmentService,
            IUserService studentService,
            ICourseService courseService)
        {
            _enrollmentService = enrollmentService;
            _userService = studentService;
            _courseService = courseService;
        }

        public List<Enrollment> Enrollments { get; set; } = new();
        public Dictionary<string, User> StudentsMap { get; set; } = new();
        public Dictionary<string, Course> CoursesMap { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; } = 10;

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }
        [BindProperty(SupportsGet = true)]
        public string FilterMonth { get; set; }

        public async Task OnGetAsync()
        {
            var enrollments = await _enrollmentService.GetAllAsync();

            // Filter by month
            if (!string.IsNullOrWhiteSpace(FilterMonth) &&
                DateTime.TryParseExact(FilterMonth + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var monthDate))
            {
                var nextMonth = monthDate.AddMonths(1);
                enrollments = enrollments
                    .Where(e => e.EnrolledDate >= monthDate && e.EnrolledDate < nextMonth);
            }

            // Search
            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                var lowerSearch = SearchString.ToLower();
                enrollments = enrollments.Where(e =>
                    (!string.IsNullOrEmpty(e.StudentId) && e.StudentId.ToLower().Contains(lowerSearch)) ||
                    (e.Student != null && !string.IsNullOrEmpty(e.Student.Name) && e.Student.Name.ToLower().Contains(lowerSearch)) ||
                    (e.Course != null && !string.IsNullOrEmpty(e.Course.Title) && e.Course.Title.ToLower().Contains(lowerSearch))
                );
            }

            // Sort
            SortOrder = string.IsNullOrEmpty(SortOrder) ? "date_desc" : SortOrder;
            enrollments = SortOrder switch
            {
                "date_asc" => enrollments.OrderBy(e => e.EnrolledDate),
                "date_desc" => enrollments.OrderByDescending(e => e.EnrolledDate),
                _ => enrollments.OrderByDescending(e => e.EnrolledDate)
            };

            // Paging
            var totalCount = enrollments.Count();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            Enrollments = enrollments
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // StudentsMap & CoursesMap
            StudentsMap = (await _userService.GetAllAsync())
                .Where(s => Enrollments.Any(e => e.StudentId == s.Id))
                .ToDictionary(s => s.Id, s => s);

            CoursesMap = (await _courseService.GetAllAsync())
                .Where(c => Enrollments.Any(e => e.CourseId == c.Id))
                .ToDictionary(c => c.Id, c => c);
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            await _enrollmentService.DeleteAsync(id);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetAutocompleteAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new JsonResult(Array.Empty<string>());

            var lowerTerm = term.ToLower();
            var enrollments = await _enrollmentService.GetAllAsync();

            var results = enrollments
                .Where(e =>
                    (!string.IsNullOrEmpty(e.StudentId) && e.StudentId.ToLower().Contains(lowerTerm)) ||
                    (e.Student != null && !string.IsNullOrEmpty(e.Student.Name) && e.Student.Name.ToLower().Contains(lowerTerm)) ||
                    (e.Course != null && !string.IsNullOrEmpty(e.Course.Title) && e.Course.Title.ToLower().Contains(lowerTerm))
                )
                .Select(e => new
                {
                    StudentId = e.StudentId,
                    StudentName = e.Student?.Name,
                    CourseTitle = e.Course?.Title
                })
                .Distinct()
                .Take(10)
                .ToList();

            var suggestions = results.Select(r => new
            {
                StudentId = r.StudentId,
                DisplayText = $"{r.StudentId} - {r.StudentName} - {r.CourseTitle}"
            });

            return new JsonResult(suggestions);
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostUpdateStatusAsync([FromBody] UpdateStatusRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.Status))
                return new JsonResult(new { success = false });

            var enrollment = (await _enrollmentService.GetAllAsync()).FirstOrDefault(e => e.Id == request.Id);
            if (enrollment == null)
                return new JsonResult(new { success = false });

            if (Enum.TryParse<EnrollmentStatus>(request.Status, out var status))
            {
                enrollment.Status = status;
                // Ở đây giả sử EnrollmentService có UpdateAsync
                await _enrollmentService.AddAsync(enrollment); // hoặc UpdateAsync nếu đã có
            }

            return new JsonResult(new { success = true });
        }

        public class UpdateStatusRequest
        {
            public string Id { get; set; }
            public string Status { get; set; }
        }
    }
}
