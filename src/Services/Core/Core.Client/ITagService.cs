﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Client
{
    public interface ITagService
    {
        Task<Tag> ReadTagAsync(long tenantId, long tagId);
        Task<IEnumerable<Tag>> ListTagsAsync(long tenantId, IEnumerable<long> tagIds);
        Task<IEnumerable<Tag>> ListTagsAsync(long tenantId, IEnumerable<string> tagNames);
    }
}
