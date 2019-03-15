using Microsoft.AspNetCore.Mvc;

namespace Riverside.Cms.Applications.Web.Mvc.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View("Logout");
        }

        [HttpGet]
        public IActionResult UpdateProfile()
        {
            return View("UpdateProfile");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View("ChangePassword");
        }
    }
}
