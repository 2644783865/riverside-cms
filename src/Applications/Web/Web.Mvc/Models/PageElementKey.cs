using System;

namespace Riverside.Cms.Applications.Web.Mvc.Models
{
    public class PageElementKey
    {
        public long PageId { get; set; }
        public Guid ElementTypeId { get; set; }
        public long ElementId { get; set; }
    }
}
