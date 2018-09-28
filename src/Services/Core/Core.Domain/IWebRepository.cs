using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IWebRepository
    {
        Task<Web> ReadWebAsync(long tenantId);
    }
}
