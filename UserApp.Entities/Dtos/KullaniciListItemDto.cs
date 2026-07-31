namespace UserApp.Entities.Dtos
{
    public class KullaniciListItemDto
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DepartmanAdi { get; set; } = string.Empty;

        public DateTime KayitTarihi { get; set; }
        public bool IsActive { get; set; }
    }
}
