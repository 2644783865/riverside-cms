﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IUserService
    {
        Task<IEnumerable<User>> ListUsersAsync(long tenantId, IEnumerable<long> userIds);
    }
}
