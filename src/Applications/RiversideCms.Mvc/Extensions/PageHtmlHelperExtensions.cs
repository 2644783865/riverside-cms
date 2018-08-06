using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace RiversideCms.Mvc.Extensions
{
    public static class PageHtmlHelperExtensions
    {
        private static void AddTagNames(RouteValueDictionary routeValueDictionary, IEnumerable<string> tagNames)
        {
            if (tagNames != null)
                routeValueDictionary.Add("tags", string.Join('+', tagNames));
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
            string routeName = home ? "Home" : "Page";
            if (tagNames != null)
                routeName += "Tagged";
            return routeName;
        }

        public static IHtmlContent PageLink(this IHtmlHelper htmlHelper, string linkText, long pageId, string pageName, bool home, IEnumerable<string> tagNames, object values)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary(values);
            AddTagNames(routeValueDictionary, tagNames);
            AddPageDetails(routeValueDictionary, pageId, pageName, home);
            string routeName = GetRouteName(home, tagNames);
            return htmlHelper.RouteLink(linkText, routeName, routeValueDictionary);
        }
    }
}
