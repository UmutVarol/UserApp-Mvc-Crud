namespace UserApp.Entities.Dtos
{
    /// <summary>
    /// Yetkilendirme (authorization) kararları için kullanılan minimal veri taşıyıcı.
    /// Sadece Edit/Delete/Detail policy'lerinin karar vermek için ihtiyaç duyduğu
    /// alanları taşır — tam Kullanici entity'sini her yetki kontrolünde
    /// yüklemek gereksiz veritabanı yüküdür.
    /// </summary>
    public class KullaniciAccessInfo
    {
        public int DepartmanId { get; set; }
        public bool IsActive { get; set; }
    }
}