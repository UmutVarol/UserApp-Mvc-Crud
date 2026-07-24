using Microsoft.EntityFrameworkCore;
using UserApp.Entities;

namespace UserApp.Data
{
    public class DepartmanRepository : IDepartmanRepository
    {
        private readonly AppDbContext _context;

        public DepartmanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Departman>> GetAllAsync()
        {
            return await _context.Departmanlar
                .AsNoTracking()
                .OrderBy(d => d.Ad)
                .ToListAsync();
        }
    }
}
