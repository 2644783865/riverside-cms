using System.IO;

namespace Riverside.Cms.Services.Storage.Domain
{
    public interface IBlobContent
    {
        Stream Stream { get; set; }
        string Name { get; set; }
        string Type { get; set; }
    }
}
