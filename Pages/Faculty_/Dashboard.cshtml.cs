using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Services;
using System.Text.Json;

namespace SIMS.Pages.Faculty_
{
    public class DashboardModel : PageModel
    {
        // Dữ liệu cho thống kê tổng
        public int TotalCourses { get; set; }
        public int TotalAssignments { get; set; }
        public int TotalReports { get; set; }
        public int TotalSubmissions { get; set; }

        // Dữ liệu cho biểu đồ submissions
        public List<string> ChartLabels { get; set; } = new();
        public List<int> ChartData { get; set; } = new();

        private readonly ICourseService _courseService;
        private readonly IAssignmentService _assignmentService;
        private readonly IReportService _reportService;
        private readonly ISubmissionService _submissionService;

        public DashboardModel(
            ICourseService courseService,
            IAssignmentService assignmentService,
            IReportService reportService,
            ISubmissionService submissionService)
        {
            _courseService = courseService;
            _assignmentService = assignmentService;
            _reportService = reportService;
            _submissionService = submissionService;
        }

        public async Task OnGetAsync()
        {
            // Đếm số lượng
            TotalCourses = (await _courseService.GetAllAsync()).Count();
            TotalAssignments = (await _assignmentService.GetAllAsync()).Count();
            TotalReports = (await _reportService.GetAllAsync()).Count();
            var submissions = await _submissionService.GetAllAsync();
            TotalSubmissions = submissions.Count();

            // Biểu đồ: số lượng submissions theo từng assignment
            var assignments = await _assignmentService.GetAllAsync();
            foreach (var assignment in assignments)
            {
                ChartLabels.Add(assignment.Title ?? "Unknown");
                var count = submissions.Count(s => s.AssignmentId == assignment.Id);
                ChartData.Add(count);
            }
        }
    }
}
