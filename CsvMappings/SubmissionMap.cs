using CsvHelper.Configuration;
using SIMS.Models;

namespace SIMS.CsvMappings
{
    public sealed class SubmissionMap : ClassMap<Submission>
    {
        public SubmissionMap()
        {
            Map(m => m.Id);
            Map(m => m.StudentId);
            Map(m => m.AssignmentId);
            Map(m => m.FilePath);
            Map(m => m.Score);
            Map(m => m.Feedback);
            Map(m => m.SubmittedDate).TypeConverterOption.Format("o"); // ISO 8601 datetime format
        }
    }

}
