using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Services.Core.Client
{
    public interface IUserService
    {
        Task<User> ReadUserAsync(long tenantId, long userId);
        Task<BlobContent> ReadUserImageAsync(long tenantId, long userId, UserImageType userImageType);
        Task<IEnumerable<User>> ListUsersAsync(long tenantId, IEnumerable<long> userIds);
    }
}
