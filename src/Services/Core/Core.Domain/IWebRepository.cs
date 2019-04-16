using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IWebRepository
    {
        Task<Web> ReadWebAsync(long tenantId);
        Task UpdateWebAsync(long tenantId, Web web);
    }
}
