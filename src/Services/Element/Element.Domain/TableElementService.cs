using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Utilities.Text.Csv;

namespace Riverside.Cms.Services.Element.Domain
{
    public class TableElementSettings : ElementSettings
    {
        public string DisplayName { get; set; }
        public string Preamble { get; set; }
        public bool ShowHeaders { get; set; }
        public string Rows { get; set; }
    }

    public class TableElementContent
    {
        public IEnumerable<string> Headers { get; set; }
        public IEnumerable<IEnumerable<string>> Rows { get; set; }
    }

    public interface ITableElementService : IElementSettingsService<TableElementSettings>, IElementViewService<TableElementSettings, TableElementContent>
    {
    }

    public class TableElementService : ITableElementService
    {
        private readonly ICsvService _csvService;
        private readonly IElementRepository<TableElementSettings> _elementRepository;

        public TableElementService(ICsvService csvService, IElementRepository<TableElementSettings> elementRepository)
        {
            _csvService = csvService;
            _elementRepository = elementRepository;
        }

        public Task<TableElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        private TableElementContent GetElementContent(bool showHeaders, string rows)
        {
            IEnumerable<string> contentHeaders = null;
            List<IEnumerable<string>> contentRows = new List<IEnumerable<string>>();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(rows)))
            {
                using (StreamReader sr = new StreamReader(ms))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        IEnumerable<string> values = _csvService.GetCsvValues(line);
                        if (showHeaders && contentHeaders == null)
                            contentHeaders = values;
                        else
                            contentRows.Add(values);
                    }
                }
            }
            return new TableElementContent
            {
                Headers = contentHeaders,
                Rows = contentRows
            };
        }

        public async Task<IElementView<TableElementSettings, TableElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            TableElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            TableElementContent content = GetElementContent(settings.ShowHeaders, settings.Rows);

            return new ElementView<TableElementSettings, TableElementContent>
            {
                Settings = settings,
                Content = content
            };
        }
    }
}
