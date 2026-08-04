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

        /// Yöneticinin Edit ekranındaki toggle switch üzerinden değiştirdiği
        /// aktif/pasif durumu. Formdan checkbox olarak geldiği için model
        /// binding sırasında işaretli değilse "false" olarak bağlanır.
        public bool IsActive { get; set; }

        /// Kullanıcının arayüzden seçtiği fiziksel dosya (Resim).
        public IFormFile? ProfileImage { get; set; }
        public string? ProfileImagePath { get; set; }
    }
}