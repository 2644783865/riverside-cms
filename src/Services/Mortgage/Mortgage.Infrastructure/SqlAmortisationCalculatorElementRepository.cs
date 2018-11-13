using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Element.Domain;
using Riverside.Cms.Services.Mortgage.Domain;

namespace Riverside.Cms.Services.Mortgage.Infrastructure
{
    public class SqlAmortisationCalculatorElementRepository : SqlCalculatorElementRepository<AmortisationCalculatorElementSettings>
    {
        public SqlAmortisationCalculatorElementRepository(IOptions<SqlOptions> options) : base(options)
        {
        }
    }
}
