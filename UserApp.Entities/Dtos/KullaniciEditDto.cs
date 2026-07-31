using Microsoft.AspNetCore.Http;

namespace UserApp.Entities.Dtos
{
    /// Kullanıcı güncelleme ekranından backend'e gelen verileri taşıyan kurye nesnemiz.
    /// CreateDto'dan farkı, güncellenecek kaydın Id'sini de bilmek zorunda olmasıdır.
    public class KullaniciEditDto
    {
        public int Id { get; set; }
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public string? Email { get; set; }
        public int DepartmanId { get; set; }

        /// Kullanıcının arayüzden seçtiği fiziksel dosya (Resim).
        /// Güncelleme sırasında yeni resim seçilmezse null gelebilir.
        public IFormFile? ProfileImage { get; set; }
        public string? ProfileImagePath { get; set; }
    }
}