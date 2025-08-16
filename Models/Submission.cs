using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Models
{
    public class Submission
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public Student? Student { get; set; }

        [Required]
        public string AssignmentId { get; set; }
        [ForeignKey(nameof(AssignmentId))]
        public Assignment Assignment { get; set; }

        public string FilePath { get; set; }
        public float? Score { get; set; }

        public string? Feedback { get; set; } // Thêm feedback

        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow; // Ngày nộp bài
    }
}
