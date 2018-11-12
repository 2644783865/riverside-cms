using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Riverside.Cms.Applications.Web.Mvc.Services
{
    public interface ISeoService
    {
        Task<string> GetRobotsExclusionStandardAsync(string rootUrl);
        Task<string> GetSitemapAsync(long tenantId, string rootUrl);
    }
}
