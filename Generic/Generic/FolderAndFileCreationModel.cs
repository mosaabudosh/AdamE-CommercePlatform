using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.Generic.Generic
{
    public class FolderAndFileCreationModel
    {
        public string FolderName { get; set; }
        public string FolderPath { get; set; }
        public List<FileCreationModel> FileCreationModels { get; set; }
    }
}
