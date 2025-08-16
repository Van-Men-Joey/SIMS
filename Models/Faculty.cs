using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Faculty : User
    {

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
