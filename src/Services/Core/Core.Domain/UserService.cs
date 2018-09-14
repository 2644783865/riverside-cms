using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Storage.Domain;

namespace Riverside.Cms.Services.Core.Domain
{
    public class UserService : IUserService
    {
        private readonly IStorageService _storageService;
        private readonly IUserRepository _userRepository;

        private const string UserImagePath = "users/images";

        public UserService(IStorageService storageService, IUserRepository userRepository)
        {
            _storageService = storageService;
            _userRepository = userRepository;
        }

        public Task<User> ReadUserAsync(long tenantId, long userId)
        {
            return _userRepository.ReadUserAsync(tenantId, userId);
        }

        private long? GetBlobId(User user, UserImageType userImageType)
        {
            switch (userImageType)
            {
                case UserImageType.Original:
                    return user.ImageBlobId;

                case UserImageType.Preview:
                    return user.PreviewImageBlobId;

                case UserImageType.Thumbnail:
                    return user.ThumbnailImageBlobId;

                default:
                    return null;
            }
        }

        public async Task<BlobContent> ReadUserImageAsync(long tenantId, long userId, UserImageType userImageType)
        {
            User user = await _userRepository.ReadUserAsync(tenantId, userId);
            if (user == null)
                return null;
            long? blobId = GetBlobId(user, userImageType);
            if (blobId == null)
                return null;
            return await _storageService.ReadBlobContentAsync(tenantId, blobId.Value, UserImagePath);
        }

        public Task<IEnumerable<User>> ListUsersAsync(long tenantId, IEnumerable<long> userIds)
        {
            return _userRepository.ListUsersAsync(tenantId, userIds);
        }
    }
}
