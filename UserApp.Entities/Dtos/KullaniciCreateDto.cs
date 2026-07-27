namespace UserApp.Entities.Dtos
{
    public class KullaniciCreateDto
    {
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public string? Email { get; set; }
        public int DepartmanId { get; set; }
    }
}
