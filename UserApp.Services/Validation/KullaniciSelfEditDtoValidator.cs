using FluentValidation;
using UserApp.Entities.Dtos;

namespace UserApp.Services.Validation
{
    /// <summary>
    /// Self-servis profil güncellemesi için doğrulama. Sadece Email'i kontrol
    /// eder — Ad/Soyad/Departman zaten bu DTO'da yok (yukarıdaki yoruma bakın).
    /// </summary>
    public class KullaniciSelfEditDtoValidator : AbstractValidator<KullaniciSelfEditDto>
    {
        public KullaniciSelfEditDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email alanı boş bırakılamaz.")
                .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");
        }
    }
}