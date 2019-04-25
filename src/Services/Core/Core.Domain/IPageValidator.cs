using System.Threading.Tasks;
using Riverside.Cms.Services.Storage.Domain;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IPageValidator
    {
        Task ValidateCreatePageImagesAsync(long tenantId, long pageId, IBlobContent content);
        void ValidateUpdatePage(long tenantId, long pageId, Page page);
    }
}
