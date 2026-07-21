using Microsoft.AspNetCore.Mvc;
using UserApp.Services;
using UserApp.Entities;

namespace UserApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        public HomeController(UserService userService) { _userService = userService; }

        public IActionResult Index() => View(_userService.GetAll());

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Kullanici kullanici)
        {
            // Validasyon (Doğrulama) Kontrolü
            if (!ModelState.IsValid) return View(kullanici); 
            
            _userService.Add(kullanici);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(Kullanici kullanici)
        {
            // Validasyon (Doğrulama) Kontrolü
            if (!ModelState.IsValid) return View(kullanici); 
            
            _userService.Update(kullanici);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _userService.Delete(id);
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }
    }
}