using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Mortgage.Domain;

namespace Riverside.Cms.Services.Mortgage.Infrastructure
{
    public class SqlPayCalculatorElementRepository : SqlCalculatorElementRepository<PayCalculatorElementSettings>
    {
        public SqlPayCalculatorElementRepository(IOptions<SqlOptions> options) : base(options)
        {
        }
    }
}
