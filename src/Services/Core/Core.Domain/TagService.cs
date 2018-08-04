﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public Task<Tag> ReadTagAsync(long tenantId, long tagId)
        {
            return _tagRepository.ReadTagAsync(tenantId, tagId);
        }

        public Task<IEnumerable<Tag>> ListTagsAsync(long tenantId, IEnumerable<long> tagIds)
        {
            return _tagRepository.ListTagsAsync(tenantId, tagIds);
        }
    }
}
