using Microsoft.AspNetCore.Http;

namespace UserApp.Entities.Dtos
{
    /// Kullanıcı ekleme ekranından (View) backend'e gelen verileri taşıyan nesne.
    /// DB nesnesini (Kullanici.cs) doğrudan ekrana açmamak için DTO kullanıyoruz.

    public class KullaniciCreateDto
    {
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public string? Email { get; set; }
        public int DepartmanId { get; set; }


        /// Kullanıcının arayüzden seçtiği fiziksel dosya (Resim).
        /// Veritabanına kaydolmaz, sadece Controller'a kadar taşınması için kullanılır.
        public IFormFile? ProfileImage { get; set; }
    }
}