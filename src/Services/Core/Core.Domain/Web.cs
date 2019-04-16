using System.ComponentModel.DataAnnotations;

namespace Riverside.Cms.Services.Core.Domain
{
    public class Web
    {
        public long TenantId { get; set; }

        [Required(ErrorMessageResourceType = typeof(WebResource), ErrorMessageResourceName = "NameRequiredMessage")]
        [StringLength(256, MinimumLength = 1, ErrorMessageResourceType = typeof(WebResource), ErrorMessageResourceName = "NameLengthMessage")]
        public string Name { get; set; }

        [StringLength(256, ErrorMessageResourceType = typeof(WebResource), ErrorMessageResourceName = "GoogleSiteVerificationLengthMessage")]
        public string GoogleSiteVerification { get; set; }

        [StringLength(5000, ErrorMessageResourceType = typeof(WebResource), ErrorMessageResourceName = "HeadScriptLengthMessage")]
        public string HeadScript { get; set; }
    }
}
