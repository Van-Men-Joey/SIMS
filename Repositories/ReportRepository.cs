using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Interfaces;
using SIMS.Models;

namespace SIMS.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly SIMSContext _context;

        public ReportRepository(SIMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            return await _context.Reports
                .Include(r => r.Student)
                .Include(r => r.Course)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(string id)
        {
            return await _context.Reports
                .Include(r => r.Student)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Report>> GetByStudentIdAsync(string studentId)
        {
            return await _context.Reports
                .Where(r => r.StudentId == studentId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetByCourseIdAsync(string courseId)
        {
            return await _context.Reports
                .Where(r => r.CourseId == courseId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Report report)
        {
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Report report)
        {
            _context.Reports.Update(report);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.Reports.FindAsync(id);
            if (entity != null)
            {
                _context.Reports.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
