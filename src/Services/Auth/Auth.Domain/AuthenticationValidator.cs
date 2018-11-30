using System;
using System.Threading.Tasks;
using Riverside.Cms.Utilities.Security.Encryption;
using Riverside.Cms.Utilities.Validation.DataAnnotations;
using Riverside.Cms.Utilities.Validation.Framework;

namespace Riverside.Cms.Services.Auth.Domain
{
    public class AuthenticationValidator : IAuthenticationValidator
    {
        private readonly IAuthenticationConfigurationService _authenticationConfigurationService;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IModelValidator _modelValidator;

        public AuthenticationValidator(IAuthenticationConfigurationService authenticationConfigurationService, IAuthenticationRepository authenticationRepository, IEncryptionService encryptionService, IModelValidator modelValidator)
        {
            _authenticationConfigurationService = authenticationConfigurationService;
            _authenticationRepository = authenticationRepository;
            _encryptionService = encryptionService;
            _modelValidator = modelValidator;
        }

        /// <summary>
        /// Unlock a locked out account.
        /// </summary>
        private async Task UnlockAsync(long tenantId, string email, AuthenticationState state)
        {
            // Clear password failures associated with a user and sets user's locked out flag to false
            state.LockedOut = false;
            state.LastPasswordFailure = null;
            state.PasswordFailures = 0;

            // Do the update
            await _authenticationRepository.UpdateAuthenticationStateAsync(tenantId, email, state);
        }

        /// <summary>
        /// Registers a password failure against specified user account. If password failure count exceeds "password failures before lock out", then user 
        /// has entered their password incorrectly too many times and will be locked out.
        /// </summary>
        private async Task RegisterPasswordFailureAsync(long tenantId, string email, AuthenticationState state)
        {
            // Get password failure before lockout count
            int passwordFailuresBeforeLockOut = _authenticationConfigurationService.GetPasswordFailuresBeforeLockOut(tenantId);

            // Increment password failures
            state.LastPasswordFailure = DateTime.UtcNow;
            state.PasswordFailures = state.PasswordFailures + 1;
            state.LockedOut = state.PasswordFailures > passwordFailuresBeforeLockOut;

            // Update user
            await _authenticationRepository.UpdateAuthenticationStateAsync(tenantId, email, state);
        }

        public async Task ValidateLogonAsync(long tenantId, LogonModel model)
        {
            // Do initial model checks (valid email, password required etc)
            _modelValidator.Validate(model);

            // Get user given email address
            string email = model.Email.Trim();
            AuthenticationState state = await _authenticationRepository.ReadAuthenticationStateAsync(tenantId, email);

            // The first condition that causes an invalid user is when user not found due to an invalid email address entered. In this case, state is null.
            if (state == null)
                throw new ValidationErrorException(new ValidationError(null, AuthenticationResource.LogonUserCredentialsInvalidMessage));

            // If user has not been confirmed, they must first set their password before they can be validated
            if (!state.Confirmed)
                throw new ValidationErrorException(new ValidationError(null, AuthenticationResource.LogonUserUnconfirmedMessage));

            // User may have been disabled by an administrator, in which case it will not be possible to validate account
            if (!state.Enabled)
                throw new ValidationErrorException(new ValidationError(null, AuthenticationResource.LogonUserDisabledMessage));

            // If account locked out, check to see when last password failure occured. If lockout duration period expired, undo the lockout.
            if (state.LockedOut)
            {
                TimeSpan lockOutDuration = _authenticationConfigurationService.GetLockOutDuration(tenantId);
                if ((DateTime.UtcNow - (DateTime)state.LastPasswordFailure) > lockOutDuration)
                    await UnlockAsync(tenantId, email, state);
                else
                    throw new UserLockedOutException(new ValidationError(null, AuthenticationResource.LogonUserLockedOutMessage));
            }

            // Finally, check password entered is correct and if not register a password failure which may lock user out
            byte[] userPasswordSalt = _encryptionService.GetBytes(state.PasswordSalt);
            byte[] userPasswordSaltedHash = _encryptionService.GetBytes(state.PasswordSaltedHash);
            byte[] logonPasswordSaltedHash = _encryptionService.EncryptPassword(model.Password, userPasswordSalt);
            if (!_encryptionService.ByteArraysEqual(logonPasswordSaltedHash, userPasswordSaltedHash))
            {
                await RegisterPasswordFailureAsync(tenantId, email, state);
                if (state.LockedOut)
                    throw new ValidationErrorException(new ValidationError(null, AuthenticationResource.LogonUserLockedOutMessage));
                else
                    throw new ValidationErrorException(new ValidationError(null, AuthenticationResource.LogonUserCredentialsInvalidMessage));
            }
        }
    }
}
