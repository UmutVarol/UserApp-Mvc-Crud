using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UserApp.Services;
using UserApp.Entities;
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
            var (toplam, departmanSayisi, sonEklenen) = await _userService.GetSummaryAsync();

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
                SonEklenen = sonEklenen
            };

            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDepartmanlarAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Kullanici kullanici)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDepartmanlarAsync(kullanici.DepartmanId);
                return View(kullanici);
            }

            await _userService.AddAsync(kullanici);
            TempData["ToastMessage"] = $"{kullanici.Ad} {kullanici.Soyad} başarıyla eklendi.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            await PopulateDepartmanlarAsync(user.DepartmanId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Kullanici kullanici)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDepartmanlarAsync(kullanici.DepartmanId);
                return View(kullanici);
            }

            await _userService.UpdateAsync(kullanici);
            TempData["ToastMessage"] = "Değişiklikler kaydedildi.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
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
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        private async Task PopulateDepartmanlarAsync(int? selectedId = null)
        {
            var departmanlar = await _userService.GetDepartmanlarAsync();
            ViewBag.Departmanlar = new SelectList(departmanlar, "Id", "Ad", selectedId);
        }
    }
}