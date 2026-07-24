using UserApp.Entities;

namespace UserApp.Data
{
    public interface IDepartmanRepository
    {
        Task<List<Departman>> GetAllAsync();
    }
}
