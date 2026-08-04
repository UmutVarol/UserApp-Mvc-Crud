using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserApp.Entities;
using UserApp.Web.Models;

namespace UserApp.Web.Controllers
{
    /// <summary>
    /// Giriş/çıkış akışını yönetir. Identity'nin SignInManager/UserManager
    /// servislerini kullanır — parola hash'leme, lockout, cookie oluşturma
    /// gibi işlerin hiçbirini kendimiz yazmıyoruz, Identity'ye bırakıyoruz.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // PasswordSignInAsync: email+şifreyi doğrular, başarılıysa cookie oluşturur.
            // lockoutOnFailure: true -> Program.cs'de tanımladığımız 5 başarısız
            // denemede 10 dakika kilitleme kuralı burada devreye girer.
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Hesabınız çok fazla başarısız denemeden dolayı geçici olarak kilitlendi. Lütfen 10 dakika sonra tekrar deneyin.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Email veya şifre hatalı.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}