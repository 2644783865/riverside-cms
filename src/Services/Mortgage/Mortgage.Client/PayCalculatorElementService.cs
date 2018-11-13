using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Element.Client;

namespace Riverside.Cms.Services.Mortgage.Client
{
    public class PayCalculatorElementSettings : ElementSettings
    {
    }

    public interface IPayCalculatorElementService : ICalculatorElementService<PayCalculatorElementSettings>
    {
    }

    public class PayCalculatorElementService : CalculatorElementService<PayCalculatorElementSettings>, IPayCalculatorElementService
    {
        public PayCalculatorElementService(IOptions<MortgageApiOptions> options) : base(options)
        {
        }

        public override string ElementTypeId => "f03aa333-4a52-4716-aa1b-1d0d6a31dc15";
    }
}
