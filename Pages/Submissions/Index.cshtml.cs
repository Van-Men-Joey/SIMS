using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;

namespace SIMS.Pages.Submissions
{
    public class IndexModel : PageModel
    {
        private readonly ISubmissionService _submissionService;

        public IndexModel(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        public IEnumerable<Submission> Submissions { get; set; } = new List<Submission>();

        public async Task OnGetAsync()
        {
            Submissions = await _submissionService.GetAllAsync();
        }
    }
}
