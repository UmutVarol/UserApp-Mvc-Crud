using FluentValidation;
using UserApp.Entities.Dtos;

namespace UserApp.Services.Validation
{
    public class KullaniciCreateDtoValidator : AbstractValidator<KullaniciCreateDto>
    {
        public KullaniciCreateDtoValidator()
        {
            // Sadece Türkçe ve İngilizce harfler ile boşluk kabul eder. Rakam veya sembol giremez.
            var isimDeseni = @"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$";

            RuleFor(x => x.Ad)
                .NotEmpty().WithMessage("Ad alanı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.")
                .Matches(isimDeseni).WithMessage("Ad yalnızca harf içerebilir.");

            RuleFor(x => x.Soyad)
                .NotEmpty().WithMessage("Soyad alanı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.")
                .Matches(isimDeseni).WithMessage("Soyad yalnızca harf içerebilir.");

                  RuleFor(x => x.DepartmanId)
                .GreaterThan(0).WithMessage("Departman seçilmelidir.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email alanı boş bırakılamaz.")
                .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");
                // Email benzersizliğini (Unique) veritabanına bağlanması gerektiği için UserService içinde kontrol edeceğiz.
        }
    }
}