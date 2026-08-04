using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserApp.Entities;

namespace UserApp.Data.Seed
{
    /// <summary>
    /// Uygulama her ayağa kalktığında rolleri ve tek bir başlangıç admin
    /// hesabını veritabanına ekler (varsa tekrar oluşturmaz, idempotenttir).
    /// </summary>
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. ROLLERİ OLUŞTUR (yoksa)
            // ÜÇ ROL: Admin (tam yetki), DepartmanYoneticisi (kendi departmanı
            // ile sınırlı CRUD), User (sadece kendi profili + salt-okunur dizin).
            string[] roles = { "Admin", "DepartmanYoneticisi", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. İLK ADMIN HESABINI OLUŞTUR (yoksa)
            var adminEmail = configuration["SeedAdmin:Email"] ?? "admin@userapp.local";
            var adminPassword = configuration["SeedAdmin:Password"] ?? "ChangeMe123!";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    AdSoyad = "Sistem Yöneticisi",
                    EmailConfirmed = true
                    // KullaniciId ve YonetilenDepartmanId bilinçli olarak null
                    // bırakıldı: Admin'in hem tüm departmanlara erişimi olduğu
                    // hem de tek bir personel kaydına bağlı olmadığı için bu
                    // alanların dolu olması anlamsız, hatta yetkilendirme
                    // mantığında yanlış kısıtlamaya yol açabilirdi.
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new Exception("Admin kullanıcı seed edilemedi: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}