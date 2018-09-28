using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace RiversideCms.Mvc.Extensions
{
    public static class PageHelperExtensions
    {
        private static void AddTagNames(RouteValueDictionary routeValueDictionary, IEnumerable<string> tagNames)
        {
            if (tagNames != null)
                routeValueDictionary.Add("tags", string.Join('+', tagNames.OrderBy(t => t)));
        }

        private static void AddPageDetails(RouteValueDictionary routeValueDictionary, long pageId, string pageName, bool home)
        {
            if (!home)
            {
                routeValueDictionary.Add("pageid", pageId);
                routeValueDictionary.Add("description", UrlUtils.UrlFriendly(pageName));
            }
        }

        private static string GetRouteName(bool home, IEnumerable<string> tagNames)
        {
            bool tags = tagNames != null && tagNames.Any();
            switch (home)
            {
                case true:
                    return tags ? RouteNames.HomeTagged : RouteNames.Home;

                default:
                    return tags ? RouteNames.PageTagged : RouteNames.Page;
            }
        }

        private static RouteValueDictionary GetRouteValueDictionary(long pageId, string pageName, bool home, IEnumerable<string> tagNames, object values)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary(values);
            AddTagNames(routeValueDictionary, tagNames);
            AddPageDetails(routeValueDictionary, pageId, pageName, home);
            return routeValueDictionary;
        }

        private static string GetRouteUrl(IUrlHelper urlHelper, long pageId, string pageName, bool home, IEnumerable<string> tagNames, object values)
        {
            RouteValueDictionary routeValueDictionary = GetRouteValueDictionary(pageId, pageName, home, tagNames, values);
            string routeName = GetRouteName(home, tagNames);
            string routeUrl = urlHelper.RouteUrl(routeName, routeValueDictionary);
            return routeUrl.Replace("%2B", "+");
        }

        public static IHtmlContent PageLink(this IHtmlHelper htmlHelper, string linkText, long pageId, string pageName, bool home, IEnumerable<string> tagNames, object values)
        {
            IUrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext);
            string routeUrl = GetRouteUrl(urlHelper, pageId, pageName, home, tagNames, values);
            return new HtmlString($"<a href=\"{routeUrl}\">{WebUtility.HtmlEncode(linkText)}</a>");
        }

        public static string PageUrl(this IUrlHelper urlHelper, long pageId, string pageName, bool home, IEnumerable<string> tagNames, object values)
        {
            return GetRouteUrl(urlHelper, pageId, pageName, home, tagNames, values);
        }
    }
}
