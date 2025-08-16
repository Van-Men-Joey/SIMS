using SIMS.Interfaces;
using SIMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repo;
        private readonly ICourseRepository _courseRepo; // thêm để lấy course

        public EnrollmentService(IEnrollmentRepository repo, ICourseRepository courseRepo)
        {
            _repo = repo;
            _courseRepo = courseRepo;
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            var enrollments = (await _repo.GetAllAsync()).ToList();
            var courses = (await _courseRepo.GetAllAsync()).ToList();

            foreach (var enrollment in enrollments)
            {
                enrollment.Course = courses.FirstOrDefault(c => c.Id == enrollment.CourseId);
            }

            return enrollments;
        }

        public async Task<Enrollment?> GetByIdAsync(string id)
        {
            var enrollment = await _repo.GetByIdAsync(id);
            if (enrollment != null)
            {
                enrollment.Course = await _courseRepo.GetByIdAsync(enrollment.CourseId);
            }
            return enrollment;
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(string studentId)
        {
            var enrollments = (await _repo.GetByStudentIdAsync(studentId)).ToList();
            var courses = (await _courseRepo.GetAllAsync()).ToList();

            foreach (var enrollment in enrollments)
            {
                enrollment.Course = courses.FirstOrDefault(c => c.Id == enrollment.CourseId);
            }

            return enrollments;
        }

        public Task<IEnumerable<Enrollment>> GetByCourseIdAsync(string courseId) => _repo.GetByCourseIdAsync(courseId);

        public Task AddAsync(Enrollment enrollment) => _repo.AddAsync(enrollment);

        public Task DeleteAsync(string id) => _repo.DeleteAsync(id);

        public Task UpdateAsync(Enrollment enrollment) => _repo.UpdateAsync(enrollment);
    }
}
