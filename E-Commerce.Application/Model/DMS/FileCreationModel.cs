using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.DMS
{
    public class FileCreationModel
    {
        public string FileName { get; set; }
        public string FileContent { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public string FolderName { get; set; }
        public int? FolderId { get; set; }
        public int? FileTypeId { get; set; }
    }
}
