using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        private const int MaxTagSize = 9;

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

        public Task<IEnumerable<Tag>> ListTagsAsync(long tenantId, IEnumerable<string> tagNames)
        {
            return _tagRepository.ListTagsAsync(tenantId, tagNames);
        }

        private void CalculateTagSizes(IEnumerable<TagCount> tagCounts)
        {
            IEnumerable<KeyValuePair<int, IEnumerable<TagCount>>> tagsByCount = tagCounts
                .GroupBy(t => t.Count)
                .ToDictionary(g => g.Key, g => g.Select(tc => tc))
                .OrderBy(kvp => kvp.Key);

            int distinctCounts = tagsByCount.Count();

            if (distinctCounts > 1)
            {
                int index = 0;
                double sizeStep = (double)MaxTagSize / (double)(distinctCounts - 1);
                foreach (KeyValuePair<int, IEnumerable<TagCount>> kvp in tagsByCount)
                {
                    int size = (int)Math.Round(((double)index) * sizeStep) + 1;
                    foreach (TagCount tagCount in kvp.Value)
                        tagCount.Size = size;
                    index++;
                }
            }
            else if (distinctCounts == 1)
            {
                int size = (int)Math.Round((double)MaxTagSize / 2.0) + 1;
                foreach (TagCount tagCount in tagsByCount.First().Value)
                    tagCount.Size = size;
            }
        }

        public async Task<IEnumerable<TagCount>> ListTagCountsAsync(long tenantId, long? parentPageId, bool recursive)
        {
            IEnumerable<TagCount> tags = await _tagRepository.ListTagCountsAsync(tenantId, parentPageId, recursive);
            CalculateTagSizes(tags);
            return tags;
        }

        public async Task<IEnumerable<TagCount>> ListRelatedTagCountsAsync(long tenantId, IEnumerable<long> tagIds, long? parentPageId, bool recursive)
        {
            IEnumerable<TagCount> tags = await _tagRepository.ListRelatedTagCountsAsync(tenantId, tagIds, parentPageId, recursive);
            CalculateTagSizes(tags);
            return tags;
        }
    }
}
