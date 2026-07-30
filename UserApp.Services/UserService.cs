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
                DepartmanAdi = k.Departman?.Ad ?? ""
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
                DepartmanAdi = kullanici.Departman?.Ad ?? ""
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
                DepartmanId = kullanici.DepartmanId
            };
        }

        public async Task<ServiceResult> AddAsync(KullaniciCreateDto dto)
        {
            var validation = await _createValidator.ValidateAsync(dto);
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();

            // Format kuralları (regex, boş kontrolü) geçse bile email zaten
            // kullanımdaysa ayrı bir iş kuralı olarak burada yakalıyoruz.
            if (!string.IsNullOrWhiteSpace(dto.Email) && await _repository.EmailExistsAsync(dto.Email))
            {
                errors.Add("Bu email adresi zaten kayıtlı. Aynı email ile birden fazla kullanıcı oluşturulamaz.");
            }

            if (errors.Any())
                return ServiceResult.Fail(errors);

            var kullanici = new Kullanici
            {
                Ad = dto.Ad,
                Soyad = dto.Soyad,
                Email = dto.Email,
                DepartmanId = dto.DepartmanId
            };

            await _repository.AddAsync(kullanici);
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> UpdateAsync(KullaniciEditDto dto)
        {
            var validation = await _editValidator.ValidateAsync(dto);
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();

            // excludeId: dto.Id → kullanıcı kendi mevcut email'ini değiştirmeden
            // kaydettiğinde "email zaten kullanımda" hatası almasın diye kendi kaydı hariç tutuluyor.
            if (!string.IsNullOrWhiteSpace(dto.Email) && await _repository.EmailExistsAsync(dto.Email, dto.Id))
            {
                errors.Add("Bu email adresi zaten başka bir kullanıcıda kayıtlı.");
            }

            if (errors.Any())
                return ServiceResult.Fail(errors);

            var kullanici = new Kullanici
            {
                Id = dto.Id,
                Ad = dto.Ad,
                Soyad = dto.Soyad,
                Email = dto.Email,
                DepartmanId = dto.DepartmanId
            };

            await _repository.UpdateAsync(kullanici);
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