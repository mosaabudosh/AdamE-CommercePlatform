

namespace E_Commerce.Domain.Domains.DMS
{
    public class File : SharedBase
    {
        public int FileId { get; set; }
        public string Name { get; set; }
        public int FileTypeId { get; set; }
        public decimal? Size { get; set; }
        public string? SizeType { get; set; }
        public int? FolderId { get; set; }
        public string Path { get; set; }

        public FileType FileType { set; get; }
        public Folder Folder { set; get; }


    }
}
