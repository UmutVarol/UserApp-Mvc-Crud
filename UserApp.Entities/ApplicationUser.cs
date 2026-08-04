using Microsoft.AspNetCore.Identity;

namespace UserApp.Entities
{
    /// Sisteme giriş yapan (login olan) kişiyi temsil eder — IdentityUser'ı genişletir.
    /// DİKKAT: "Kullanici" entity'siyle KARIŞTIRILMAMALIDIR:
    /// - ApplicationUser: "Sisteme kim giriş yapıyor?"
    /// - Kullanici: "Sistemde yönetilen personel kaydı"
    public class ApplicationUser : IdentityUser
    {
        public string? AdSoyad { get; set; }

        /// Bu login hesabının karşılık geldiği personel kaydı (Kullanici.Id).
        /// SADECE "User" rolündeki hesaplar için doldurulur — "kendi profilini
        /// düzenleme/görme" yetkilendirmesi bu alan üzerinden çalışacak.
        /// Admin ve DepartmanYoneticisi için null kalabilir (onların ayrıca
        /// bir personel kaydı olmasına gerek yok, sadece giriş hesabıdırlar).
        public int? KullaniciId { get; set; }

        /// Bu hesap "DepartmanYoneticisi" rolündeyse, hangi departmanı
        /// yönettiğini belirtir (Departman.Id). Diğer roller için null.
        /// Departman bazlı erişim kontrolü (CanViewKullanici/CanEditKullanici
        /// policy'leri) bu alanı, ilgili Kullanici kaydının DepartmanId'siyle
        /// karşılaştırarak karar verecek.
        public int? YonetilenDepartmanId { get; set; }
    }
}