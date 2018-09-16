using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public class DomainService : IDomainService
    {
        private readonly IDomainRepository _domainRepository;

        public DomainService(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public Task<WebDomain> ReadDomainByUrlAsync(string url)
        {
            return _domainRepository.ReadDomainByUrlAsync(url);
        }
    }
}
