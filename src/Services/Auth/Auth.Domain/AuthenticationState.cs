using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Auth.Domain
{
    public class AuthenticationState
    {
        /// <summary>
        /// Identifies the website that the user belongs to.
        /// </summary>
        public long TenantId { get; set; }

        /// <summary>
        /// User email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// True if user confirmed, false if not. A value of true indicates user has confirmed their account following creation.
        /// </summary>
        public bool Confirmed { get; set; }

        /// <summary>
        /// Used by administrators to control access to website. Set false to prevent user accessing site.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// True if user locked out due to repeated attempts to access website with the wrong password within a given time span.
        /// </summary>
        public bool LockedOut { get; set; }

        /// <summary>
        /// Stores salted hash of password (or null if password not set).
        /// </summary>
        public string PasswordSaltedHash { get; set; }

        /// <summary>
        /// Stores salt used during password hashing.
        /// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        /// The date and time password last incorrectly entered (or null if password never incorrectly entered).
        /// </summary>
        public DateTime? LastPasswordFailure { get; set; }

        /// <summary>
        /// The number of times password incorrectly entered within a given time span.
        /// </summary>
        public int PasswordFailures { get; set; }
    }
}
