using SIMS.Interfaces;
using SIMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repo;
        private readonly IUserRepository _userRepository; // dùng để lấy Faculty

        public CourseService(ICourseRepository repo, IUserRepository userRepository)
        {
            _repo = repo;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            var courses = (await _repo.GetAllAsync()).ToList();
            var faculties = (await _userRepository.GetAllFacultiesAsync()).ToList();

            foreach (var course in courses)
            {
                course.Faculty = faculties.FirstOrDefault(f => f.Id == course.FacultyId);
            }

            return courses;
        }

        public Task<Course?> GetByIdAsync(string id) => _repo.GetByIdAsync(id);

        public Task<IEnumerable<Course>> GetByFacultyIdAsync(string facultyId) => _repo.GetByFacultyIdAsync(facultyId);

        public Task<IEnumerable<Course>> GetByStudentIdAsync(string studentId) => _repo.GetByStudentIdAsync(studentId);

        public async Task AddAsync(Course course)
        {
            if (string.IsNullOrEmpty(course.Id))
            {
                course.Id = await GenerateNewCourseIdAsync();
            }
            await _repo.AddAsync(course);
        }

        public Task UpdateAsync(Course course) => _repo.UpdateAsync(course);

        public Task DeleteAsync(string id) => _repo.DeleteAsync(id);

        private async Task<string> GenerateNewCourseIdAsync()
        {
            var allCourses = await _repo.GetAllAsync();
            string prefix = "CRS";

            var lastCourse = allCourses
                .Where(c => c.Id.StartsWith(prefix))
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();

            if (lastCourse == null)
            {
                return prefix + "001";
            }
            else
            {
                string lastNumberStr = lastCourse.Id.Substring(prefix.Length);
                int lastNumber = 0;

                if (!int.TryParse(lastNumberStr, out lastNumber))
                {
                    lastNumber = 0;
                }

                int newNumber = lastNumber + 1;
                return prefix + newNumber.ToString("D3");
            }
        }
    }
}
