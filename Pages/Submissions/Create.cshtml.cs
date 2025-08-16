using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIMS.Models;
using SIMS.Services;

namespace SIMS.Pages.Submissions
{
    public class CreateModel : PageModel
    {
        private readonly ISubmissionService _service;
        private readonly IWebHostEnvironment _env;

        public CreateModel(ISubmissionService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        [BindProperty]
        public Submission Submission { get; set; } = new();

        [BindProperty]
        public IFormFile? UploadFile { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (UploadFile != null)
            {
                var fileName = $"{Guid.NewGuid()}_{UploadFile.FileName}";
                var path = Path.Combine(_env.WebRootPath, "uploads", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (var stream = System.IO.File.Create(path))
                {
                    await UploadFile.CopyToAsync(stream);
                }

                Submission.FilePath = $"/uploads/{fileName}";
            }

            await _service.AddAsync(Submission);
            return RedirectToPage("Index");
        }
    }
}
