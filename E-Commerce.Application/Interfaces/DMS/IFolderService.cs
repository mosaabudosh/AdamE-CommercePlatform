using Adam.Generic.Generic;
using E_Commerce.Domain.Domains.DMS;
using Generic.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Interfaces.DMS
{
    public interface IFolderService
    {
        Task<BaseResponse> CreateFolder(Folder folde);
        Task<BaseResponse> DeleteFolder(int folderId, bool isCommit = true);
        Task<BaseResponse<Folder>> GetFolderByParentPathId(string parentPathId);
        Task<BaseResponse<List<Folder>>> GetFoldersByParentFolderId(int parentFolderId);
        Task<BaseResponse<Folder>> GetFolderByFolderId(int folderId);
        Task<BaseResponse<List<Folder>>> GetRootFolders();
        Task<BaseResponse<Folder>> RenameFolderAsync(int folderId, string newName);
        Task<BaseResponse> CreateFolderAndFiles(Folder folde, List<FileCreationModel> fileCreationModels);
        Task<BaseResponse> CreateFolderAndFilesPhysically(Folder folder, FileCreationModel fileCreationModel);
        Task<BaseResponse<Folder>> GetFolderById(int folderId);
        Task<BaseResponse<List<FileCreationModel>>> GetFilesByFolderPath(string folderPath);
        Task<BaseResponse<List<FileCreationModel>>> GetFolderByPathAndName(string folderPath, string folderName);
        Task<BaseResponse<List<FileCreationModel>>> GetImagesByFilePath(string filePath);
        Task<BaseResponse> CreateFolderAndOneFile(FolderAndFileCreationModel folderAndFileCreationModel);
        Task<BaseResponse<List<FileCreationModel>>> GetFilesByFilePath(List<string> filePaths);
        Task<BaseResponse> CreateFolderAndFilesWithoutDelete(Folder folder, List<FileCreationModel> fileCreationModels);
        Task<BaseResponse<FileCreationModel>> GetImagesByFileName(string fileName);
        Task<BaseResponse> DeleteFilesByFolderId(int folderId);
        Task<BaseResponse> UpdatFolderName(string path, string find, string replace);
        Task<BaseResponse<List<FileCreationModel>>> GetFilesByRootFolderPath(string rootFolderPath);
        Task<BaseResponse<List<FileCreationModel>>> GetImagesByFileNameList(List<string> filesName);
    }
}
