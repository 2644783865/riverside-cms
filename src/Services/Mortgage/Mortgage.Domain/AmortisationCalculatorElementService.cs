using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Element.Domain;

namespace Riverside.Cms.Services.Mortgage.Domain
{
    public class AmortisationCalculatorElementSettings : ElementSettings
    {
    }

    public interface IAmortisationCalculatorElementService : ICalculatorElementService<AmortisationCalculatorElementSettings>
    {
    }

    public class AmortisationCalculatorElementService : CalculatorElementService<AmortisationCalculatorElementSettings>, IAmortisationCalculatorElementService
    {
        public AmortisationCalculatorElementService(IElementRepository<AmortisationCalculatorElementSettings> elementRepository) : base(elementRepository)
        {
        }
    }
}
