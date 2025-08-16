using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Interfaces;
using SIMS.Models;

namespace SIMS.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SIMSContext _context;

        public UserRepository(SIMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.Users.FindAsync(id);
            if (entity != null)
            {
                _context.Users.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Faculty>> GetAllFacultiesAsync()
        {
            return await _context.Users
                .OfType<Faculty>() // EF Core filter theo Discriminator
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
