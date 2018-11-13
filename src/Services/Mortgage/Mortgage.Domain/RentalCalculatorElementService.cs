using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Element.Domain;

namespace Riverside.Cms.Services.Mortgage.Domain
{
    public class RentalCalculatorElementSettings : ElementSettings
    {
    }

    public interface IRentalCalculatorElementService : ICalculatorElementService<RentalCalculatorElementSettings>
    {
    }

    public class RentalCalculatorElementService : CalculatorElementService<RentalCalculatorElementSettings>, IRentalCalculatorElementService
    {
        public RentalCalculatorElementService(IElementRepository<RentalCalculatorElementSettings> elementRepository) : base(elementRepository)
        {
        }
    }
}
