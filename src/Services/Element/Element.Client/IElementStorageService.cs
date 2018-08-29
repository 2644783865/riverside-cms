﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Services.Element.Client
{
    public interface IElementStorageService
    {
        Task<BlobContent> ReadBlobContentAsync(long tenantId, long elementId, long elementBlobId, PageImageType imageType);
    }
}
