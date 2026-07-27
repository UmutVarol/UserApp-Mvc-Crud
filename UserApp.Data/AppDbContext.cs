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
            modelBuilder.Entity<Kullanici>(entity =>
            {
                entity.Property(k => k.Ad).IsRequired().HasMaxLength(50);
                entity.Property(k => k.Soyad).IsRequired().HasMaxLength(50);
                entity.Property(k => k.Email).IsRequired();

                entity.HasOne(k => k.Departman)
                    .WithMany()
                    .HasForeignKey(k => k.DepartmanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

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

            modelBuilder.Entity<Kullanici>().HasData(
                new Kullanici { Id = 1, Ad = "Ahmet", Soyad = "Yılmaz", Email = "ahmet.yilmaz@example.com", DepartmanId = 1 },
                new Kullanici { Id = 2, Ad = "Elif", Soyad = "Kaya", Email = "elif.kaya@example.com", DepartmanId = 2 },
                new Kullanici { Id = 3, Ad = "Mehmet", Soyad = "Demir", Email = "mehmet.demir@example.com", DepartmanId = 3 },
                new Kullanici { Id = 4, Ad = "Zeynep", Soyad = "Şahin", Email = "zeynep.sahin@example.com", DepartmanId = 4 },
                new Kullanici { Id = 5, Ad = "Can", Soyad = "Öztürk", Email = "can.ozturk@example.com", DepartmanId = 5 },
                new Kullanici { Id = 6, Ad = "Ayşe", Soyad = "Arslan", Email = "ayse.arslan@example.com", DepartmanId = 6 },
                new Kullanici { Id = 7, Ad = "Emre", Soyad = "Koç", Email = "emre.koc@example.com", DepartmanId = 7 },
                new Kullanici { Id = 8, Ad = "Selin", Soyad = "Aydın", Email = "selin.aydin@example.com", DepartmanId = 8 }
            );
        }
    }
}