using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Riverside.Cms.Applications.Web.Mvc.Extensions
{
    public static class TextHtmlHelperExtensions
    {
        public static IHtmlContent FormatMultiline(this IHtmlHelper htmlHelper, string text)
        {
            string html = text.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "<br />");
            return new HtmlString(html);
        }
    }
}
