using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Models
{
    public enum EnrollmentStatus
    {
        Pending,
        Accepted,
        Rejected
    }

    public class Enrollment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; }

        [Required]
        public string CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }

        public DateTime EnrolledDate { get; set; } = DateTime.UtcNow;

        // Thêm trạng thái vào DB, mặc định là Pending
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;
    }
}
