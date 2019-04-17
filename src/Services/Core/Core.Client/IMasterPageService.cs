using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Client
{
    public interface IMasterPageService
    {
        Task<MasterPage> ReadMasterPageAsync(long tenantId, long masterPageId);
    }
}
