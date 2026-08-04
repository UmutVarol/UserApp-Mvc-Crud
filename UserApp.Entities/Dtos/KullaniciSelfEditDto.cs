using Microsoft.AspNetCore.Http;

namespace UserApp.Entities.Dtos
{
    /// <summary>
    /// "User" rolündeki bir personelin KENDİ profilini düzenlerken kullandığı
    /// DTO. BİLİNÇLİ OLARAK KullaniciEditDto'dan AYRI: DepartmanId ve IsActive
    /// alanlarını hiç içermez — bu sayede bir kullanıcı formu manipüle edip
    /// bu alanları POST etmeye çalışsa bile, model binder böyle bir property
    /// bulamayacağı için mass-assignment ile departman/aktiflik değiştirme
    /// İMKANSIZ hale gelir (SRP + güvenlik: minimum yetki ilkesi).
    /// Id de bilinçli olarak YOK — hangi kaydın güncelleneceği, formdan değil,
    /// giriş yapan kişinin ClaimsPrincipal'ındaki KullaniciId claim'inden
    /// (ProfileController tarafından) belirlenir.
    /// </summary>
    public class KullaniciSelfEditDto
    {
        // Salt okunur alanlar: view'da düz metin olarak gösterilir, hiçbir
        // <input> ile bağlanmaz, bu yüzden POST'ta bu değerler zaten gelmez.
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public string? DepartmanAdi { get; set; }

        // Gerçekten düzenlenebilir alanlar:
        public string? Email { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? ProfileImagePath { get; set; }
    }
}