using Adam.Logging.Interfaces;
using E_Commerce.Application.Interfaces.DMS;
using E_Commerce.Domain.Domains.DMS;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.DMS
{
    internal class FolderConfigurationService : IFolderConfigurationService
    {
        #region Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IExceptionService _exceptionService;
        private readonly IInformationService _informationService;
        private readonly IUserAccessor _userAccessor;
        #endregion

        #region Constructor
        public FolderConfigurationService(IUnitOfWork unitOfWork, IConfiguration configuration, IExceptionService exceptionService, IUserAccessor userAccessor, IInformationService informationService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _exceptionService = exceptionService;
            _userAccessor = userAccessor;
            _informationService = informationService;
        }
        #endregion

        #region IFolderConfigurationService
        public async Task<BaseResponse> CreateFolderConfiguration(FolderConfiguration folderConfiguration)
        {
            try
            {
                _unitOfWork.GetRepository<FolderConfiguration>().Insert(folderConfiguration);            

                await _unitOfWork.CommitChanges();
               
                return new BaseResponse(true, "The folder configuration was created.");
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(CreateFolderConfiguration), _userAccessor.GetCurrentUserId(), folderConfiguration);
                return new BaseResponse(false, "The folder configuration was not created.");
            }
        }

        public async Task<BaseResponse> DeleteFolderConfiguration(int folderConfigurationId)
        {
            try
            {
                BaseResponse<FolderConfiguration> folderConfiguration = await GetFolderConfigurationById(folderConfigurationId);
                if (!folderConfiguration.IsSuccess)
                {
                    return folderConfiguration;
                }
                _unitOfWork.GetRepository<FolderConfiguration>().Delete(folderConfiguration.DataObject);             

                await _unitOfWork.CommitChanges();
                
                return new BaseResponse(true, "The folder configuration was deleted.");
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(DeleteFolderConfiguration), _userAccessor.GetCurrentUserId(), folderConfigurationId);
                return new BaseResponse(false, "The folder configuration was not deleted.");
            }
        }

        public async Task<BaseResponse<List<FolderConfiguration>>> GetAllFolderConfigurations()
        {
            try
            {
                IQueryable<FolderConfiguration> folderConfigurationQuery = _unitOfWork.GetRepository<FolderConfiguration>().GetByExpression();

                List<FolderConfiguration> folderConfigurations = await _unitOfWork.GetRepository<FolderConfiguration>().ToListAsync(folderConfigurationQuery);

                return new BaseResponse<List<FolderConfiguration>>(true, string.Empty, folderConfigurations);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(GetAllFolderConfigurations), _userAccessor.GetCurrentUserId());
                return new BaseResponse<List<FolderConfiguration>>(false, "The folder configuration was not deleted.");
            }
        }

        public async Task<BaseResponse<FolderConfiguration>> GetFolderConfigurationById(int folderConfigurationId)
        {
            try
            {
                FolderConfiguration folderConfiguration = await _unitOfWork.GetRepository<FolderConfiguration>().GetById(folderConfigurationId);
                return new BaseResponse<FolderConfiguration>(true,string.Empty, folderConfiguration);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(GetFolderConfigurationById), _userAccessor.GetCurrentUserId(), folderConfigurationId);
                return new BaseResponse<FolderConfiguration>(false, "The folder configuration was not deleted.");
            }
        }

        public async Task<BaseResponse> UpdateFolderConfiguration(FolderConfiguration folderConfiguration)
        {
            try
            {
                _unitOfWork.GetRepository<FolderConfiguration>().Update(folderConfiguration);
           
                await _unitOfWork.CommitChanges();
                return new BaseResponse(true, "The folder configuration was updated.");
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(UpdateFolderConfiguration), _userAccessor.GetCurrentUserId(), folderConfiguration);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse<FolderConfiguration>> GetFolderConfiguration()
        {
            try
            {
                FolderConfiguration folderConfiguration = _unitOfWork.GetRepository<FolderConfiguration>().GetByExpression().FirstOrDefault();
                return new BaseResponse<FolderConfiguration>(true, string.Empty, folderConfiguration);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(GetFolderConfiguration), _userAccessor.GetCurrentUserId());
                return new BaseResponse<FolderConfiguration>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<FolderConfiguration>> GetAllFolderConfiguration(bool isActive)
        {
            try
            {
                FolderConfiguration folderConfiguration = _unitOfWork.GetRepository<FolderConfiguration>().GetByExpression(x => x.IsActive == isActive).FirstOrDefault();

                return new BaseResponse<FolderConfiguration>(true, string.Empty, folderConfiguration);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(GetAllFolderConfiguration), _userAccessor.GetCurrentUserId(), isActive);
                return new BaseResponse<FolderConfiguration>(false, ex.Message);
            }
        }

        public async Task<string> GetRootPath()
        {
            string rootPath = _configuration.GetSection("RootPath").Value;
            return rootPath;
        }
        #endregion
    }
}
