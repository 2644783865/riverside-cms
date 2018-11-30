using System;
using System.Collections.Generic;
using System.Text;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Auth.Domain
{
    public class AuthenticationSession
    {
        public bool Persist { get; set; }
        public UserIdentity Identity { get; set; }
    }
}
