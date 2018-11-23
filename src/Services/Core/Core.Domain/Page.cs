using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Core.Domain
{
    public class Page
    {
        public long TenantId { get; set; }

        public long PageId { get; set; }
        public long? ParentPageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime? Occurred { get; set; }

        public long MasterPageId { get; set; }

        public long? ImageBlobId { get; set; }
        public long? PreviewImageBlobId { get; set; }
        public long? ThumbnailImageBlobId { get; set; }

        public IEnumerable<Tag> Tags { get; set; }
    }
}
