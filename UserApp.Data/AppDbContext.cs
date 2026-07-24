using Microsoft.EntityFrameworkCore;
using UserApp.Entities;

namespace UserApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Departman> Departmanlar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kullanici>()
                .HasOne(k => k.Departman)
                .WithMany()
                .HasForeignKey(k => k.DepartmanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Departman>().HasData(
                new Departman { Id = 1, Ad = "IT" },
                new Departman { Id = 2, Ad = "Muhasebe" },
                new Departman { Id = 3, Ad = "İnsan Kaynakları" },
                new Departman { Id = 4, Ad = "Satış" },
                new Departman { Id = 5, Ad = "Pazarlama" },
                new Departman { Id = 6, Ad = "Ar-Ge" },
                new Departman { Id = 7, Ad = "Hukuk" },
                new Departman { Id = 8, Ad = "Lojistik" },
                new Departman { Id = 9, Ad = "Müşteri Hizmetleri" },
                new Departman { Id = 10, Ad = "Yönetim" }
            );
        }
    }
}