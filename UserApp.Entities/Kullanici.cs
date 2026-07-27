namespace UserApp.Entities
{
    /// DAL katmanının konuştuğu saf model. Doğrulama kuralları ve ekran bazlı
    /// veri şekilleri burada değil, Dtos/ altındaki DTO sınıflarında ve
    /// UserApp.Services/Validation altındaki FluentValidation kurallarında yer alır.
    /// Şema kısıtları (Required/MaxLength/FK) ise AppDbContext.OnModelCreating içinde tanımlıdır.
    public class Kullanici
    {
        public int Id { get; set; }
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public string? Email { get; set; }
        public int DepartmanId { get; set; }
        public Departman? Departman { get; set; }
    }
}