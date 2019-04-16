using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IWebService
    {
        Task<Web> ReadWebAsync(long tenantId);
        Task UpdateWebAsync(long tenantId, Web web);
    }
}
