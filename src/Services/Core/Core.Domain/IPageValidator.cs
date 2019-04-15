namespace Riverside.Cms.Services.Core.Domain
{
    public interface IPageValidator
    {
        void ValidateUpdatePage(long tenantId, long pageId, Page page);
    }
}
