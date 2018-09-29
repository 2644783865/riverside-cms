using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Element.Client
{
    public interface IPageContext
    {
        long PageId { get; set; }
        IEnumerable<long> TagIds { get; set; }
        IDictionary<string, string> Parameters { get; set; }
    }
}
