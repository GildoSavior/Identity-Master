using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult index()
        {
            return View();
        }

        public async Task<IActionResult> Register()
        {
            RegisterViewModel register = new RegisterViewModel();
            return View(register);
        }
    }
}