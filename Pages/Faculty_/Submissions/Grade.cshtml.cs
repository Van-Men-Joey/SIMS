using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SIMS.Models;
using SIMS.Services;
using System.Threading.Tasks;

namespace SIMS.Pages.Faculty_.Submissions
{
    public class GradeModel : PageModel
    {
        private readonly ISubmissionService _submissionService;
        private readonly ILogger<GradeModel> _logger;

        public GradeModel(ISubmissionService submissionService, ILogger<GradeModel> logger)
        {
            _submissionService = submissionService;
            _logger = logger;
        }

        [BindProperty]
        public Submission Submission { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            _logger.LogInformation("OnGetAsync called with id = {Id}", id);

            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Id parameter is null or empty");
                return NotFound();
            }

            var submission = await _submissionService.GetByIdAsync(id);
            if (submission == null)
            {
                _logger.LogWarning("No submission found for id = {Id}", id);
                return NotFound();
            }

            Submission = submission;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("OnPostAsync called for Submission Id = {Id}", Submission?.Id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid");
                foreach (var entry in ModelState)
                {
                    if (entry.Value.Errors.Count > 0)
                    {
                        _logger.LogWarning($"Key: {entry.Key}, Errors: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }

                Submission = await _submissionService.GetByIdAsync(Submission?.Id);
                return Page();
            }


            var submissionToUpdate = await _submissionService.GetByIdAsync(Submission.Id);
            if (submissionToUpdate == null)
            {
                _logger.LogWarning("Submission not found for update, Id = {Id}", Submission.Id);
                return NotFound();
            }

            // Cập nhật điểm và feedback
            submissionToUpdate.Score = Submission.Score;
            submissionToUpdate.Feedback = Submission.Feedback;

            await _submissionService.UpdateAsync(submissionToUpdate);
            _logger.LogInformation("Submission updated successfully: Id={Id}, Score={Score}", submissionToUpdate.Id, submissionToUpdate.Score);

            return RedirectToPage("Index", new { SelectedCourseId = submissionToUpdate.Assignment?.CourseId });
        }
    }
}
