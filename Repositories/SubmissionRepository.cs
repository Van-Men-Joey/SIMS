//using Microsoft.EntityFrameworkCore;
//using SIMS.Data;
//using SIMS.Interfaces;
//using SIMS.Models;

//namespace SIMS.Repositories
//{
//    public class SubmissionRepository : ISubmissionRepository
//    {
//        private readonly SIMSContext _context;

//        public SubmissionRepository(SIMSContext context)
//        {
//            _context = context;
//        }

//        public async Task<IEnumerable<Submission>> GetAllAsync()
//        {
//            return await _context.Submissions
//                .Include(s => s.Student)
//                .Include(s => s.Assignment)
//                .AsNoTracking()
//                .ToListAsync();
//        }

//        public async Task<Submission?> GetByIdAsync(string id)
//        {
//            return await _context.Submissions
//                .Include(s => s.Student)
//                .Include(s => s.Assignment)
//                .FirstOrDefaultAsync(s => s.Id == id);
//        }

//        public async Task<IEnumerable<Submission>> GetByAssignmentIdAsync(string assignmentId)
//        {
//            return await _context.Submissions
//                .Where(s => s.AssignmentId == assignmentId)
//                .AsNoTracking()
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<Submission>> GetByStudentIdAsync(string studentId)
//        {
//            return await _context.Submissions
//                .Where(s => s.StudentId == studentId)
//                .AsNoTracking()
//                .ToListAsync();
//        }

//        public async Task AddAsync(Submission submission)
//        {
//            _context.Submissions.Add(submission);
//            await _context.SaveChangesAsync();
//        }

//        public async Task UpdateAsync(Submission submission)
//        {
//            _context.Submissions.Update(submission);
//            await _context.SaveChangesAsync();
//        }

//        public async Task DeleteAsync(string id)
//        {
//            var entity = await _context.Submissions.FindAsync(id);
//            if (entity != null)
//            {
//                _context.Submissions.Remove(entity);
//                await _context.SaveChangesAsync();
//            }
//        }
//    }
//}
