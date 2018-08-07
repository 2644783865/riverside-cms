﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Storage.Client
{
    public class BlobModel
    {
        public long TenantId { get; set; }
        public long BlobId { get; set; }
        public int Size { get; set; }
        public BlobType BlobType { get; set; }
        public string ContentType { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
}
