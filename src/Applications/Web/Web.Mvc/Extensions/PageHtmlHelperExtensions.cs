using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Applications.Web.Mvc.Extensions
{
    public static class PageHtmlHelperExtensions
    {
        public static IHtmlContent FormatRaw(this IHtmlHelper htmlHelper, PageView pageView, string html)
        {
            // Nothing to do if page does not have a preview image
            if (!pageView.PreviewImageBlobId.HasValue)
                return new HtmlString(html);

            // Nothing to do if HTML does not contain the jumbotron outer class
            if (html == null || !html.Contains("class=\"jumbotron-outer\""))
                return new HtmlString(html);

            // Update HTML so page preview image is background of div with jumbotron outer class
            IUrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext);
            string previewImageUrl = urlHelper.RouteUrl(RouteNames.PageImage, new { pageid = pageView.PageId, pageImageType = "preview", description = UrlUtils.UrlFriendly(pageView.Title), t = pageView.PreviewImageBlobId.Value });
            string replaceText = string.Format("class=\"jumbotron-outer\" style=\"background-image: url({0});\"", previewImageUrl);
            html = html.Replace("class=\"jumbotron-outer\"", replaceText);
            return new HtmlString(html);
        }
    }
}
