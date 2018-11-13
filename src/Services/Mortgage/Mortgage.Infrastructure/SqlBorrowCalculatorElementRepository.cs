using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Mortgage.Domain;

namespace Riverside.Cms.Services.Mortgage.Infrastructure
{
    public class SqlBorrowCalculatorElementRepository : SqlCalculatorElementRepository<BorrowCalculatorElementSettings>
    {
        public SqlBorrowCalculatorElementRepository(IOptions<SqlOptions> options) : base(options)
        {
        }
    }
}
