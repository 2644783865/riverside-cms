using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public class WebService : IWebService
    {
        private readonly IWebRepository _webRepository;
        private readonly IWebValidator _webValidator;

        public WebService(IWebRepository webRepository, IWebValidator webValidator)
        {
            _webRepository = webRepository;
            _webValidator = webValidator;
        }

        public Task<Web> ReadWebAsync(long tenantId)
        {
            return _webRepository.ReadWebAsync(tenantId);
        }

        public Task UpdateWebAsync(long tenantId, Web web)
        {
            // Ensure properties supplied are in the correct format
            web.Name = (web.Name ?? string.Empty).Trim();
            web.GoogleSiteVerification = (web.GoogleSiteVerification ?? string.Empty).Trim();
            web.HeadScript = (web.HeadScript ?? string.Empty).Trim();

            // Perform validation (including checking that all or none of the image upload properties are specified)
            _webValidator.ValidateUpdateWeb(tenantId, web);

            // Do the update
            return _webRepository.UpdateWebAsync(tenantId, web);
        }
    }
}
