using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Interfaces;
using SIMS.Models;

namespace SIMS.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly SIMSContext _context;

        public CourseRepository(SIMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(c => c.Faculty)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(string id)
        {
            return await _context.Courses
                .Include(c => c.Faculty)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Course>> GetByFacultyIdAsync(string facultyId)
        {
            return await _context.Courses
                .Where(c => c.FacultyId == facultyId)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetByStudentIdAsync(string studentId)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Accepted)
                .Select(e => e.Course)
                .ToListAsync();
        }


        public async Task AddAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.Courses.FindAsync(id);
            if (entity != null)
            {
                _context.Courses.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
