using Riverside.Cms.Utilities.Validation.DataAnnotations;
using Riverside.Cms.Utilities.Validation.Framework;

namespace Riverside.Cms.Services.Core.Domain
{
    public class PageValidator : IPageValidator
    {
        private readonly IModelValidator _modelValidator;

        public PageValidator(IModelValidator modelValidator)
        {
            _modelValidator = modelValidator;
        }

        public void ValidateUpdatePage(long tenantId, long pageId, Page page)
        {
            // Do general model validation
            _modelValidator.Validate(page);

            // Check that either all image values are specified or that none are specified
            bool allHaveValue = page.ImageBlobId.HasValue && page.PreviewImageBlobId.HasValue && page.ThumbnailImageBlobId.HasValue;
            bool noneHaveValue = !page.ImageBlobId.HasValue && !page.PreviewImageBlobId.HasValue && !page.ThumbnailImageBlobId.HasValue;
            if (!(allHaveValue || noneHaveValue))
                throw new ValidationErrorException(new ValidationError(PagePropertyNames.ImageBlobId, PageResource.ImageInvalidMessage));
        }
    }
}
