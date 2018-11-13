using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Element.Client;

namespace Riverside.Cms.Services.Mortgage.Client
{
    public class RentalCalculatorElementSettings : ElementSettings
    {
    }

    public interface IRentalCalculatorElementService : ICalculatorElementService<RentalCalculatorElementSettings>
    {
    }

    public class RentalCalculatorElementService : CalculatorElementService<RentalCalculatorElementSettings>, IRentalCalculatorElementService
    {
        public RentalCalculatorElementService(IOptions<MortgageApiOptions> options) : base(options)
        {
        }

        public override string ElementTypeId => "eec21fac-d185-45ee-a8dd-6032679697b1";
    }
}
