using System;
using System.Collections.Generic;
using System.Text;
using Riverside.Cms.Utilities.Validation.Framework;

namespace Riverside.Cms.Services.Auth.Domain
{
    public class UserLockedOutException : ValidationErrorException
    {
        public UserLockedOutException() { }
        public UserLockedOutException(string message) : base(message) { }
        public UserLockedOutException(string message, Exception inner) : base(message, inner) { }
        public UserLockedOutException(string message, List<ValidationError> errors) : base(message, errors) { }
        public UserLockedOutException(string message, ValidationError error) : base(message, error) { }
        public UserLockedOutException(string message, List<ValidationError> errors, Exception inner) : base(message, errors, inner) { }
        public UserLockedOutException(string message, ValidationError error, Exception inner) : base(message, error, inner) { }
        public UserLockedOutException(ValidationError error) : base(error) { }
        public UserLockedOutException(List<ValidationError> errors) : base(errors) { }
    }
}
