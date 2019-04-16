using Riverside.Cms.Utilities.Validation.DataAnnotations;

namespace Riverside.Cms.Services.Core.Domain
{
    public class WebValidator : IWebValidator
    {
        private readonly IModelValidator _modelValidator;

        public WebValidator(IModelValidator modelValidator)
        {
            _modelValidator = modelValidator;
        }

        public void ValidateUpdateWeb(long tenantId, Web web)
        {
            // Do general model validation
            _modelValidator.Validate(web);
        }
    }
}
