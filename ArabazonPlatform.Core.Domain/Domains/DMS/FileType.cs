

namespace ArabazonPlatform.Domain.Domains.DMS
{
    public class FileType : SharedBase
    {
        public int FileTypeId { get; set; }
        public string Extension { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }

        public List<File> Files { get; set; }
    }
}
