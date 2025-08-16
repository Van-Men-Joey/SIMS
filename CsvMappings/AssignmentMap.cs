using CsvHelper.Configuration;
using SIMS.Models;

namespace SIMS.CsvMappings
{
    public sealed class AssignmentMap : ClassMap<Assignment>
    {
        public AssignmentMap()
        {
            Map(a => a.Id);
            Map(a => a.Title);
            Map(a => a.Description);
            Map(a => a.CourseId);
            // Course không cần map, vì chỉ lưu khóa ngoại CourseId trong CSV
            Map(a => a.CreatedDate).TypeConverterOption.Format("o"); // ISO 8601 format
            Map(a => a.DueDate).TypeConverterOption.Format("o");
        }
    }
}
