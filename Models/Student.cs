using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Student : User
    {
     
        [Required, StringLength(100)]
        public List<Enrollment> Enrollments { get; set; } = new();
        public List<Submission> Submissions { get; set; } = new();
    }
}
