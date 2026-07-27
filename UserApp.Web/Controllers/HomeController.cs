using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UserApp.Services;
using UserApp.Entities.Dtos;
using UserApp.Web.Models;

namespace UserApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        private const int PageSize = 8;

        public HomeController(UserService userService)
        {
            _userService = userService;
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
            var result = await _userService.AddAsync(dto);
            if (!result.Success)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);

                await PopulateDepartmanlarAsync(dto.DepartmanId);
                return View(dto);
            }

            TempData["ToastMessage"] = $"{dto.Ad} {dto.Soyad} başarıyla eklendi.";
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
            var result = await _userService.UpdateAsync(dto);
            if (!result.Success)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);

                await PopulateDepartmanlarAsync(dto.DepartmanId);
                return View(dto);
            }

            TempData["ToastMessage"] = "Değişiklikler kaydedildi.";
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
            TempData["ToastMessage"] = "Kullanıcı silindi.";
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
    }
}