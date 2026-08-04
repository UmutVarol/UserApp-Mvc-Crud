using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserApp.Entities;

namespace UserApp.Data
{
    /// <summary>
    /// DİKKAT: DbContext artık IdentityDbContext&lt;ApplicationUser&gt;'dan türüyor.
    /// Bu, EF Core'un Identity'nin ihtiyaç duyduğu AspNetUsers, AspNetRoles,
    /// AspNetUserRoles vb. tabloları modele otomatik dahil etmesini sağlar.
    /// Mevcut Kullanicilar/Departmanlar tablolarınız ve ilişkileri AYNEN
    /// korunuyor — sadece base class ve OnModelCreating içinde base çağrısı eklendi.
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Departman> Departmanlar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // KRİTİK: base.OnModelCreating MUTLAKA önce çağrılmalı, aksi halde
            // Identity tabloları migration'da hiç oluşmaz ve login sistemi
            // çalışma zamanında "geçersiz nesne adı 'AspNetUsers'" hatası verir.
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Kullanici>(entity =>
            {
                entity.Property(k => k.Ad).IsRequired().HasMaxLength(50);
                entity.Property(k => k.Soyad).IsRequired().HasMaxLength(50);
                entity.Property(k => k.Email).IsRequired();

                entity.HasIndex(k => k.Email).IsUnique();

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
        }
    }
}