using Microsoft.AspNetCore.Mvc;

namespace Riverside.Cms.Applications.Web.Mvc.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Home()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ReadPage(long pageId)
        {
            return View(pageId);
        }

        [HttpGet]
        public IActionResult UpdatePage(long pageId)
        {
            return View(pageId);
        }

        [HttpGet]
        public IActionResult ReadWeb()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UpdateWeb()
        {
            return View();
        }
    }
}
 