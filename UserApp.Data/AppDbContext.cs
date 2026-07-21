using Microsoft.EntityFrameworkCore;
using UserApp.Entities;

namespace UserApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Kullanici> Kullanicilar { get; set; }
    }
}