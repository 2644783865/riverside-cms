namespace Riverside.Cms.Services.Core.Domain
{
    public interface IWebValidator
    {
        void ValidateUpdateWeb(long tenantId, Web web);
    }
}
