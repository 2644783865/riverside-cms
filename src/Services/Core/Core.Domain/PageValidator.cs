using System.IO;
using System.Threading.Tasks;
using Riverside.Cms.Services.Storage.Domain;
using Riverside.Cms.Utilities.Drawing.ImageAnalysis;
using Riverside.Cms.Utilities.Validation.DataAnnotations;
using Riverside.Cms.Utilities.Validation.Framework;

namespace Riverside.Cms.Services.Core.Domain
{
    public class PageValidator : IPageValidator
    {
        private readonly IImageAnalysisService _imageAnalysisService;
        private readonly IMasterPageRepository _masterPageRepository;
        private readonly IModelValidator _modelValidator;
        private readonly IPageRepository _pageRepository;

        public PageValidator(IImageAnalysisService imageAnalysisService, IMasterPageRepository masterPageRepository, IModelValidator modelValidator, IPageRepository pageRepository)
        {
            _imageAnalysisService = imageAnalysisService;
            _masterPageRepository = masterPageRepository;
            _modelValidator = modelValidator;
            _pageRepository = pageRepository;
        }

        public async Task ValidateCreatePageImagesAsync(long tenantId, long pageId, IBlobContent content)
        {
            // Get page details
            Page page = await _pageRepository.ReadPageAsync(tenantId, pageId);

            // Check that master page associated with page allows images
            MasterPage masterPage = await _masterPageRepository.ReadMasterPageAsync(tenantId, page.MasterPageId);
            if (!masterPage.HasImage)
                throw new ValidationErrorException(new ValidationError(PagePropertyNames.ImageBlobId, PageResource.ImageNotAllowedMessage));

            // Check that blob type is correct
            if (content.Type != ContentTypes.Gif && content.Type != ContentTypes.Jpeg && content.Type != ContentTypes.Png)
                throw new ValidationErrorException(new ValidationError(PagePropertyNames.ImageBlobId, PageResource.ImageInvalidMessage));

            // Check that supplied upload is an image
            ImageMetadata metadata = _imageAnalysisService.GetImageMetadata(content.Stream);
            if (metadata == null)
                throw new ValidationErrorException(new ValidationError(PagePropertyNames.ImageBlobId, PageResource.ImageInvalidMessage));

            // Check image dimension constraints (minimum width and height)
            if (metadata.Width < masterPage.ImageMinWidth.Value || metadata.Height < masterPage.ImageMinHeight.Value)
                throw new ValidationErrorException(new ValidationError(PagePropertyNames.ImageBlobId, string.Format(PageResource.ImageDimensionsInvalidMessage, masterPage.ImageMinWidth.Value, masterPage.ImageMinHeight.Value)));
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
