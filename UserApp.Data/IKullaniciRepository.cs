using UserApp.Entities;
using UserApp.Entities.Dtos;

namespace UserApp.Data
{
    public interface IKullaniciRepository
    {
        Task<(List<Kullanici> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm, string? sortBy, int page, int pageSize,
            int? departmanFiltre = null, bool sadeceAktif = false);

        Task<(int ToplamKullanici, int DepartmanSayisi, Kullanici? SonEklenen)> GetSummaryAsync();
        Task<Kullanici?> GetByIdAsync(int id);
        Task<KullaniciAccessInfo?> GetAccessInfoAsync(int kullaniciId);

        /// <summary>
        /// "Bu login hesabı hangi personele bağlansın?" dropdown'ı için tüm
        /// (silinmemiş) personel kayıtlarının Id + Ad Soyad bilgisini getirir.
        /// </summary>
        Task<List<KullaniciSelectItemDto>> GetAllForSelectAsync();

        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task AddAsync(Kullanici kullanici);
        Task UpdateAsync(Kullanici kullanici);
        Task DeleteAsync(Kullanici kullanici);
    }
}