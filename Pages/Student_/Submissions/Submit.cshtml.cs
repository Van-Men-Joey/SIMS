using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SIMS.Pages.Student_.Submissions
{
    public class SubmitModel : PageModel
    {
        private readonly IAssignmentService _assignmentService;
        private readonly ISubmissionService _submissionService;

        public SubmitModel(
            IAssignmentService assignmentService,
            ISubmissionService submissionService)
        {
            _assignmentService = assignmentService;
            _submissionService = submissionService;
        }

        [BindProperty]
        public IFormFile UploadFile { get; set; }

        public Assignment Assignment { get; set; }
        public Submission ExistingSubmission { get; set; }

        public async Task<IActionResult> OnGetAsync(string assignmentId)
        {
            var studentId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(studentId))
                return RedirectToPage("/Auth/Login");

            Assignment = await _assignmentService.GetByIdAsync(assignmentId);
            if (Assignment == null) return NotFound();

            ExistingSubmission = (await _submissionService.GetByAssignmentIdAsync(assignmentId))
                                    .FirstOrDefault(s => s.StudentId == studentId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string assignmentId)
        {
            var studentId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(studentId))
                return RedirectToPage("/Auth/Login");

            Assignment = await _assignmentService.GetByIdAsync(assignmentId);
            if (Assignment == null) return NotFound();

            if (DateTime.UtcNow > Assignment.DueDate)
            {
                ModelState.AddModelError(string.Empty, "The submission deadline has passed.");
                return Page();
            }

            if (UploadFile == null || UploadFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please choose a file to upload.");
                return Page();
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "submissions");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(UploadFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await UploadFile.CopyToAsync(stream);
            }

            ExistingSubmission = (await _submissionService.GetByAssignmentIdAsync(assignmentId))
                                    .FirstOrDefault(s => s.StudentId == studentId);

            if (ExistingSubmission != null)
            {
                ExistingSubmission.FilePath = $"uploads/submissions/{fileName}";
                ExistingSubmission.SubmittedDate = DateTime.UtcNow;
                await _submissionService.UpdateAsync(ExistingSubmission);
            }
            else
            {
                var submission = new Submission
                {
                    Id = Guid.NewGuid().ToString(),
                    AssignmentId = assignmentId,
                    StudentId = studentId,
                    FilePath = $"uploads/submissions/{fileName}",
                    SubmittedDate = DateTime.UtcNow
                };
                await _submissionService.AddAsync(submission);
            }

            TempData["SuccessMessage"] = "Submission uploaded successfully!";
            return RedirectToPage("/Student_/Courses/MyCoursesStudentDetail", new { id = Assignment.CourseId });
        }

        // Handler hủy submission
        public async Task<IActionResult> OnPostCancelAsync(string assignmentId)
        {
            var studentId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(studentId))
                return RedirectToPage("/Auth/Login");

            var existingSubmission = (await _submissionService.GetByAssignmentIdAsync(assignmentId))
                                    .FirstOrDefault(s => s.StudentId == studentId);

            if (existingSubmission != null)
            {
                // Xóa file trên server
                var fileFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingSubmission.FilePath.Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(fileFullPath))
                {
                    System.IO.File.Delete(fileFullPath);
                }

                await _submissionService.DeleteAsync(existingSubmission.Id);
                TempData["SuccessMessage"] = "Submission cancelled successfully. You can submit again.";
            }
            else
            {
                TempData["ErrorMessage"] = "No submission found to cancel.";
            }

            Assignment = await _assignmentService.GetByIdAsync(assignmentId);
            return RedirectToPage("/Student_/Courses/MyCoursesStudentDetail", new { id = Assignment?.CourseId });
        }
    }
}
