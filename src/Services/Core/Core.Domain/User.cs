using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Core.Domain
{
    public class User
    {
        public long TenantId { get; set; }

        public long UserId { get; set; }
        public string Alias { get; set; }

        public long? ImageBlobId { get; set; }
        public long? PreviewImageBlobId { get; set; }
        public long? ThumbnailImageBlobId { get; set; }
    }
}
