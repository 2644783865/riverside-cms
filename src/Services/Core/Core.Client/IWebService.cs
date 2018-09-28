using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Client
{
    public interface IWebService
    {
        Task<Web> ReadWebAsync(long tenantId);
    }
}
