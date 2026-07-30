using FluentValidation;
using UserApp.Entities.Dtos;

namespace UserApp.Services.Validation
{
    public class KullaniciCreateDtoValidator : AbstractValidator<KullaniciCreateDto>
    {
        // Sadece harf (Türkçe karakterler dahil) ve tek boşluk içeren isimlere izin verir.
        // Örn: "Ahmet Can" geçer, "umut 123123412 muhammed" geçmez.
        private const string IsimDeseni = @"^[a-zA-ZğüşıöçĞÜŞİÖÇ]+(\s[a-zA-ZğüşıöçĞÜŞİÖÇ]+)*$";

        public KullaniciCreateDtoValidator()
        {
            RuleFor(x => x.Ad)
                .NotEmpty().WithMessage("Ad alanı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.")
                .Matches(IsimDeseni).WithMessage("Ad yalnızca harf içerebilir, rakam veya özel karakter olamaz.");

            RuleFor(x => x.Soyad)
                .NotEmpty().WithMessage("Soyad alanı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.")
                .Matches(IsimDeseni).WithMessage("Soyad yalnızca harf içerebilir, rakam veya özel karakter olamaz.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email alanı boş bırakılamaz.")
                .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");

            RuleFor(x => x.DepartmanId)
                .GreaterThan(0).WithMessage("Departman seçilmelidir.");
        }
    }
}