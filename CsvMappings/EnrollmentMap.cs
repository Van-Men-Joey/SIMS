using CsvHelper.Configuration;
using SIMS.Models;

namespace SIMS.CsvMappings
{
    public sealed class EnrollmentMap : ClassMap<Enrollment>
    {
        public EnrollmentMap()
        {
            Map(e => e.Id);
            Map(e => e.StudentId);
            Map(e => e.CourseId);
            Map(e => e.EnrolledDate).TypeConverterOption.Format("o"); // ISO 8601 format
            Map(e => e.Status).TypeConverter<EnrollmentStatusConverter>();
        }
    }
}
