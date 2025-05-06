using E_Commerce.Domain.Domains.DMS;
using Generic.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Interfaces.DMS
{
    public interface IFolderConfigurationService
    {
        Task<BaseResponse<List<FolderConfiguration>>> GetAllFolderConfigurations();
        Task<BaseResponse<FolderConfiguration>> GetFolderConfigurationById(int folderConfigurationId);
        Task<BaseResponse> CreateFolderConfiguration(FolderConfiguration folderConfiguration);
        Task<BaseResponse> UpdateFolderConfiguration(FolderConfiguration folderConfiguration);
        Task<BaseResponse> DeleteFolderConfiguration(int folderConfigurationId);
        Task<BaseResponse<FolderConfiguration>> GetFolderConfiguration();
        Task<BaseResponse<FolderConfiguration>> GetAllFolderConfiguration(bool isActive);
        Task<string> GetRootPath();
    }
}
