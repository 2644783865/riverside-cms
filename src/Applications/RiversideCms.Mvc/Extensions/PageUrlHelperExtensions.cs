using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RiversideCms.Mvc.Extensions
{
    public static class PageUrlHelperExtensions
    {
        public static string PageUrl(this IUrlHelper urlHelper, long pageId, string pageName, bool home)
        {
            if (home)
                return urlHelper.RouteUrl("Home");
            else
                return urlHelper.RouteUrl("Page", new { pageId, description = UrlUtils.UrlFriendly(pageName) });
        }
    }
}
