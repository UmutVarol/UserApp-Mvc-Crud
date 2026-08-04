using FluentValidation;
using UserApp.Data;
using UserApp.Entities;
using UserApp.Entities.Dtos;

namespace UserApp.Services
{
    /// Kullanıcı yönetiminin iş mantığını (BLL) yürütür: doğrulama, email benzersizlik
    /// kontrolü ve DTO ↔ Entity dönüşümlerini üstlenir. Controller, bu sınıf dışında
    /// hiçbir Repository'yi doğrudan görmez.
    public class UserService : IUserService
    {
        private readonly IKullaniciRepository _repository;
        private readonly IDepartmanRepository _departmanRepository;
        private readonly IValidator<KullaniciCreateDto> _createValidator;
        private readonly IValidator<KullaniciEditDto> _editValidator;
        private readonly IValidator<KullaniciSelfEditDto> _selfEditValidator;

        public UserService(
            IKullaniciRepository repository,
            IDepartmanRepository departmanRepository,
            IValidator<KullaniciCreateDto> createValidator,
            IValidator<KullaniciEditDto> editValidator,
            IValidator<KullaniciSelfEditDto> selfEditValidator)
        {
            _repository = repository;
            _departmanRepository = departmanRepository;
            _createValidator = createValidator;
            _editValidator = editValidator;
            _selfEditValidator = selfEditValidator;
        }

        public async Task<(List<KullaniciListItemDto> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm, string? sortBy, int page, int pageSize,
            int? departmanFiltre = null, bool sadeceAktif = false)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(
                searchTerm, sortBy, page, pageSize, departmanFiltre, sadeceAktif);

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
                IsActive = kullanici.IsActive,
                ProfileImagePath = kullanici.ProfileImagePath
            };
        }

        public Task<KullaniciAccessInfo?> GetAccessInfoAsync(int kullaniciId) => _repository.GetAccessInfoAsync(kullaniciId);

        public async Task<ServiceResult> AddAsync(KullaniciCreateDto dto, string webRootPath)
        {
            dto.Ad = dto.Ad?.Trim();
            dto.Soyad = dto.Soyad?.Trim();

            var validation = await _createValidator.ValidateAsync(dto);
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();

            if (!string.IsNullOrWhiteSpace(dto.Email) && await _repository.EmailExistsAsync(dto.Email))
            {
                errors.Add("Bu e-posta adresi ile daha önce kayıt olunmuş! Lütfen başka bir e-posta deneyin.");
            }

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
                IsActive = true,
                ProfileImagePath = kaydedilenFotoYolu
            };

            await _repository.AddAsync(kullanici);
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> UpdateAsync(KullaniciEditDto dto, string webRootPath)
        {
            dto.Ad = dto.Ad?.Trim();
            dto.Soyad = dto.Soyad?.Trim();

            var validation = await _editValidator.ValidateAsync(dto);
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();

            if (!string.IsNullOrWhiteSpace(dto.Email) && await _repository.EmailExistsAsync(dto.Email, dto.Id))
            {
                errors.Add("Bu Email adresi zaten başka bir kullanıcıda kayıtlı.");
            }

            if (errors.Any())
                return ServiceResult.Fail(errors);

            var mevcutKullanici = await _repository.GetByIdAsync(dto.Id);
            if (mevcutKullanici == null)
            {
                return ServiceResult.Fail(new List<string> { "Güncellenmek istenen kullanıcı bulunamadı." });
            }

            if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
            {
                var uploadResult = await FileHelper.UploadProfileImageAsync(dto.ProfileImage, webRootPath);
                if (!uploadResult.Success)
                {
                    return ServiceResult.Fail(new List<string> { uploadResult.ErrorMessage! });
                }
                mevcutKullanici.ProfileImagePath = uploadResult.FilePath;
            }

            mevcutKullanici.Ad = dto.Ad;
            mevcutKullanici.Soyad = dto.Soyad;
            mevcutKullanici.Email = dto.Email;
            mevcutKullanici.DepartmanId = dto.DepartmanId;
            mevcutKullanici.IsActive = dto.IsActive;

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

        public async Task<KullaniciSelfEditDto?> GetSelfEditAsync(int kullaniciId)
        {
            var kullanici = await _repository.GetByIdAsync(kullaniciId);
            if (kullanici == null) return null;

            return new KullaniciSelfEditDto
            {
                Ad = kullanici.Ad,
                Soyad = kullanici.Soyad,
                DepartmanAdi = kullanici.Departman?.Ad ?? "",
                Email = kullanici.Email,
                ProfileImagePath = kullanici.ProfileImagePath
            };
        }

        /// SADECE Email ve (varsa) yeni fotoğrafı günceller. Ad/Soyad/DepartmanId/
        /// IsActive bu metodun ELİNE HİÇ GEÇMEZ — DTO'da yoklar. Bu, "User rolü
        /// sadece kendi email/fotoğrafını değiştirebilir" kuralının kod seviyesinde
        /// KIRILAMAZ şekilde garanti edilmesidir (görünürlük/UI kısıtlaması değil).
        public async Task<ServiceResult> UpdateSelfAsync(int kullaniciId, KullaniciSelfEditDto dto, string webRootPath)
        {
            var validation = await _selfEditValidator.ValidateAsync(dto);
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();

            if (!string.IsNullOrWhiteSpace(dto.Email) && await _repository.EmailExistsAsync(dto.Email, kullaniciId))
            {
                errors.Add("Bu Email adresi zaten başka bir kullanıcıda kayıtlı.");
            }

            if (errors.Any())
                return ServiceResult.Fail(errors);

            var mevcutKullanici = await _repository.GetByIdAsync(kullaniciId);
            if (mevcutKullanici == null)
            {
                return ServiceResult.Fail(new List<string> { "Profiliniz bulunamadı." });
            }

            if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
            {
                var uploadResult = await FileHelper.UploadProfileImageAsync(dto.ProfileImage, webRootPath);
                if (!uploadResult.Success)
                {
                    return ServiceResult.Fail(new List<string> { uploadResult.ErrorMessage! });
                }
                mevcutKullanici.ProfileImagePath = uploadResult.FilePath;
            }

            mevcutKullanici.Email = dto.Email;

            await _repository.UpdateAsync(mevcutKullanici);
            return ServiceResult.Ok();
        }
        public Task<List<KullaniciSelectItemDto>> GetKullanicilarForSelectAsync() => _repository.GetAllForSelectAsync();

        public async Task<ServiceResult> UpdateStatusAsync(int id, bool isActive)
        {
            var kullanici = await _repository.GetByIdAsync(id);
            if (kullanici == null)
            {
                return ServiceResult.Fail(new List<string> { "Kullanıcı bulunamadı." });
            }

            kullanici.IsActive = isActive;
            await _repository.UpdateAsync(kullanici);
            return ServiceResult.Ok();
        }
    }
}