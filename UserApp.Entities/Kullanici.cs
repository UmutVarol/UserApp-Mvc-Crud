namespace UserApp.Entities
{
    /// <summary>
    /// DAL (Veri Erişim Katmanı) katmanının konuştuğu saf model. 
    /// Veritabanındaki 'Kullanicilar' tablosunun C# tarafındaki birebir karşılığıdır.
    /// </summary>
    public class Kullanici
    {
        public int Id { get; set; }
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public string? Email { get; set; }
        public int DepartmanId { get; set; }
        public Departman? Departman { get; set; }
        /// Kullanıcının sisteme ilk eklendiği (işe başlama) tarih ve saati tutar.
        /// Nesne ilk oluşturulduğunda varsayılan olarak o anki zamanı (DateTime.Now) alır.
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        /// Kullanıcının sistemde aktif mi yoksa pasif mi olduğunu tutar.
        /// Yeni eklenen bir kullanıcı varsayılan olarak aktiftir (true).
        public bool IsActive { get; set; } = true;

        /// Soft Delete (Yumuşak Silme) işlemi için kullanılır. 
        /// Kullanıcı silindiğinde veritabanından tamamen uçurulmaz, bu değer 'true' yapılır.
        public bool IsDeleted { get; set; } = false;

        /// Kullanıcının profil fotoğrafının sunucudaki (wwwroot içindeki) dosya yolunu tutar.
        /// Örn: "/uploads/profiles/ahmet.jpg". 
        /// Resmin kendisini (byte olarak) değil, sadece yolunu (string olarak) veritabanında saklarız.
        public string? ProfileImagePath { get; set; }
    }
}