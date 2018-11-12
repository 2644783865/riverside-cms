using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Applications.Web.Mvc.Extensions;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Applications.Web.Mvc.Services
{
    public class SeoService : ISeoService
    {
        private readonly IPageService _pageService;

        public SeoService(IPageService pageService)
        {
            _pageService = pageService;
        }

        private string GetPageUrl(string rootUrl, Page page)
        {
            if (page.ParentPageId == null)
                return rootUrl;
            return $"{rootUrl}/pages/{page.PageId}/{UrlUtils.UrlFriendly(page.Name)}";
        }

        private string GetPageLastModified(Page page)
        {
            // Credit: https://stackoverflow.com/a/5677405 (but with fractional seconds removed)
            return page.Updated.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }

        public Task<string> GetRobotsExclusionStandardAsync(string rootUrl)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Disallow: /css/");
            sb.AppendLine("Disallow: /fonts/");
            sb.AppendLine("Disallow: /js/");
            sb.AppendLine("Disallow: /webs/");
            sb.AppendLine();
            sb.Append($"Sitemap: {rootUrl}/sitemap.xml");
            return Task.FromResult(sb.ToString());
        }

        public async Task<string> GetSitemapAsync(long tenantId, string rootUrl)
        {
            // Get all pages for inclusion in the sitemap. TODO: This code will need to be re-written to support multiple sitemaps (required for large sites)
            const int maxPageSize = 5000;
            PageListResult homeResult = await _pageService.ListPagesAsync(tenantId, null, false, PageType.Folder, null, SortBy.Created, false, 0, 1);
            PageListResult folderResult = await _pageService.ListPagesAsync(tenantId, null, true, PageType.Folder, null, SortBy.Created, false, 0, maxPageSize);
            PageListResult documentResult = await _pageService.ListPagesAsync(tenantId, null, true, PageType.Document, null, SortBy.Created, false, 0, maxPageSize);
            IEnumerable<Page> pages = Enumerable.Concat(homeResult.Pages, Enumerable.Concat(folderResult.Pages, documentResult.Pages));

            // Construct sitemap XML
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
            foreach (Page page in pages)
            {
                sb.AppendLine("<url>");
                sb.AppendLine($"<loc>{GetPageUrl(rootUrl, page)}</loc>");
                sb.AppendLine($"<lastmod>{GetPageLastModified(page)}</lastmod>");
                sb.AppendLine("</url>");
            }
            sb.Append("</urlset>");
            return sb.ToString();
        }
    }
}
