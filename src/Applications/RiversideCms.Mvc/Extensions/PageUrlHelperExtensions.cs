using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

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

        public static string PageUrl(this IUrlHelper urlHelper, long pageId, string pageName, bool home, object values)
        {
            if (home)
            {
                return urlHelper.RouteUrl("Home", values);
            }
            else
            {
                RouteValueDictionary newValues = new RouteValueDictionary(values);
                newValues.Add("pageid", pageId);
                newValues.Add("description", UrlUtils.UrlFriendly(pageName));
                return urlHelper.RouteUrl("Page", newValues);
            }
        }

        public static string PageTaggedUrl(this IUrlHelper urlHelper, long pageId, string pageName, bool home, IEnumerable<string> tagNames, object values)
        {
            RouteValueDictionary newValues = new RouteValueDictionary(values);
            if (tagNames != null)
            {
                string tags = string.Join('+', tagNames);
                newValues.Add("tags", tags);
            }

            string url = null;
            if (home)
            {
                url = urlHelper.RouteUrl("HomeTagged", newValues);
            }
            else
            {
                newValues.Add("pageid", pageId);
                newValues.Add("description", UrlUtils.UrlFriendly(pageName));
                url = urlHelper.RouteUrl("PageTagged", newValues);
            }

            return url.Replace("%2B", "+");
        }
    }
}
