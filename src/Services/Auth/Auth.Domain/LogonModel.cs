using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Riverside.Cms.Utilities.Validation.Framework;

namespace Riverside.Cms.Services.Auth.Domain
{
    public class LogonModel
    {
        [Display(ResourceType = typeof(AuthenticationResource), Name = "LogonRememberMeLabel")]
        public bool RememberMe { get; set; }

        [Display(ResourceType = typeof(AuthenticationResource), Name = "LogonEmailLabel")]
        [Required(ErrorMessageResourceType = typeof(AuthenticationResource), ErrorMessageResourceName = "LogonEmailRequiredMessage")]
        [StringLength(StringLengths.EmailMaxLength, ErrorMessageResourceType = typeof(AuthenticationResource), ErrorMessageResourceName = "LogonEmailMaxLengthMessage")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(AuthenticationResource), ErrorMessageResourceName = "LogonEmailInvalidMessage")]
        [RegularExpression(RegularExpressions.Email, ErrorMessageResourceType = typeof(AuthenticationResource), ErrorMessageResourceName = "LogonEmailInvalidMessage")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(AuthenticationResource), Name = "LogonPasswordLabel")]
        [Required(ErrorMessageResourceType = typeof(AuthenticationResource), ErrorMessageResourceName = "LogonPasswordRequiredMessage")]
        public string Password { get; set; }
    }
}
