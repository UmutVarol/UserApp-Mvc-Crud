using Microsoft.AspNetCore.Http;
using System.IO;

namespace UserApp.Services
{
    /// Dosya yükleme işlemlerini yönetir. 
    /// Güvenlik kontrollerini (Uzantı ve Boyut) yaparak dosyayı fiziksel klasöre kaydeder.
    public static class FileHelper
    {
        // 5 MB = 5 * 1024 * 1024 byte
        private const int MaxFileSize = 5 * 1024 * 1024; 
        
        // İzin verilen güvenli uzantılar
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

        /// Fotoğrafı wwwroot/uploads/profiles klasörüne benzersiz bir isimle kaydeder.
        /// Başarılıysa dosya yolunu, başarısızsa hata mesajını döner.
        public static async Task<(bool Success, string? FilePath, string? ErrorMessage)> UploadProfileImageAsync(IFormFile? file, string webRootPath)
        {
            if (file == null || file.Length == 0)
                return (true, null, null); // Fotoğraf seçmek zorunlu değilse null döner geçeriz

            // 1. GÜVENLİK KONTROLÜ: Dosya Boyutu
            if (file.Length > MaxFileSize)
                return (false, null, "Dosya boyutu maksimum 5 MB olabilir.");

            // 2. GÜVENLİK KONTROLÜ: Dosya Uzantısı
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return (false, null, "Sadece .jpg, .jpeg ve .png formatında resim yükleyebilirsiniz!");

            // Dosya çakışmalarını önler (Örn: 550e8400-e29b-41d4-a716-446655440000.jpg)
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            
            // Klasör Yolu: wwwroot/uploads/profiles
            var uploadsFolder = Path.Combine(webRootPath, "uploads", "profiles");

            // Eğer klasör yoksa sıfırdan oluştur
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var physicalFilePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Dosyayı sunucuya fiziksel olarak kopyala
            using (var stream = new FileStream(physicalFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Veritabanına kaydedilecek relative yolu dön
            return (true, $"/uploads/profiles/{uniqueFileName}", null);
        }
    }
}