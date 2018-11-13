using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Element.Domain;

namespace Riverside.Cms.Services.Mortgage.Domain
{
    public class StampDutyCalculatorElementSettings : ElementSettings
    {
    }

    public interface IStampDutyCalculatorElementService : ICalculatorElementService<StampDutyCalculatorElementSettings>
    {
    }

    public class StampDutyCalculatorElementService : CalculatorElementService<StampDutyCalculatorElementSettings>, IStampDutyCalculatorElementService
    {
        public StampDutyCalculatorElementService(IElementRepository<StampDutyCalculatorElementSettings> elementRepository) : base(elementRepository)
        {
        }
    }
}
