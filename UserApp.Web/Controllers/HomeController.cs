using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using UserApp.Services;
using UserApp.Entities.Dtos;
using UserApp.Web.Models;

namespace UserApp.Web.Controllers
{
    /// <summary>
    /// GÜNCELLEME: Edit action'ı artık SADECE Admin'e açık (Ad/Soyad/Email/
    /// Departman/Foto değiştirme yetkisi). DepartmanYoneticisi bu action'a
    /// hiç erişemez — onun için ayrı ve minimal bir ToggleStatus action'ı var,
    /// sadece IsActive alanını değiştirir, başka hiçbir alana dokunmaz.
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IWebHostEnvironment _env;
        private const int PageSize = 8;

        public HomeController(
            IUserService userService,
            IAuthorizationService authorizationService,
            IWebHostEnvironment env)
        {
            _userService = userService;
            _authorizationService = authorizationService;
            _env = env;
        }

        private (int? departmanFiltre, bool sadeceAktif) ResolveListScope()
        {
            if (User.IsInRole("DepartmanYoneticisi"))
            {
                var claim = User.FindFirst("YonetilenDepartmanId")?.Value;
                if (int.TryParse(claim, out var yid))
                    return (yid, false);

                return (-1, false);
            }

            if (User.IsInRole("User"))
            {
                return (null, true);
            }

            return (null, false);
        }

        public async Task<IActionResult> Index(string? q, string? sort, int page = 1)
        {
            if (page < 1) page = 1;

            var (departmanFiltre, sadeceAktif) = ResolveListScope();

            var (items, totalCount) = await _userService.GetPagedAsync(q, sort, page, PageSize, departmanFiltre, sadeceAktif);
            var (toplam, departmanSayisi, sonEklenenAdSoyad) = await _userService.GetSummaryAsync();

            var vm = new KullaniciListViewModel
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = PageSize,
                SearchTerm = q,
                SortBy = sort,
                ToplamKullanici = toplam,
                DepartmanSayisi = departmanSayisi,
                SonEklenenAdSoyad = sonEklenenAdSoyad
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateDepartmanlarAsync();
            return View(new KullaniciCreateDto());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KullaniciCreateDto dto)
        {
            var result = await _userService.AddAsync(dto, _env.WebRootPath);
            if (!result.Success)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);

                await PopulateDepartmanlarAsync(dto.DepartmanId);
                return View(dto);
            }

            TempData["ToastMessage"] = $"{dto.Ad} {dto.Soyad} Başarıyla Eklendi.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        /// GÜNCELLEME: Artık SADECE Admin. DepartmanYoneticisi buraya asla
        /// erişemez — dolayısıyla "departman değiştirilemez" gibi eski
        /// kısıtlama kontrollerine burada artık gerek yok (dead code temizlendi).
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _userService.GetForEditAsync(id);
            if (dto == null) return NotFound();

            await PopulateDepartmanlarAsync(dto.DepartmanId);
            return View(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(KullaniciEditDto dto)
        {
            var result = await _userService.UpdateAsync(dto, _env.WebRootPath);
            if (!result.Success)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);

                await PopulateDepartmanlarAsync(dto.DepartmanId);
                return View(dto);
            }

            TempData["ToastMessage"] = "Değişiklikler Kaydedildi.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// YENİ: DepartmanYoneticisi'nin kendi departmanındaki bir kullanıcıyı
        /// aktif/pasif yapabildiği TEK eylem. Ad/Soyad/Email/Departman/Foto
        /// alanlarının HİÇBİRİ bu action'ın parametrelerinde yok — bu yüzden
        /// bir DepartmanYoneticisi, tarayıcıda formu/isteği manipüle etse bile
        /// bu alanları değiştirmesi YAPISAL OLARAK imkansızdır (DTO'da alan yok).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,DepartmanYoneticisi")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            var accessInfo = await _userService.GetAccessInfoAsync(id);
            if (accessInfo == null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, accessInfo, "CanManageKullanici");
            if (!authResult.Succeeded) return Forbid();

            var result = await _userService.UpdateStatusAsync(id, isActive);
            if (!result.Success)
            {
                TempData["ToastMessage"] = string.Join(" ", result.Errors);
                TempData["ToastType"] = "danger";
            }
            else
            {
                TempData["ToastMessage"] = isActive ? "Kullanıcı aktif edildi." : "Kullanıcı pasif edildi.";
                TempData["ToastType"] = "success";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,DepartmanYoneticisi")]
        public async Task<IActionResult> Delete(int id)
        {
            var accessInfo = await _userService.GetAccessInfoAsync(id);
            if (accessInfo == null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, accessInfo, "CanManageKullanici");
            if (!authResult.Succeeded) return Forbid();

            var dto = await _userService.GetDetailAsync(id);
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,DepartmanYoneticisi")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accessInfo = await _userService.GetAccessInfoAsync(id);
            if (accessInfo == null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, accessInfo, "CanManageKullanici");
            if (!authResult.Succeeded) return Forbid();

            await _userService.DeleteAsync(id);
            TempData["ToastMessage"] = "Kullanıcı Silindi.";
            TempData["ToastType"] = "danger";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var accessInfo = await _userService.GetAccessInfoAsync(id);
            if (accessInfo == null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, accessInfo, "CanViewKullaniciDetail");
            if (!authResult.Succeeded) return Forbid();

            var dto = await _userService.GetDetailAsync(id);
            if (dto == null) return NotFound();
            return View(dto);
        }

        private async Task PopulateDepartmanlarAsync(int? selectedId = null)
        {
            var departmanlar = await _userService.GetDepartmanlarAsync();
            ViewBag.Departmanlar = new SelectList(departmanlar, "Id", "Ad", selectedId);
        }

        [HttpGet]
        public async Task<IActionResult> GetKullanicilarJson()
        {
            var (departmanFiltre, sadeceAktif) = ResolveListScope();

            var (items, _) = await _userService.GetPagedAsync(null, null, 1, 1000, departmanFiltre, sadeceAktif);

            var jsonVeri = items.Select(k => new
            {
                id = k.Id,
                ad = k.Ad ?? "",
                soyad = k.Soyad ?? "",
                email = k.Email ?? "",
                departmanAd = k.DepartmanAdi ?? "",
                kayitTarihi = k.KayitTarihi.ToString("dd.MM.yyyy HH:mm"),
                isActive = k.IsActive,
                profileImagePath = k.ProfileImagePath
            });

            return Json(new { data = jsonVeri });
        }

        [HttpGet]
        public async Task<IActionResult> GetDetailJson(int id)
        {
            var accessInfo = await _userService.GetAccessInfoAsync(id);
            if (accessInfo == null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, accessInfo, "CanViewKullaniciDetail");
            if (!authResult.Succeeded) return Forbid();

            var dto = await _userService.GetDetailAsync(id);
            if (dto == null) return NotFound();

            return Json(new
            {
                id = dto.Id,
                adSoyad = $"{dto.Ad} {dto.Soyad}",
                email = dto.Email,
                departmanAd = dto.DepartmanAdi,
                kayitTarihi = dto.KayitTarihi.ToString("dd.MM.yyyy HH:mm"),
                isActive = dto.IsActive,
                profileImagePath = dto.ProfileImagePath
            });
        }
    }
}