﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IUserRepository
    {
        Task<User> ReadUserAsync(long tenantId, long userId);
        Task<UserIdentity> ReadUserIdentityAsync(long tenantId, string email);
        Task<IEnumerable<User>> ListUsersAsync(long tenantId, IEnumerable<long> userIds);
    }
}
