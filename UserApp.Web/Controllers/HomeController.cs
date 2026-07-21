using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserApp.Services;
using UserApp.Entities;

namespace UserApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        public HomeController(UserService userService) { _userService = userService; }

        public async Task<IActionResult> Index() => View(await _userService.GetAllAsync());

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Kullanici kullanici)
        {
            if (!ModelState.IsValid) return View(kullanici); 
            
            await _userService.AddAsync(kullanici);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Kullanici kullanici)
        {
            if (!ModelState.IsValid) return View(kullanici); 
            
            await _userService.UpdateAsync(kullanici);
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
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }
    }
}