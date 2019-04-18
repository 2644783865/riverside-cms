using System;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Applications.Web.Mvc.Models;

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
        public IActionResult ReadPageElement(long pageId, Guid elementTypeId, long elementId)
        {
            return View(new PageElementKey { PageId = pageId, ElementTypeId = elementTypeId, ElementId = elementId });
        }

        [HttpGet]
        public IActionResult UpdatePageElement(long pageId, Guid elementTypeId, long elementId)
        {
            return View(new PageElementKey { PageId = pageId, ElementTypeId = elementTypeId, ElementId = elementId });
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
 