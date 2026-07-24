using UserApp.Data;
using UserApp.Entities;

namespace UserApp.Services
{
    public class UserService
    {
        private readonly IKullaniciRepository _repository;
        private readonly IDepartmanRepository _departmanRepository;

        public UserService(IKullaniciRepository repository, IDepartmanRepository departmanRepository)
        {
            _repository = repository;
            _departmanRepository = departmanRepository;
        }

        public Task<(List<Kullanici> Items, int TotalCount)> GetPagedAsync(string? searchTerm, string? sortBy, int page, int pageSize)
            => _repository.GetPagedAsync(searchTerm, sortBy, page, pageSize);

        public Task<(int ToplamKullanici, int DepartmanSayisi, Kullanici? SonEklenen)> GetSummaryAsync()
            => _repository.GetSummaryAsync();

        public Task<Kullanici?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public Task AddAsync(Kullanici kullanici) => _repository.AddAsync(kullanici);

        public Task UpdateAsync(Kullanici kullanici) => _repository.UpdateAsync(kullanici);

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