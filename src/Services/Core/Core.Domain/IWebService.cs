using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IWebService
    {
        Task<Web> ReadWebAsync(long tenantId);
    }
}
