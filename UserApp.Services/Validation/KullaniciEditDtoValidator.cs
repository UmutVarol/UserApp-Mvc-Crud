using FluentValidation;
using UserApp.Entities.Dtos;

namespace UserApp.Services.Validation
{
    public class KullaniciEditDtoValidator : AbstractValidator<KullaniciEditDto>
    {
        public KullaniciEditDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);

            RuleFor(x => x.Ad)
                .NotEmpty().WithMessage("Ad alanı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");

            RuleFor(x => x.Soyad)
                .NotEmpty().WithMessage("Soyad alanı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email alanı boş bırakılamaz.")
                .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");

            RuleFor(x => x.DepartmanId)
                .GreaterThan(0).WithMessage("Departman seçilmelidir.");
        }
    }
}
