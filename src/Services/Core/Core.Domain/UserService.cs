using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<IEnumerable<User>> ListUsersAsync(long tenantId, IEnumerable<long> userIds)
        {
            return _userRepository.ListUsersAsync(tenantId, userIds);
        }
    }
}
