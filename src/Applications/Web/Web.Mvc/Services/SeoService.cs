using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Applications.Web.Mvc.Services
{
    public class SeoService : ISeoService
    {
        public string GetRobotsExclusionStandard()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Disallow: /css/");
            sb.AppendLine("Disallow: /fonts/");
            sb.AppendLine("Disallow: /js/");
            sb.AppendLine("Disallow: /webs/");
            return sb.ToString();
        }
    }
}
