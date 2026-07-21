using System.ComponentModel.DataAnnotations;

namespace UserApp.Entities
{
    public class Kullanici
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad alanı boş bırakılamaz.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string? Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı boş bırakılamaz.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string? Soyad { get; set; }

        [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Departman alanı boş bırakılamaz.")]
        [StringLength(50, ErrorMessage = "Departman adı en fazla 50 karakter olabilir.")]
        public string? Departman { get; set; }
    }
}