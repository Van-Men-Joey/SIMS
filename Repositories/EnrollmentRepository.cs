//using Microsoft.EntityFrameworkCore;
//using SIMS.Data;
//using SIMS.Interfaces;
//using SIMS.Models;

//namespace SIMS.Repositories
//{
//    public class EnrollmentRepository : IEnrollmentRepository
//    {
//        private readonly SIMSContext _context;

//        public EnrollmentRepository(SIMSContext context)
//        {
//            _context = context;
//        }

//        public async Task<IEnumerable<Enrollment>> GetAllAsync()
//        {
//            return await _context.Enrollments
//                .Include(e => e.Student)
//                .Include(e => e.Course)
//                .AsNoTracking()
//                .ToListAsync();
//        }


//        public async Task<Enrollment?> GetByIdAsync(string id)
//        {
//            return await _context.Enrollments
//                .Include(e => e.Student)
//                .Include(e => e.Course)
//                .FirstOrDefaultAsync(e => e.Id == id);
//        }

//        public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(string studentId)
//        {
//            return await _context.Enrollments
//                .Where(e => e.StudentId == studentId)
//                .Include(e => e.Course)
//                .AsNoTracking()
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<Enrollment>> GetByCourseIdAsync(string courseId)
//        {
//            return await _context.Enrollments
//                .Where(e => e.CourseId == courseId)
//                .Include(e => e.Student)
//                .AsNoTracking()
//                .ToListAsync();
//        }

//        public async Task AddAsync(Enrollment enrollment)
//        {
//            _context.Enrollments.Add(enrollment);
//            await _context.SaveChangesAsync();
//        }

//        public async Task DeleteAsync(string id)
//        {
//            var entity = await _context.Enrollments.FindAsync(id);
//            if (entity != null)
//            {
//                _context.Enrollments.Remove(entity);
//                await _context.SaveChangesAsync();
//            }
//        }
//    }
//}
