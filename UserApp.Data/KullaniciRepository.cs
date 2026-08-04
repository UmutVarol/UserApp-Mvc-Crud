using Microsoft.EntityFrameworkCore;
using UserApp.Entities;
using UserApp.Entities.Dtos;

namespace UserApp.Data
{
    public class KullaniciRepository : IKullaniciRepository
    {
        private readonly AppDbContext _context;

        public KullaniciRepository(AppDbContext context)
        {
            _context = context;
        }

        /// DataTables listeleme işlemleri için kullanıcıları sayfalayarak (Pagination) getirir.
        /// ROL BAZLI FİLTRELEME: departmanFiltre doluysa sadece o departman,
        /// sadeceAktif true ise sadece IsActive=true kayıtlar döner — ikisi de
        /// SQL WHERE ile uygulanır (performans + doğru TotalCount için kritik).
        public async Task<(List<Kullanici> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm, string? sortBy, int page, int pageSize,
            int? departmanFiltre = null, bool sadeceAktif = false)
        {
            var query = _context.Kullanicilar
                .Where(k => !k.IsDeleted)
                .AsNoTracking()
                .Include(k => k.Departman)
                .AsQueryable();

            if (departmanFiltre.HasValue)
            {
                query = query.Where(k => k.DepartmanId == departmanFiltre.Value);
            }

            if (sadeceAktif)
            {
                query = query.Where(k => k.IsActive);
            }

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

        public async Task<(int ToplamKullanici, int DepartmanSayisi, Kullanici? SonEklenen)> GetSummaryAsync()
        {
            var toplam = await _context.Kullanicilar.Where(k => !k.IsDeleted).CountAsync();

            var departmanSayisi = await _context.Kullanicilar
                .Where(k => !k.IsDeleted).Select(k => k.DepartmanId).Distinct().CountAsync();

            var sonEklenen = await _context.Kullanicilar
                .Where(k => !k.IsDeleted).Include(k => k.Departman)
                .OrderByDescending(k => k.Id).FirstOrDefaultAsync();

            return (toplam, departmanSayisi, sonEklenen);
        }

        public async Task<Kullanici?> GetByIdAsync(int id) =>
            await _context.Kullanicilar
                .Where(k => !k.IsDeleted)
                .Include(k => k.Departman)
                .FirstOrDefaultAsync(k => k.Id == id);

        /// Yetkilendirme SADECE bir kaydın hangi departmana ait olduğunu ve
        /// aktif olup olmadığını bilmek ister; bu yüzden Departman
        /// navigation'ını Include etmeden hafif bir sorgu kullanıyoruz.
        public async Task<KullaniciAccessInfo?> GetAccessInfoAsync(int kullaniciId) =>
            await _context.Kullanicilar
                .Where(k => !k.IsDeleted && k.Id == kullaniciId)
                .Select(k => new KullaniciAccessInfo { DepartmanId = k.DepartmanId, IsActive = k.IsActive })
                .FirstOrDefaultAsync();

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            var normalized = email.Trim().ToLower();
            return await _context.Kullanicilar
                .Where(k => !k.IsDeleted)
                .AnyAsync(k => k.Email != null
                            && k.Email.ToLower() == normalized
                            && (excludeId == null || k.Id != excludeId));
        }
          public async Task<List<KullaniciSelectItemDto>> GetAllForSelectAsync() =>
            await _context.Kullanicilar
                .Where(k => !k.IsDeleted)
                .OrderBy(k => k.Ad)
                .Select(k => new KullaniciSelectItemDto
                {
                    Id = k.Id,
                    AdSoyad = (k.Ad ?? "") + " " + (k.Soyad ?? "")
                })
                .ToListAsync();
                
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
            kullanici.IsDeleted = true;
            _context.Kullanicilar.Update(kullanici);
            await _context.SaveChangesAsync();
        }
    }
}