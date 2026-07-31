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

        /// DataTables listeleme işlemleri için kullanıcıları sayfalayarak (Pagination) getirir.
        /// Soft Delete prensibine uygun olarak sadece silinmemiş (IsDeleted == false) aktif kullanıcıları listeler.
        /// Gelen searchTerm'e göre Ad, Soyad, Email ve Departman kolonlarında SQL LIKE sorgusu atar.
        public async Task<(List<Kullanici> Items, int TotalCount)> GetPagedAsync(string? searchTerm, string? sortBy, int page, int pageSize)
        {
            var query = _context.Kullanicilar
                .Where(k => !k.IsDeleted) // Silinmiş (Soft Delete) kayıtları gizle
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

        /// İstatistik kartları (Toplam Kullanıcı, Departman Sayısı vs.) için verileri hesaplar.
        /// Tüm hesaplamalar sadece IsDeleted == false (Aktif) olan kullanıcılar üzerinden yapılır.
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

        /// Kullanıcıyı ID'sine göre getirir.
        /// Silinmiş (IsDeleted = true) bir kullanıcıysa null döner.
        public async Task<Kullanici?> GetByIdAsync(int id) =>
            await _context.Kullanicilar
                .Where(k => !k.IsDeleted) // Silinmişleri getirme
                .Include(k => k.Departman)
                .FirstOrDefaultAsync(k => k.Id == id);

        /// Sadece aktif (silinmemiş) kullanıcıların verilerini tarayarak
        /// bu mail adresinin kullanımda olup olmadığını kontrol eder.
        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            var normalized = email.Trim().ToLower();
            return await _context.Kullanicilar
                .Where(k => !k.IsDeleted) // Silinmiş kişilerin maillerini hesaba katma
                .AnyAsync(k => k.Email != null
                            && k.Email.ToLower() == normalized
                            && (excludeId == null || k.Id != excludeId));
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

        /// Kullanıcıyı veritabanından fiziksel olarak silmez.
        /// Bunun yerine IsDeleted bayrağını true yaparak Soft Delete uygular.
        public async Task DeleteAsync(Kullanici kullanici)
        {
            kullanici.IsDeleted = true; 
            _context.Kullanicilar.Update(kullanici);
            await _context.SaveChangesAsync();
        }
    }
}