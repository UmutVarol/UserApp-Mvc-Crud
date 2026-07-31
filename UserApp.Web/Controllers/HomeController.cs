using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using UserApp.Services;
using UserApp.Entities.Dtos;
using UserApp.Web.Models;

namespace UserApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        private readonly IWebHostEnvironment _env; // Sunucu yolunu bulmamızı sağlayacak araç
        private const int PageSize = 8;

        public HomeController(UserService userService, IWebHostEnvironment env)
        {
            _userService = userService;
            _env = env;
        }

        public async Task<IActionResult> Index(string? q, string? sort, int page = 1)
        {
            if (page < 1) page = 1;

            var (items, totalCount) = await _userService.GetPagedAsync(q, sort, page, PageSize);
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

        public async Task<IActionResult> Create()
        {
            await PopulateDepartmanlarAsync();
            return View(new KullaniciCreateDto());
        }

        [HttpPost]
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

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _userService.GetForEditAsync(id);
            if (dto == null) return NotFound();

            await PopulateDepartmanlarAsync(dto.DepartmanId);
            return View(dto);
        }

        [HttpPost]
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

        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _userService.GetDetailAsync(id);
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteAsync(id);
            TempData["ToastMessage"] = "Kullanıcı Silindi.";
            TempData["ToastType"] = "danger";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
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
            var (items, _) = await _userService.GetPagedAsync(null, null, 1, 1000);

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
/// "Detay" butonuna tıklandığında JS'in çağırdığı endpoint. Tarayıcıya önceden
/// gömülmüş veri yerine, o an DB'den taze çekilen tek kullanıcının bilgisini
/// JSON olarak döner. Bu sayede liste satırlarının HTML'inde artık yalnızca
/// id/ad/soyad gibi asgari bilgi taşınır, hassas alanlar (email, foto, durum)
/// yalnızca kullanıcı gerçekten "Detay"a bastığında sunucudan çekilir.
[HttpGet]
public async Task<IActionResult> GetDetailJson(int id)
{
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