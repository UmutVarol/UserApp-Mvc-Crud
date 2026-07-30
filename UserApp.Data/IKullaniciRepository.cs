using UserApp.Entities;

namespace UserApp.Data
{
    public interface IKullaniciRepository
    {
        Task<(List<Kullanici> Items, int TotalCount)> GetPagedAsync(string? searchTerm, string? sortBy, int page, int pageSize);
        Task<(int ToplamKullanici, int DepartmanSayisi, Kullanici? SonEklenen)> GetSummaryAsync();
        Task<Kullanici?> GetByIdAsync(int id);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task AddAsync(Kullanici kullanici);
        Task UpdateAsync(Kullanici kullanici);
        Task DeleteAsync(Kullanici kullanici);
    }
}
