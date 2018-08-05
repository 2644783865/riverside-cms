using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Core.Client
{
    public class PageContext
    {
        public long PageId { get; set; }
        public IEnumerable<long> TagIds { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
    }
}
