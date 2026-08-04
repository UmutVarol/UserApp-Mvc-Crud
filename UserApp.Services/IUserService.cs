using UserApp.Entities;
using UserApp.Entities.Dtos;

namespace UserApp.Services
{
    public interface IUserService
    {
        Task<(List<KullaniciListItemDto> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm, string? sortBy, int page, int pageSize,
            int? departmanFiltre = null, bool sadeceAktif = false);

        Task<(int ToplamKullanici, int DepartmanSayisi, string? SonEklenenAdSoyad)> GetSummaryAsync();

        Task<KullaniciDetailDto?> GetDetailAsync(int id);

        Task<KullaniciEditDto?> GetForEditAsync(int id);

        Task<KullaniciAccessInfo?> GetAccessInfoAsync(int kullaniciId);

        Task<ServiceResult> AddAsync(KullaniciCreateDto dto, string webRootPath);

        Task<ServiceResult> UpdateAsync(KullaniciEditDto dto, string webRootPath);

        Task DeleteAsync(int id);

        Task<List<Departman>> GetDepartmanlarAsync();

        /// Self-servis profil düzenleme için kullanıcının kendi bilgilerini getirir.
        Task<KullaniciSelfEditDto?> GetSelfEditAsync(int kullaniciId);

        /// Self-servis profil güncellemesi — Email ve ProfileImage'ı değiştirir.
        Task<ServiceResult> UpdateSelfAsync(int kullaniciId, KullaniciSelfEditDto dto, string webRootPath);

        Task<List<KullaniciSelectItemDto>> GetKullanicilarForSelectAsync();
        /// Departman Yöneticisinin SADECE aktif/pasif durumunu değiştirebildiği,
        /// başka hiçbir alana (Ad/Soyad/Email/Departman/Foto) dokunmayan minimal güncelleme.
        Task<ServiceResult> UpdateStatusAsync(int id, bool isActive);
    }
}