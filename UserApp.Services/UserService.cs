using FluentValidation;
using UserApp.Data;
using UserApp.Entities;
using UserApp.Entities.Dtos;

namespace UserApp.Services
{
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
            if (!validation.IsValid)
                return ServiceResult.Fail(validation.Errors.Select(e => e.ErrorMessage));

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
            if (!validation.IsValid)
                return ServiceResult.Fail(validation.Errors.Select(e => e.ErrorMessage));

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