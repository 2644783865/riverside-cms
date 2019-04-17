using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Riverside.Cms.Services.Core.Domain
{
    public class Page
    {
        public long TenantId { get; set; }

        public long PageId { get; set; }
        public long? ParentPageId { get; set; }

        [Required(ErrorMessageResourceType = typeof(PageResource), ErrorMessageResourceName = "NameRequiredMessage")]
        [StringLength(256, MinimumLength = 1, ErrorMessageResourceType = typeof(PageResource), ErrorMessageResourceName = "NameLengthMessage")]
        public string Name { get; set; }

        [StringLength(5000, ErrorMessageResourceType = typeof(PageResource), ErrorMessageResourceName = "DescriptionLengthMessage")]
        public string Description { get; set; }

        [StringLength(256, ErrorMessageResourceType = typeof(PageResource), ErrorMessageResourceName = "TitleLengthMessage")]
        public string Title { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime? Occurred { get; set; }

        public long MasterPageId { get; set; }

        public long? ImageBlobId { get; set; }
        public long? PreviewImageBlobId { get; set; }
        public long? ThumbnailImageBlobId { get; set; }

        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<PageZone> PageZones { get; set; }
    }
}
