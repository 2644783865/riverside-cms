using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Domain;
using Riverside.Cms.Services.Storage.Domain;

namespace Riverside.Cms.Services.Element.Domain
{
    public class LatestThreadsElementSettings : ElementSettings
    {
        public long? PageId { get; set; }
        public string DisplayName { get; set; }
        public string NoThreadsMessage { get; set; }
        public string Preamble { get; set; }
        public int PageSize { get; set; }
        public bool Recursive { get; set; }
    }

    public class LatestThreadsImage
    {
        public long BlobId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class LatestThreadsUser
    {
        public long UserId { get; set; }
        public string Alias { get; set; }
        public LatestThreadsImage Image { get; set; }
    }

    public class LatestThreadsThread
    {
        public long PageId { get; set; }
        public long ForumId { get; set; }
        public long ThreadId { get; set; }
        public string Subject { get; set; }
        public int Replies { get; set; }
        public long? LastPostId { get; set; }
        public LatestThreadsUser StartedByUser { get; set; }
        public LatestThreadsUser LastPostByUser { get; set; }
    }

    public class LatestThreadsElementContent
    {
        public IEnumerable<LatestThreadsThread> Threads { get; set; }
    }

    public interface ILatestThreadsElementService : IElementSettingsService<LatestThreadsElementSettings>, IElementViewService<LatestThreadsElementSettings, LatestThreadsElementContent>
    {
    }

    public class LatestThreadsElementService : ILatestThreadsElementService
    {
        private readonly IElementRepository<LatestThreadsElementSettings> _elementRepository;
        private readonly IForumService _forumService;
        private readonly IStorageService _storageService;
        private readonly IUserService _userService;

        public LatestThreadsElementService(IElementRepository<LatestThreadsElementSettings> elementRepository, IForumService forumService, IStorageService storageService, IUserService userService)
        {
            _elementRepository = elementRepository;
            _forumService = forumService;
            _storageService = storageService;
            _userService = userService;
        }

        public Task<LatestThreadsElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        private LatestThreadsImage GetImage(long? blobId, IDictionary<long, BlobImage> blobsById)
        {
            if (!blobId.HasValue)
                return null;
            BlobImage blobImage = blobsById[blobId.Value];
            return new LatestThreadsImage
            {
                BlobId = blobImage.BlobId,
                Height = blobImage.Height,
                Width = blobImage.Width
            };
        }

        private LatestThreadsUser GetUser(long? userId, IDictionary<long, User> usersById, IDictionary<long, BlobImage> blobsById)
        {
            if (!userId.HasValue)
                return null;
            User user = usersById[userId.Value];
            return new LatestThreadsUser
            {
                Alias = user.Alias,
                UserId = user.UserId,
                Image = GetImage(user.ThumbnailImageBlobId, blobsById)
            };
        }

        public async Task<IElementView<LatestThreadsElementSettings, LatestThreadsElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            LatestThreadsElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);

            long latestThreadsPageId = settings.PageId ?? context.PageId;

            IEnumerable<ForumThread> threads = await _forumService.ListLatestThreadsAsync(tenantId, latestThreadsPageId, settings.Recursive, context.TagIds, settings.PageSize);

            IEnumerable<long> userIds = Enumerable.Concat<long>(threads.Select(t => t.UserId), threads.Where(t => t.LastPostUserId.HasValue).Select(t => (long)t.LastPostUserId)).Distinct();
            IEnumerable<User> users = await _userService.ListUsersAsync(tenantId, userIds);
            IDictionary<long, User> usersById = users.ToDictionary(u => u.UserId, u => u);

            IEnumerable<long> blobIds = users.Where(u => u.ThumbnailImageBlobId.HasValue).Select(u => (long)u.ThumbnailImageBlobId);
            IEnumerable<Blob> blobs = await _storageService.ListBlobsAsync(tenantId, blobIds);
            IDictionary<long, BlobImage> blobsById = blobs.ToDictionary(b => b.BlobId, b => (BlobImage)b);

            LatestThreadsElementContent content = new LatestThreadsElementContent
            {
                Threads = threads.Select(t => new LatestThreadsThread
                {
                    ForumId = t.ForumId,
                    LastPostId = t.LastPostId,
                    PageId = t.PageId,
                    Replies = t.Replies,
                    Subject = t.Subject,
                    StartedByUser = GetUser(t.UserId, usersById, blobsById),
                    ThreadId = t.ThreadId,
                    LastPostByUser = GetUser(t.LastPostUserId, usersById, blobsById)
                })
            };

            return new ElementView<LatestThreadsElementSettings, LatestThreadsElementContent>
            {
                Settings = settings,
                Content = content
            };
        }
    }
}
