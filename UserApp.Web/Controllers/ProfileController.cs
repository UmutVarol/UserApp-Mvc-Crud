using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using UserApp.Entities.Dtos;
using UserApp.Services;

namespace UserApp.Web.Controllers
{
    /// <summary>
    /// Giriş yapan kişinin KENDİ profilini (Email + Fotoğraf) düzenlemesini
    /// sağlar. HomeController'daki Edit'ten kasıtlı olarak ayrı: farklı DTO
    /// (KullaniciSelfEditDto), farklı yetki modeli (rol değil, KullaniciId
    /// claim'i var mı yok mu) kullanır.
    /// </summary>
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _env;

        public ProfileController(IUserService userService, IWebHostEnvironment env)
        {
            _userService = userService;
            _env = env;
        }

        /// Giriş yapan hesabın hangi personel kaydına (Kullanici.Id) bağlı
        /// olduğunu, formdan DEĞİL, ClaimsPrincipal'dan okur — bu yüzden bir
        /// kullanıcının "başka birinin ID'siyle profilime giriyorum" diye bir
        /// URL/form manipülasyonu yapması mümkün değildir, ID hiçbir zaman
        /// dışarıdan (query/form) alınmaz.
        private int? GetKullaniciId()
        {
            var claim = User.FindFirst("KullaniciId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        public async Task<IActionResult> Edit()
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null)
            {
                // Admin/DepartmanYoneticisi gibi bir Kullanici kaydına bağlı
                // OLMAYAN hesaplar için: düzenlenecek bir "kendi profili" yok.
                return Forbid();
            }

            var dto = await _userService.GetSelfEditAsync(kullaniciId.Value);
            if (dto == null) return NotFound();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(KullaniciSelfEditDto dto)
        {
            var kullaniciId = GetKullaniciId();
            if (kullaniciId == null) return Forbid();

            var result = await _userService.UpdateSelfAsync(kullaniciId.Value, dto, _env.WebRootPath);
            if (!result.Success)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);

                // Salt-okunur alanları (Ad/Soyad/Departman) hata sonrası
                // formda boş göstermemek için tekrar dolduruyoruz.
                var refreshed = await _userService.GetSelfEditAsync(kullaniciId.Value);
                if (refreshed != null)
                {
                    dto.Ad = refreshed.Ad;
                    dto.Soyad = refreshed.Soyad;
                    dto.DepartmanAdi = refreshed.DepartmanAdi;
                    dto.ProfileImagePath = refreshed.ProfileImagePath;
                }
                return View(dto);
            }

            TempData["ToastMessage"] = "Profiliniz güncellendi.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Edit));
        }
    }
}