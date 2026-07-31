namespace UserApp.Entities.Dtos
{
    public class KullaniciDetailDto
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DepartmanAdi { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
    }
}
