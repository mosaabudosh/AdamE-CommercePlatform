

namespace E_Commerce.Domain.Domains.DMS
{
    public class Folder : SharedBase
    {
        public int FolderId { get; set; }
        public string Name { get; set; }
        public int? ParentFolderId { get; set; }
        public string Path { get; set; }

        public List<File> Files { get; set; }
        public virtual Folder ParentFolder { get; set; }
        public virtual List<Folder> ChildFolders { get; set; }
    }
}
