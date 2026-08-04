using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UserApp.Entities;
using UserApp.Services;
using UserApp.Web.Models;

namespace UserApp.Web.Controllers
{
    /// <summary>
    /// SADECE Admin'e açık: sisteme giriş yapabilen hesapları listeler,
    /// yeni hesap oluşturur, rol/departman/personel bağlantısını düzenler.
    /// UserManager/RoleManager (Identity'nin kendi API'leri) doğrudan
    /// kullanılır — bunları tekrar bir Repository'ye sarmak Microsoft'un
    /// zaten sağladığı soyutlamayı gereksiz yere tekrarlamak olurdu.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AccountManagementController : Controller
    {
        private static readonly string[] Roller = { "Admin", "DepartmanYoneticisi", "User" };

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public AccountManagementController(UserManager<ApplicationUser> userManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var vmList = new List<UserAccountListItemViewModel>();

            var departmanlar = await _userService.GetDepartmanlarAsync();
            var kullanicilar = await _userService.GetKullanicilarForSelectAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                vmList.Add(new UserAccountListItemViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    AdSoyad = user.AdSoyad,
                    Rol = roles.FirstOrDefault() ?? "(rol yok)",
                    BagliPersonelAdSoyad = user.KullaniciId.HasValue
                        ? kullanicilar.FirstOrDefault(k => k.Id == user.KullaniciId.Value)?.AdSoyad
                        : null,
                    YonetilenDepartmanAdi = user.YonetilenDepartmanId.HasValue
                        ? departmanlar.FirstOrDefault(d => d.Id == user.YonetilenDepartmanId.Value)?.Ad
                        : null
                });
            }

            return View(vmList.OrderBy(v => v.Email).ToList());
        }

        public async Task<IActionResult> Create()
        {
            var vm = new AccountFormViewModel();
            await PopulateSelectListsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountFormViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Password))
            {
                ModelState.AddModelError(nameof(vm.Password), "Yeni hesap için şifre zorunludur.");
            }

            await ValidateRoleSpecificFieldsAsync(vm, excludeUserId: null);

            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync(vm);
                return View(vm);
            }

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                EmailConfirmed = true,
                KullaniciId = vm.SecilenRol == "User" ? vm.KullaniciId : null,
                YonetilenDepartmanId = vm.SecilenRol == "DepartmanYoneticisi" ? vm.YonetilenDepartmanId : null
            };

            var createResult = await _userManager.CreateAsync(user, vm.Password!);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                await PopulateSelectListsAsync(vm);
                return View(vm);
            }

            await _userManager.AddToRoleAsync(user, vm.SecilenRol);

            TempData["ToastMessage"] = $"{vm.Email} için hesap oluşturuldu.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var vm = new AccountFormViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                SecilenRol = roles.FirstOrDefault() ?? "User",
                YonetilenDepartmanId = user.YonetilenDepartmanId,
                KullaniciId = user.KullaniciId
            };

            await PopulateSelectListsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(AccountFormViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.Id)) return NotFound();

            var user = await _userManager.FindByIdAsync(vm.Id);
            if (user == null) return NotFound();

            await ValidateRoleSpecificFieldsAsync(vm, excludeUserId: user.Id);

            // Kendi kendini Admin'likten düşürmeyi engelliyoruz — aksi halde
            // son admin yanlışlıkla kendi yetkisini kaldırıp sistemden dışlanabilir.
            if (user.Id == _userManager.GetUserId(User) && vm.SecilenRol != "Admin")
            {
                ModelState.AddModelError(string.Empty, "Kendi hesabınızın Admin rolünü kaldıramazsınız.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync(vm);
                return View(vm);
            }

            // Şifre alanı doluysa değiştir, boşsa mevcut şifreye dokunma.
            if (!string.IsNullOrWhiteSpace(vm.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, vm.Password);
                if (!resetResult.Succeeded)
                {
                    foreach (var error in resetResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    await PopulateSelectListsAsync(vm);
                    return View(vm);
                }
            }

            var mevcutRoller = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, mevcutRoller);
            await _userManager.AddToRoleAsync(user, vm.SecilenRol);

            user.KullaniciId = vm.SecilenRol == "User" ? vm.KullaniciId : null;
            user.YonetilenDepartmanId = vm.SecilenRol == "DepartmanYoneticisi" ? vm.YonetilenDepartmanId : null;
            await _userManager.UpdateAsync(user);

            TempData["ToastMessage"] = $"{user.Email} güncellendi.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        /// Rol'e göre zorunlu/anlamsız alanları kontrol eder ve iş kurallarını uygular:
        /// - DepartmanYoneticisi seçiliyse departman zorunlu.
        /// - User seçiliyse personel kaydı zorunlu VE o kayıt BAŞKA bir hesaba bağlı olmamalı.
        private async Task ValidateRoleSpecificFieldsAsync(AccountFormViewModel vm, string? excludeUserId)
        {
            if (vm.SecilenRol == "DepartmanYoneticisi" && vm.YonetilenDepartmanId == null)
            {
                ModelState.AddModelError(nameof(vm.YonetilenDepartmanId), "Departman Yöneticisi için departman seçilmelidir.");
            }

            if (vm.SecilenRol == "User")
            {
                if (vm.KullaniciId == null)
                {
                    ModelState.AddModelError(nameof(vm.KullaniciId), "User rolü için bağlı personel kaydı seçilmelidir.");
                }
                else
                {
                    var baskaHesapVarMi = _userManager.Users.Any(u =>
                        u.KullaniciId == vm.KullaniciId && u.Id != excludeUserId);

                    if (baskaHesapVarMi)
                    {
                        ModelState.AddModelError(nameof(vm.KullaniciId), "Bu personel kaydı zaten başka bir hesaba bağlı.");
                    }
                }
            }

            await Task.CompletedTask;
        }

        private async Task PopulateSelectListsAsync(AccountFormViewModel vm)
        {
            vm.RolSecenekleri = Roller.Select(r => new SelectListItem(r, r)).ToList();

            var departmanlar = await _userService.GetDepartmanlarAsync();
            vm.DepartmanSecenekleri = departmanlar.Select(d => new SelectListItem(d.Ad, d.Id.ToString())).ToList();

            var kullanicilar = await _userService.GetKullanicilarForSelectAsync();
            vm.KullaniciSecenekleri = kullanicilar.Select(k => new SelectListItem(k.AdSoyad, k.Id.ToString())).ToList();
        }
    }
}