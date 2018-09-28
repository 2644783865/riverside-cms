using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public class WebService : IWebService
    {
        private readonly IWebRepository _webRepository;

        public WebService(IWebRepository webRepository)
        {
            _webRepository = webRepository;
        }

        public Task<Web> ReadWebAsync(long tenantId)
        {
            return _webRepository.ReadWebAsync(tenantId);
        }
    }
}
