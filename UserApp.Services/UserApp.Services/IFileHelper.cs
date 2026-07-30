using Microsoft.AspNetCore.Http;

namespace UserApp.Services
{
  
    /// Dosya yükleme işlemlerini soyutlayan arayüz (Interface).
    /// Controller veya Service katmanlarının doğrudan dosya sistemiyle konuşmasını engeller (Gevşek bağlılık).

    public interface IFileHelper
    {
        /// Gelen dosyayı (resmi) wwwroot altındaki ilgili klasöre asenkron olarak kaydeder.
        /// <param name="file">Form üzerinden gelen yüklenecek dosya nesnesi</param>
        /// <param name="folderName">Kaydedilecek klasör adı (örn: 'profiles')</param>
        /// <returns>Kaydedilen dosyanın veritabanına yazılacak göreceli yolunu döndürür (örn: /uploads/profiles/resim.jpg)</returns>
        Task<string> UploadImageAsync(IFormFile file, string folderName);
    }
}