using CsvHelper;
using CsvHelper.Configuration;
using SIMS.CsvMappings;
using SIMS.Interfaces;
using SIMS.Models;
using System.Globalization;
using System.Text;

namespace SIMS.Repositories
{
    public class SubmissionRepositoryCsv : ISubmissionRepository
    {
        private readonly string _csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "csv", "submissions.csv");
        private readonly CsvConfiguration _csvConfig;

        public SubmissionRepositoryCsv()
        {
            _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                PrepareHeaderForMatch = args => args.Header.Trim(),
            };

            var dir = Path.GetDirectoryName(_csvFilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(_csvFilePath))
            {
                using var writer = new StreamWriter(_csvFilePath, false, Encoding.UTF8);
                writer.WriteLine("Id,StudentId,AssignmentId,FilePath,Score,Feedback,SubmittedDate");
            }
        }


        private List<Submission> ReadAll()
        {
            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, _csvConfig);
            csv.Context.RegisterClassMap<SubmissionMap>();
            return csv.GetRecords<Submission>().ToList();
        }

        private void WriteAll(List<Submission> submissions)
        {
            using var writer = new StreamWriter(_csvFilePath, false, Encoding.UTF8);
            using var csv = new CsvWriter(writer, _csvConfig);
            csv.Context.RegisterClassMap<SubmissionMap>();
            csv.WriteHeader<Submission>();
            csv.NextRecord();
            foreach (var sub in submissions)
            {
                csv.WriteRecord(sub);
                csv.NextRecord();
            }
        }

        public Task<IEnumerable<Submission>> GetAllAsync()
        {
            var data = ReadAll();
            return Task.FromResult(data.AsEnumerable());
        }

        public Task<Submission?> GetByIdAsync(string id)
        {
            var sub = ReadAll().FirstOrDefault(s => s.Id == id);
            return Task.FromResult(sub);
        }

        public Task<IEnumerable<Submission>> GetByAssignmentIdAsync(string assignmentId)
        {
            var list = ReadAll().Where(s => s.AssignmentId == assignmentId).ToList();
            return Task.FromResult(list.AsEnumerable());
        }

        public Task<IEnumerable<Submission>> GetByStudentIdAsync(string studentId)
        {
            var list = ReadAll().Where(s => s.StudentId == studentId).ToList();
            return Task.FromResult(list.AsEnumerable());
        }

        public Task AddAsync(Submission submission)
        {
            var submissions = ReadAll();

            if (submissions.Any(s => s.Id == submission.Id))
                throw new Exception("Submission with same Id already exists.");

            submissions.Add(submission);
            WriteAll(submissions);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Submission submission)
        {
            var submissions = ReadAll();
            var idx = submissions.FindIndex(s => s.Id == submission.Id);
            if (idx == -1)
                throw new Exception("Submission not found.");

            submissions[idx] = submission;
            WriteAll(submissions);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var submissions = ReadAll();
            var subToRemove = submissions.FirstOrDefault(s => s.Id == id);
            if (subToRemove != null)
            {
                submissions.Remove(subToRemove);
                WriteAll(submissions);
            }
            return Task.CompletedTask;
        }
        public Task<IEnumerable<Submission>> GetByCourseIdAsync(string courseId)
        {
            // Đọc toàn bộ submissions
            var submissions = ReadAll();

            // Lọc theo courseId (dựa vào AssignmentId -> CourseId)
            // Vì CSV Submission chỉ có AssignmentId, nên cần join với assignments.csv
            var assignmentFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "csv", "assignments.csv");
            var assignments = new List<Assignment>();

            if (File.Exists(assignmentFilePath))
            {
                using var reader = new StreamReader(assignmentFilePath);
                using var csv = new CsvReader(reader, _csvConfig);
                csv.Context.RegisterClassMap<AssignmentMap>(); // map CSV của Assignment
                assignments = csv.GetRecords<Assignment>().ToList();
            }

            var filtered = from sub in submissions
                           join a in assignments on sub.AssignmentId equals a.Id
                           where a.CourseId == courseId
                           select sub;

            return Task.FromResult(filtered.AsEnumerable());
        }

    }
}
