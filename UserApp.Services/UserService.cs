using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserApp.Data;
using UserApp.Entities;

namespace UserApp.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Kullanici>> GetAllAsync()
        {
            return await _context.Kullanicilar.AsNoTracking().ToListAsync();
        }

        public async Task<Kullanici?> GetByIdAsync(int id)
        {
            return await _context.Kullanicilar.FindAsync(id);
        }

        public async Task AddAsync(Kullanici kullanici)
        {
            _context.Kullanicilar.Add(kullanici);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Kullanici kullanici)
        {
            _context.Kullanicilar.Update(kullanici);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var kullanici = await GetByIdAsync(id);
            if (kullanici != null)
            {
                _context.Kullanicilar.Remove(kullanici);
                await _context.SaveChangesAsync();
            }
        }
    }
}