﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Riverside.Cms.Services.Core.Client;

namespace RiversideCms.Mvc.Extensions
{
    public static class PageHtmlHelperExtensions
    {
        public static IHtmlContent PageLink(this IHtmlHelper htmlHelper, Page page)
        {
            if (page.ParentPageId == null)
                return htmlHelper.RouteLink(page.Name, "Home");
            else
                return htmlHelper.RouteLink(page.Name, "Page", new { pageId = page.PageId, description = UrlUtils.UrlFriendly(page.Name) });
        }

        public static IHtmlContent PageLink(this IHtmlHelper htmlHelper, string linkText, long pageId, string pageName, bool home)
        {
            if (home)
                return htmlHelper.RouteLink(linkText, "Home");
            else
                return htmlHelper.RouteLink(linkText, "Page", new { pageId = pageId, description = UrlUtils.UrlFriendly(pageName) });
        }

        public static IHtmlContent PageTaggedLink(this IHtmlHelper htmlHelper, string linkText, long pageId, string pageName, bool home, string tag)
        {
            if (home)
                return htmlHelper.RouteLink(linkText, "HomeTagged", new { tags = tag });
            else
                return htmlHelper.RouteLink(linkText, "PageTagged", new { pageId = pageId, description = UrlUtils.UrlFriendly(pageName), tags = tag });
        }
    }
}
