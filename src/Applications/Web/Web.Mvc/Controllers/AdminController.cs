using Microsoft.AspNetCore.Mvc;

namespace Riverside.Cms.Applications.Web.Mvc.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult UpdatePage(long pageId)
        {
            return View(pageId);
        }

        [HttpGet]
        public IActionResult ReadPage(long pageId)
        {
            return View(pageId);
        }
    }
}
 