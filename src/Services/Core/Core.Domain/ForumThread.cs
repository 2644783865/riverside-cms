using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Core.Domain
{
    public class ForumThread
    {
        public long PageId { get; set; }
        public long TenantId { get; set; }
        public long ForumId { get; set; }
        public long ThreadId { get; set; }
        public long UserId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool Notify { get; set; }
        public int Views { get; set; }
        public int Replies { get; set; }
        public long? LastPostId { get; set; }
        public DateTime LastPostCreated { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public long? LastPostUserId { get; set; }
    }
}