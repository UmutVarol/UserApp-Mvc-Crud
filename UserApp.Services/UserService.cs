using FluentValidation;
using UserApp.Data;
using UserApp.Entities;
using UserApp.Entities.Dtos;

namespace UserApp.Services
{
    /// <summary>
    /// Kullanıcı yönetiminin iş mantığını (BLL) yürütür: doğrulama, email benzersizlik
    /// kontrolü ve DTO ↔ Entity dönüşümlerini üstlenir. Controller, bu sınıf dışında
    /// hiçbir Repository'yi doğrudan görmez.
    /// </summary>
    public class UserService
    {
        private readonly IKullaniciRepository _repository;
        private readonly IDepartmanRepository _departmanRepository;
        private readonly IValidator<KullaniciCreateDto> _createValidator;
        private readonly IValidator<KullaniciEditDto> _editValidator;

        public UserService(
            IKullaniciRepository repository,
            IDepartmanRepository departmanRepository,
            IValidator<KullaniciCreateDto> createValidator,
            IValidator<KullaniciEditDto> editValidator)
        {
            _repository = repository;
            _departmanRepository = departmanRepository;
            _createValidator = createValidator;
            _editValidator = editValidator;
        }

        public async Task<(List<KullaniciListItemDto> Items, int TotalCount)> GetPagedAsync(string? searchTerm, string? sortBy, int page, int pageSize)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(searchTerm, sortBy, page, pageSize);

            var dtoItems = items.Select(k => new KullaniciListItemDto
            {
                Id = k.Id,
                Ad = k.Ad ?? "",
                Soyad = k.Soyad ?? "",
                Email = k.Email ?? "",
                DepartmanAdi = k.Departman?.Ad ?? "",
                KayitTarihi = k.KayitTarihi,
                IsActive = k.IsActive,
                ProfileImagePath = k.ProfileImagePath 
            }).ToList();

            return (dtoItems, totalCount);
        }

        public async Task<(int ToplamKullanici, int DepartmanSayisi, string? SonEklenenAdSoyad)> GetSummaryAsync()
        {
            var (toplam, departmanSayisi, sonEklenen) = await _repository.GetSummaryAsync();
            string? sonEklenenAdSoyad = sonEklenen != null ? $"{sonEklenen.Ad} {sonEklenen.Soyad}" : null;
            return (toplam, departmanSayisi, sonEklenenAdSoyad);
        }
public async Task<KullaniciDetailDto?> GetDetailAsync(int id)
{
    var kullanici = await _repository.GetByIdAsync(id);
    if (kullanici == null) return null;

    return new KullaniciDetailDto
    {
        Id = kullanici.Id,
        Ad = kullanici.Ad ?? "",
        Soyad = kullanici.Soyad ?? "",
        Email = kullanici.Email ?? "",
        DepartmanAdi = kullanici.Departman?.Ad ?? "",
        ProfileImagePath = kullanici.ProfileImagePath,
        KayitTarihi = kullanici.KayitTarihi,
        IsActive = kullanici.IsActive
    };
}
        public async Task<KullaniciEditDto?> GetForEditAsync(int id)
        {
            var kullanici = await _repository.GetByIdAsync(id);
            if (kullanici == null) return null;

            return new KullaniciEditDto
            {
                Id = kullanici.Id,
                Ad = kullanici.Ad,
                Soyad = kullanici.Soyad,
                Email = kullanici.Email,
                DepartmanId = kullanici.DepartmanId,
                ProfileImagePath = kullanici.ProfileImagePath // Düzenleme ekranında mevcut resmi göstermek için
            };
        }

        /// Yeni bir kullanıcı ekler. Form kurallarını denetler, Email benzersizliğini kontrol eder 
        /// ve profil fotoğrafı varsa sunucuya yükleyip yolunu veritabanına kaydeder.
        public async Task<ServiceResult> AddAsync(KullaniciCreateDto dto, string webRootPath)
        {
            var validation = await _createValidator.ValidateAsync(dto);
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();

            if (!string.IsNullOrWhiteSpace(dto.Email) && await _repository.EmailExistsAsync(dto.Email))
            {
                errors.Add("Bu e-posta adresi ile daha önce kayıt olunmuş! Lütfen başka bir e-posta deneyin.");
            }

            // 1. FOTOĞRAF YÜKLEME İŞLEMİ 
            string? kaydedilenFotoYolu = null;
            if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
            {
                var uploadResult = await FileHelper.UploadProfileImageAsync(dto.ProfileImage, webRootPath);
                if (!uploadResult.Success)
                {
                    errors.Add(uploadResult.ErrorMessage!);
                    return ServiceResult.Fail(errors);
                }
                kaydedilenFotoYolu = uploadResult.FilePath;
            }

            if (errors.Any())
                return ServiceResult.Fail(errors);

            var kullanici = new Kullanici
            {
                Ad = dto.Ad,
                Soyad = dto.Soyad,
                Email = dto.Email,
                DepartmanId = dto.DepartmanId,
                KayitTarihi = DateTime.Now,
                ProfileImagePath = kaydedilenFotoYolu // Fotoğrafın yolunu DB'ye kaydediyoruz
            };

            await _repository.AddAsync(kullanici);
            return ServiceResult.Ok();
        }

        /// <summary>
        /// Mevcut kullanıcıyı günceller. Eğer yeni bir fotoğraf seçilmişse eskisinin üzerine 
        /// klasörde yenisini oluşturur ve veritabanındaki yolu günceller.
        /// </summary>
        public async Task<ServiceResult> UpdateAsync(KullaniciEditDto dto, string webRootPath)
        {
            var validation = await _editValidator.ValidateAsync(dto);
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();

            if (!string.IsNullOrWhiteSpace(dto.Email) && await _repository.EmailExistsAsync(dto.Email, dto.Id))
            {
                errors.Add("Bu Email adresi zaten başka bir kullanıcıda kayıtlı.");
            }

            if (errors.Any())
                return ServiceResult.Fail(errors);

            // KRİTİK DÜZELTME: Verilerin (Tarih, Aktiflik) sıfırlanmaması için kaydı önce DB'den çekiyoruz
            var mevcutKullanici = await _repository.GetByIdAsync(dto.Id);
            if (mevcutKullanici == null)
            {
                return ServiceResult.Fail(new List<string> { "Güncellenmek istenen kullanıcı bulunamadı." });
            }

            // 1. FOTOĞRAF GÜNCELLEME İŞLEMİ (Sadece formdan yeni bir resim yüklendiyse tetiklenir)
            if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
            {
                var uploadResult = await FileHelper.UploadProfileImageAsync(dto.ProfileImage, webRootPath);
                if (!uploadResult.Success)
                {
                    return ServiceResult.Fail(new List<string> { uploadResult.ErrorMessage! });
                }
                mevcutKullanici.ProfileImagePath = uploadResult.FilePath; // Veritabanındaki yolu yeni resimle değiştir
            }

            mevcutKullanici.Ad = dto.Ad;
            mevcutKullanici.Soyad = dto.Soyad;
            mevcutKullanici.Email = dto.Email;
            mevcutKullanici.DepartmanId = dto.DepartmanId;

            await _repository.UpdateAsync(mevcutKullanici);
            return ServiceResult.Ok();
        }

        public async Task DeleteAsync(int id)
        {
            var kullanici = await _repository.GetByIdAsync(id);
            if (kullanici != null)
            {
                await _repository.DeleteAsync(kullanici);
            }
        }

        public Task<List<Departman>> GetDepartmanlarAsync() => _departmanRepository.GetAllAsync();
    }
}