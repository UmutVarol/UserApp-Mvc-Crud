using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace UserApp.Web.Models
{
    /// <summary>
    /// Hem "Yeni Hesap Oluştur" hem "Rolü Düzenle" formu için ortak model.
    /// Id boşsa (null/empty) yeni hesap oluşturma, doluysa mevcut hesabı
    /// düzenleme akışı olarak yorumlanır.
    /// </summary>
    public class AccountFormViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi girin.")]
        public string Email { get; set; } = string.Empty;

        /// Sadece yeni hesap oluştururken doldurulur; düzenlemede boş bırakılabilir
        /// (boşsa şifre değiştirilmez) — Controller bu ayrımı yapar.
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Rol seçilmelidir.")]
        public string SecilenRol { get; set; } = string.Empty;

        /// Sadece SecilenRol == "DepartmanYoneticisi" ise anlamlıdır.
        public int? YonetilenDepartmanId { get; set; }

        /// Sadece SecilenRol == "User" ise anlamlıdır.
        public int? KullaniciId { get; set; }

        public List<SelectListItem> RolSecenekleri { get; set; } = new();
        public List<SelectListItem> DepartmanSecenekleri { get; set; } = new();
        public List<SelectListItem> KullaniciSecenekleri { get; set; } = new();
    }
}