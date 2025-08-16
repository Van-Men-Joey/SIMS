using CsvHelper.Configuration;
using SIMS.Models;

namespace SIMS.CsvMappings
{
    public sealed class CourseMap : ClassMap<Course>
    {
        public CourseMap()
        {
            Map(m => m.Id);
            Map(m => m.Title);
            Map(m => m.Description);
            Map(m => m.FacultyId);
        }
    }
}
