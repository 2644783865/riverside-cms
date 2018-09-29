using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Element.Domain
{
    public class Testimonial
    {
        public long TestimonialId { get; set; }
        public string Comment { get; set; }
        public string Author { get; set; }
        public string AuthorTitle { get; set; }
        public string Date { get; set; }
    }

    public class TestimonialElementSettings : ElementSettings
    {
        public string DisplayName { get; set; }
        public string Preamble { get; set; }
        public IEnumerable<Testimonial> Testimonials { get; set; }
    }

    public interface ITestimonialElementService : IElementSettingsService<TestimonialElementSettings>, IElementViewService<TestimonialElementSettings, object>
    {
    }

    public class TestimonialElementService : ITestimonialElementService
    {
        private readonly IElementRepository<TestimonialElementSettings> _elementRepository;

        public TestimonialElementService(IElementRepository<TestimonialElementSettings> elementRepository)
        {
            _elementRepository = elementRepository;
        }

        public Task<TestimonialElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<IElementView<TestimonialElementSettings, object>> ReadElementViewAsync(long tenantId, long elementId, IPageContext context)
        {
            TestimonialElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            return new ElementView<TestimonialElementSettings, object>
            {
                Settings = settings,
                Content = null
            };
        }
    }
}
