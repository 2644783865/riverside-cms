using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Element.Client;

namespace Riverside.Cms.Services.Mortgage.Client
{
    public class StampDutyCalculatorElementSettings : ElementSettings
    {
    }

    public interface IStampDutyCalculatorElementService : ICalculatorElementService<StampDutyCalculatorElementSettings>
    {
    }

    public class StampDutyCalculatorElementService : CalculatorElementService<StampDutyCalculatorElementSettings>, IStampDutyCalculatorElementService
    {
        public StampDutyCalculatorElementService(IOptions<MortgageApiOptions> options) : base(options)
        {
        }

        public override string ElementTypeId => "9c167ed6-1caf-4b2c-8de1-2586d247e28e";
    }
}
