using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Element.Domain;

namespace Riverside.Cms.Services.Mortgage.Domain
{
    public class PayCalculatorElementSettings : ElementSettings
    {
    }

    public interface IPayCalculatorElementService : ICalculatorElementService<PayCalculatorElementSettings>
    {
    }

    public class PayCalculatorElementService : CalculatorElementService<PayCalculatorElementSettings>, IPayCalculatorElementService
    {
        public PayCalculatorElementService(IElementRepository<PayCalculatorElementSettings> elementRepository) : base(elementRepository)
        {
        }
    }
}
