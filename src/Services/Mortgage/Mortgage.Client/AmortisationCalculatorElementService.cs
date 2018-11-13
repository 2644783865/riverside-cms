using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Riverside.Cms.Services.Element.Client;

namespace Riverside.Cms.Services.Mortgage.Client
{
    public class AmortisationCalculatorElementSettings : ElementSettings
    {
    }

    public interface IAmortisationCalculatorElementService : ICalculatorElementService<AmortisationCalculatorElementSettings>
    {
    }

    public class AmortisationCalculatorElementService : CalculatorElementService<AmortisationCalculatorElementSettings>, IAmortisationCalculatorElementService
    {
        public AmortisationCalculatorElementService(IOptions<MortgageApiOptions> options) : base(options)
        {
        }

        public override string ElementTypeId => "fb7e757d-905c-4ab1-828f-c39baabe55a6";
    }
}
