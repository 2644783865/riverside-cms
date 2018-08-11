using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Core.Domain
{
    public class TagCount
    {
        public int Count { get; set; }
        public long TagId { get; set; }
        public string Name { get; set; }
    }
}
