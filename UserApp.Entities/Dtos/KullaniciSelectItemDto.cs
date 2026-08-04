namespace UserApp.Entities.Dtos
{
    /// <summary>
    /// "Bu login hesabı hangi personel kaydına bağlansın?" dropdown'ı için
    /// minimal veri taşıyıcı — tam Kullanici entity'sini yüklemeye gerek yok.
    /// </summary>
    public class KullaniciSelectItemDto
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
    }
}