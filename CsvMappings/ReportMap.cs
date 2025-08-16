using CsvHelper.Configuration;
using SIMS.Models;

namespace SIMS.CsvMappings
{
    public sealed class ReportMap : ClassMap<Report>
    {
        public ReportMap()
        {
            Map(m => m.Id);
            Map(m => m.StudentId);
            Map(m => m.CourseId);
            Map(m => m.Content);
        }
    }
}
