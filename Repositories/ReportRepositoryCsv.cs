using CsvHelper;
using CsvHelper.Configuration;
using SIMS.CsvMappings;
using SIMS.Interfaces;
using SIMS.Models;
using System.Globalization;
using System.Text;

namespace SIMS.Repositories
{
    public class ReportRepositoryCsv : IReportRepository
    {
        private readonly string _csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "csv", "reports.csv");
        private readonly CsvConfiguration _csvConfig;

        public ReportRepositoryCsv()
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
                writer.WriteLine("Id,StudentId,CourseId,Content");
            }
        }

        private List<Report> ReadAll()
        {
            using var reader = new StreamReader(_csvFilePath, Encoding.UTF8);
            using var csv = new CsvReader(reader, _csvConfig);
            csv.Context.RegisterClassMap<ReportMap>();
            return csv.GetRecords<Report>().ToList();
        }

        private void WriteAll(List<Report> reports)
        {
            using var writer = new StreamWriter(_csvFilePath, false, Encoding.UTF8);
            using var csv = new CsvWriter(writer, _csvConfig);
            csv.Context.RegisterClassMap<ReportMap>();
            csv.WriteHeader<Report>();
            csv.NextRecord();

            foreach (var report in reports)
            {
                csv.WriteRecord(report);
                csv.NextRecord();
            }
        }

        public Task<IEnumerable<Report>> GetAllAsync()
        {
            var data = ReadAll();
            return Task.FromResult(data.AsEnumerable());
        }

        public Task<Report?> GetByIdAsync(string id)
        {
            var report = ReadAll().FirstOrDefault(r => r.Id == id);
            return Task.FromResult(report);
        }

        public Task<IEnumerable<Report>> GetByStudentIdAsync(string studentId)
        {
            var list = ReadAll().Where(r => r.StudentId == studentId).ToList();
            return Task.FromResult(list.AsEnumerable());
        }

        public Task<IEnumerable<Report>> GetByCourseIdAsync(string courseId)
        {
            var list = ReadAll().Where(r => r.CourseId == courseId).ToList();
            return Task.FromResult(list.AsEnumerable());
        }

        public Task AddAsync(Report report)
        {
            var reports = ReadAll();

            if (reports.Any(r => r.Id == report.Id))
                throw new Exception("Report with same Id already exists.");

            reports.Add(report);
            WriteAll(reports);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Report report)
        {
            var reports = ReadAll();
            var idx = reports.FindIndex(r => r.Id == report.Id);
            if (idx == -1)
                throw new Exception("Report not found.");

            reports[idx] = report;
            WriteAll(reports);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var reports = ReadAll();
            var reportToRemove = reports.FirstOrDefault(r => r.Id == id);
            if (reportToRemove != null)
            {
                reports.Remove(reportToRemove);
                WriteAll(reports);
            }
            return Task.CompletedTask;
        }
    }
}
