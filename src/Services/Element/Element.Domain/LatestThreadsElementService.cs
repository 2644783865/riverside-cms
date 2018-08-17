﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;

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
        public long TenantId { get; set; }
        public long ForumId { get; set; }
        public long ThreadId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool Notify { get; set; }
        public int Views { get; set; }
        public int Replies { get; set; }
        public long? LastPostId { get; set; }
        public DateTime LastPostCreated { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public LatestThreadsUser CreatedByUser { get; set; }
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
        private readonly IUserService _userService;

        public LatestThreadsElementService(IElementRepository<LatestThreadsElementSettings> elementRepository, IForumService forumService, IUserService userService)
        {
            _elementRepository = elementRepository;
            _forumService = forumService;
            _userService = userService;
        }

        public Task<LatestThreadsElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        private LatestThreadsUser GetUser(long? userId, IDictionary<long, User> usersById)
        {
            if (!userId.HasValue)
                return null;
            User user = usersById[userId.Value];
            return new LatestThreadsUser
            {
                Alias = user.Alias,
                UserId = user.UserId,
                Image = null
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

            // _storageService.ListBlobs - get by list of IDs returned from list users

            LatestThreadsElementContent content = new LatestThreadsElementContent
            {
                Threads = threads.Select(t => new LatestThreadsThread
                {
                    Created = t.Created,
                    CreatedByUser = GetUser(t.UserId, usersById),
                    ForumId = t.ForumId,
                    LastPostCreated = t.LastPostCreated,
                    LastPostId = t.LastPostId,
                    Message = t.Message,
                    Notify = t.Notify,
                    PageId = t.PageId,
                    Replies = t.Replies,
                    Subject = t.Subject,
                    TenantId = tenantId,
                    ThreadId = t.ThreadId,
                    Updated = t.Updated,
                    LastPostByUser = GetUser(t.LastPostUserId, usersById),
                    Views = t.Views
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
