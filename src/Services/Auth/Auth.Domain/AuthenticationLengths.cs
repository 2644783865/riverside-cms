using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Auth.Domain
{
    public class AuthenticationLengths
    {
        public const int AliasMaxLength = 50;
        public const int EmailMaxLength = 256;
        public const int PasswordMaxLength = 30;
        public const int PasswordMinLength = 6;
    }
}
