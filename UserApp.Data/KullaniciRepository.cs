using Microsoft.EntityFrameworkCore;
using UserApp.Entities;

namespace UserApp.Data
{
    public class KullaniciRepository : IKullaniciRepository
    {
        private readonly AppDbContext _context;

        public KullaniciRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Kullanici> Items, int TotalCount)> GetPagedAsync(string? searchTerm, string? sortBy, int page, int pageSize)
        {
            var query = _context.Kullanicilar
                .AsNoTracking()
                .Include(k => k.Departman)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(k =>
                    EF.Functions.Like(k.Ad ?? "", $"%{term}%") ||
                    EF.Functions.Like(k.Soyad ?? "", $"%{term}%") ||
                    EF.Functions.Like(k.Email ?? "", $"%{term}%") ||
                    (k.Departman != null && EF.Functions.Like(k.Departman.Ad ?? "", $"%{term}%")));
            }

            query = sortBy switch
            {
                "ad_desc"     => query.OrderByDescending(k => k.Ad),
                "departman"   => query.OrderBy(k => k.Departman!.Ad).ThenBy(k => k.Ad),
                "son_eklenen" => query.OrderByDescending(k => k.Id),
                _             => query.OrderBy(k => k.Ad)
            };

            var totalCount = await query.CountAsync();

            if (page < 1) page = 1;

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(int, int, Kullanici?)> GetSummaryAsync()
        {
            var toplam = await _context.Kullanicilar.CountAsync();

            var departmanSayisi = await _context.Kullanicilar
                .Select(k => k.DepartmanId)
                .Distinct()
                .CountAsync();

            var sonEklenen = await _context.Kullanicilar
                .Include(k => k.Departman)
                .OrderByDescending(k => k.Id)
                .FirstOrDefaultAsync();

            return (toplam, departmanSayisi, sonEklenen);
        }

        public async Task<Kullanici?> GetByIdAsync(int id) =>
            await _context.Kullanicilar
                .Include(k => k.Departman)
                .FirstOrDefaultAsync(k => k.Id == id);

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

        public async Task DeleteAsync(Kullanici kullanici)
        {
            _context.Kullanicilar.Remove(kullanici);
            await _context.SaveChangesAsync();
        }
    }
}