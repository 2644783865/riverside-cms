using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IMasterPageRepository
    {
        Task<MasterPage> ReadMasterPageAsync(long tenantId, long masterPageId);
    }
}
