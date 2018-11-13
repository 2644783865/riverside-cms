using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Element.Client;

namespace Riverside.Cms.Services.Mortgage.Client
{
    public class BorrowCalculatorElementSettings : ElementSettings
    {
    }

    public interface IBorrowCalculatorElementService : ICalculatorElementService<BorrowCalculatorElementSettings>
    {
    }

    public class BorrowCalculatorElementService : CalculatorElementService<BorrowCalculatorElementSettings>, IBorrowCalculatorElementService
    {
        public BorrowCalculatorElementService(IOptions<MortgageApiOptions> options) : base(options)
        {
        }

        public override string ElementTypeId => "8373bdac-6f80-4c5e-8e27-1b3a9c92cd7c";
    }
}
