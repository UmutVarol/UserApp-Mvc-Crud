using System.ComponentModel.DataAnnotations;

namespace UserApp.Web.Models
{
    /// <summary>
    /// Login formundan gelen veriyi taşır. Bilinçli olarak UserApp.Entities.Dtos
    /// katmanına değil, Web.Models altına konuldu: bu saf bir UI/form modeli,
    /// iş domaini (Kullanici/Departman) ile ilgisi yok, Service katmanına
    /// hiç gitmeyecek — doğrudan Controller içinde SignInManager'a veriliyor.
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi girin.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }

        /// Giriş başarılı olduktan sonra kullanıcının geri döneceği sayfa
        /// (ör. yetkisiz erişim denemesi sonrası login'e düşen kullanıcıyı
        /// tekrar gitmek istediği sayfaya yönlendirmek için).
        public string? ReturnUrl { get; set; }
    }
}