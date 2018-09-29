using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Element.Domain
{
    public class PageContext : IPageContext
    {
        public long PageId { get; set; }
        public IEnumerable<long> TagIds { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
    }
}
