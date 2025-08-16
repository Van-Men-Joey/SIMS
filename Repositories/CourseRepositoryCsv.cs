using CsvHelper;
using CsvHelper.Configuration;
using SIMS.CsvMappings;
using SIMS.Interfaces;
using SIMS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Repositories
{
    public class CourseRepositoryCsv : ICourseRepository
    {
        private readonly string _csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "csv", "courses.csv");
        private readonly CsvConfiguration _csvConfig;

        public CourseRepositoryCsv()
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
                writer.WriteLine("Id,Title,Description,FacultyId");
            }
        }

        private List<Course> ReadAllCourses()
        {
            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, _csvConfig);
            csv.Context.RegisterClassMap<CourseMap>();
            var records = csv.GetRecords<Course>().ToList();
            return records;
        }

        private void WriteAllCourses(List<Course> courses)
        {
            using var writer = new StreamWriter(_csvFilePath);
            using var csv = new CsvWriter(writer, _csvConfig);
            csv.Context.RegisterClassMap<CourseMap>();
            csv.WriteHeader<Course>();
            csv.NextRecord();
            foreach (var course in courses)
            {
                csv.WriteRecord(course);
                csv.NextRecord();
            }
        }

        public Task<IEnumerable<Course>> GetAllAsync()
        {
            var courses = ReadAllCourses();
            return Task.FromResult(courses.AsEnumerable());
        }

        public Task<Course?> GetByIdAsync(string id)
        {
            var course = ReadAllCourses().FirstOrDefault(c => c.Id == id);
            return Task.FromResult(course);
        }

        public Task<IEnumerable<Course>> GetByFacultyIdAsync(string facultyId)
        {
            var courses = ReadAllCourses().Where(c => c.FacultyId == facultyId).ToList();
            return Task.FromResult(courses.AsEnumerable());
        }

        // Vì không có Enrollment trong CSV ở đây nên bạn cần cài đặt logic phức tạp hơn 
        // hoặc gọi service khác để lọc course theo studentId.
        // Ví dụ: nếu bạn có EnrollmentRepositoryCsv, bạn gọi nó để lấy danh sách CourseId rồi lọc courses.
        public Task<IEnumerable<Course>> GetByStudentIdAsync(string studentId)
        {
            throw new NotImplementedException("Phương thức này cần truy xuất dữ liệu Enrollment riêng, chưa implement trong CSV này.");
        }

        public Task AddAsync(Course course)
        {
            var courses = ReadAllCourses();

            if (courses.Any(c => c.Id == course.Id))
                throw new Exception("Course with same Id already exists");

            courses.Add(course);
            WriteAllCourses(courses);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Course course)
        {
            var courses = ReadAllCourses();
            var index = courses.FindIndex(c => c.Id == course.Id);
            if (index == -1)
                throw new Exception("Course not found");

            courses[index] = course;
            WriteAllCourses(courses);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var courses = ReadAllCourses();
            var courseToRemove = courses.FirstOrDefault(c => c.Id == id);
            if (courseToRemove != null)
            {
                courses.Remove(courseToRemove);
                WriteAllCourses(courses);
            }
            return Task.CompletedTask;
        }
    }
}
