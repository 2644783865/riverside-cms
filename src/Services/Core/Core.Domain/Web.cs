using System.ComponentModel.DataAnnotations;

namespace Riverside.Cms.Services.Core.Domain
{
    public class Web
    {
        public long TenantId { get; set; }

        [Required(ErrorMessageResourceType = typeof(WebResource), ErrorMessageResourceName = "NameRequiredMessage")]
        [StringLength(256, MinimumLength = 1, ErrorMessageResourceType = typeof(WebResource), ErrorMessageResourceName = "NameLengthMessage")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(WebResource), ErrorMessageResourceName = "GoogleSiteVerificationLengthMessage")]
        public string GoogleSiteVerification { get; set; }
    }
}
