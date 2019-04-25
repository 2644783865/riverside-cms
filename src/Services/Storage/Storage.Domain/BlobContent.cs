using System.IO;

namespace Riverside.Cms.Services.Storage.Domain
{
    public class BlobContent : IBlobContent
    {
        public Stream Stream { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
