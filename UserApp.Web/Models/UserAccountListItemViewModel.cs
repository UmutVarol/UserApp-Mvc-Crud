namespace UserApp.Web.Models
{
    public class UserAccountListItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? AdSoyad { get; set; }
        public string Rol { get; set; } = string.Empty;
        public string? BagliPersonelAdSoyad { get; set; }
        public string? YonetilenDepartmanAdi { get; set; }
    }
}