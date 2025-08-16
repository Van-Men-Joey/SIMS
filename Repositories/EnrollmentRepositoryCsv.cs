using CsvHelper;
using CsvHelper.Configuration;
using SIMS.CsvMappings; // chứa EnrollmentMap và EnrollmentStatusConverter
using SIMS.Interfaces;
using SIMS.Models;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SIMS.Repositories
{
    public class EnrollmentRepositoryCsv : IEnrollmentRepository
    {
        private readonly string _csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "csv", "enrollments.csv");
        private readonly CsvConfiguration _csvConfig;

        public EnrollmentRepositoryCsv()
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
                using var writer = new StreamWriter(_csvFilePath);
                writer.WriteLine("Id,StudentId,CourseId,EnrolledDate,Status");
            }
        }

        private List<Enrollment> ReadAllEnrollments()
        {
            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, _csvConfig);

            csv.Context.RegisterClassMap<EnrollmentMap>();

            var records = csv.GetRecords<Enrollment>().ToList();
            return records;
        }

        private void WriteAllEnrollments(List<Enrollment> enrollments)
        {
            using var writer = new StreamWriter(_csvFilePath);
            using var csv = new CsvWriter(writer, _csvConfig);

            csv.Context.RegisterClassMap<EnrollmentMap>();
            csv.WriteHeader<Enrollment>();
            csv.NextRecord();

            foreach (var enrollment in enrollments)
            {
                csv.WriteRecord(enrollment);
                csv.NextRecord();
            }
        }

        public Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            var list = ReadAllEnrollments();
            return Task.FromResult(list.AsEnumerable());
        }

        public Task<Enrollment?> GetByIdAsync(string id)
        {
            var enrollment = ReadAllEnrollments().FirstOrDefault(e => e.Id == id);
            return Task.FromResult(enrollment);
        }

        public Task<IEnumerable<Enrollment>> GetByStudentIdAsync(string studentId)
        {
            var list = ReadAllEnrollments().Where(e => e.StudentId == studentId).ToList();
            return Task.FromResult(list.AsEnumerable());
        }

        public Task<IEnumerable<Enrollment>> GetByCourseIdAsync(string courseId)
        {
            var list = ReadAllEnrollments().Where(e => e.CourseId == courseId).ToList();
            return Task.FromResult(list.AsEnumerable());
        }

        public Task AddAsync(Enrollment enrollment)
        {
            var list = ReadAllEnrollments();

            if (list.Any(e => e.Id == enrollment.Id))
                throw new System.Exception("Enrollment with same Id already exists");

            list.Add(enrollment);
            WriteAllEnrollments(list);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var list = ReadAllEnrollments();
            var item = list.FirstOrDefault(e => e.Id == id);
            if (item != null)
            {
                list.Remove(item);
                WriteAllEnrollments(list);
            }
            return Task.CompletedTask;
        }
        public Task UpdateAsync(Enrollment enrollment)
        {
            var list = ReadAllEnrollments();
            var index = list.FindIndex(e => e.Id == enrollment.Id);
            if (index == -1)
                throw new Exception("Enrollment not found");

            list[index] = enrollment;
            WriteAllEnrollments(list);

            return Task.CompletedTask;
        }

    }
}
