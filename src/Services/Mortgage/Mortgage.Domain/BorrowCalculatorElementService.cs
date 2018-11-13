using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Element.Domain;

namespace Riverside.Cms.Services.Mortgage.Domain
{
    public class BorrowCalculatorElementSettings : ElementSettings
    {
    }

    public interface IBorrowCalculatorElementService : ICalculatorElementService<BorrowCalculatorElementSettings>
    {
    }

    public class BorrowCalculatorElementService : CalculatorElementService<BorrowCalculatorElementSettings>, IBorrowCalculatorElementService
    {
        public BorrowCalculatorElementService(IElementRepository<BorrowCalculatorElementSettings> elementRepository) : base(elementRepository)
        {
        }
    }
}
