using E_Commerce.Domain.Domains.DMS;
using Generic.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Interfaces.DMS
{
    public interface IFileTypeService
    {
        Task<BaseResponse<List<FileType>>> GetAllFileTypes();
        Task<BaseResponse<FileType>> GetFileTypeById(int fileTypeId);
        Task<BaseResponse<List<FileType>>> GetFileTypeByIds(List<int> fileTypeIds);
        Task<BaseResponse> CreateFileType(FileType fileType);
        Task<BaseResponse> UpdateFileType(FileType fileType);
        Task<BaseResponse> DeleteFileType(int fileTypeId);
        Task<BaseResponse<List<FileType>>> GetFileTypeByExtention(string extention);
        Task<BaseResponse<FileType>> GetFileTypeByFileExtension(string fileExtension);
    }
}
