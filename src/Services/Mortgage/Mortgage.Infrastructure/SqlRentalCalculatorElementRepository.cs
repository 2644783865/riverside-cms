using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Mortgage.Domain;

namespace Riverside.Cms.Services.Mortgage.Infrastructure
{
    public class SqlRentalCalculatorElementRepository : SqlCalculatorElementRepository<RentalCalculatorElementSettings>
    {
        public SqlRentalCalculatorElementRepository(IOptions<SqlOptions> options) : base(options)
        {
        }
    }
}
