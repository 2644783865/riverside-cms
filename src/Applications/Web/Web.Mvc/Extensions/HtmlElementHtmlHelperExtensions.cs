using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using Riverside.Cms.Services.Element.Client;
using Riverside.Cms.Utilities.Text.Formatting;

namespace Riverside.Cms.Applications.Web.Mvc.Extensions
{
    public static class HtmlElementHtmlHelperExtensions
    {
        private class HtmlElementHelper
        {
            private readonly IElementView<HtmlElementSettings, HtmlElementContent> _view;
            private readonly IUrlHelper _urlHelper;

            public HtmlElementHelper(IElementView<HtmlElementSettings, HtmlElementContent> view, IUrlHelper urlHelper)
            {
                _view = view;
                _urlHelper = urlHelper;
            }

            public string Replace(string text)
            {
                string json = text.Substring(2, text.Length - 4);
                HtmlPreviewImageOverride previewImageOverride = JsonConvert.DeserializeObject<HtmlPreviewImageOverride>(json);
                if (!_view.Content.Images.ContainsKey(previewImageOverride.BlobSetId))
                    return string.Empty;
                HtmlPreviewImage previewImage = _view.Content.Images[previewImageOverride.BlobSetId];

                string width = WebUtility.HtmlEncode(previewImageOverride.Width ?? previewImage.Width.ToString());
                string height = WebUtility.HtmlEncode(previewImageOverride.Height ?? previewImage.Height.ToString());
                string alt = WebUtility.HtmlEncode(previewImageOverride.Name ?? previewImage.Name);
                string src =_urlHelper.RouteUrl(RouteNames.ElementBlobContent, new { elementTypeId = _view.Settings.ElementTypeId, elementId = _view.Settings.ElementId, blobSetId = previewImage.BlobSetId });

                return $"<img src=\"{src}\" width=\"{width}\" height=\"{height}\" alt=\"{alt}\" />";
            }
        }

        public static IHtmlContent FormatHtmlElementHtml(this IHtmlHelper htmlHelper, IElementView<HtmlElementSettings, HtmlElementContent> view)
        {
            if (string.IsNullOrWhiteSpace(view.Content.FormattedHtml))
                return new HtmlString(string.Empty);

            IUrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext);
            HtmlElementHelper helper = new HtmlElementHelper(view, urlHelper);
            IStringUtilities utilitites = new StringUtilities();
            string html = utilitites.BlockReplace(view.Content.FormattedHtml, "[[{", "}]]", helper.Replace);

            return new HtmlString(html);
        }
    }
}
