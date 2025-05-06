using Adam.Logging.Interfaces;
using E_Commerce.Application.Interfaces.DMS;
using E_Commerce.Domain.Domains.DMS;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DMS
{
    internal class FileTypeService : IFileTypeService
    {
        #region Fileds
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExceptionService _exceptionService;
        private readonly IInformationService _informationService;
        private readonly IUserAccessor _userAccessor;
        #endregion

        #region Constructor
        public FileTypeService(IUnitOfWork unitOfWork, IExceptionService exceptionService, IUserAccessor userAccessor, IInformationService informationService)
        {
            _unitOfWork = unitOfWork;
            _exceptionService = exceptionService;
            _userAccessor = userAccessor;
            _informationService = informationService;
        }
        #endregion

        #region IFileTypeService
        public async Task<BaseResponse> CreateFileType(FileType fileType)
        {
            try
            {
                _unitOfWork.GetRepository<FileType>().Insert(fileType);         

                await _unitOfWork.CommitChanges();
                return new BaseResponse(true, "The File Type was created.");
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(CreateFileType), _userAccessor.GetCurrentUserId(), fileType);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse> DeleteFileType(int fileTypeId)
        {
            try
            {
                BaseResponse<FileType> fileType = await GetFileTypeById(fileTypeId);
                if (!fileType.IsSuccess)
                {
                    return fileType;
                }
                _unitOfWork.GetRepository<FileType>().Delete(fileType.DataObject);
                await _unitOfWork.CommitChanges();
                return new BaseResponse(true, string.Empty);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(DeleteFileType), _userAccessor.GetCurrentUserId(), fileTypeId);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<FileType>>> GetAllFileTypes()
        {
            try
            {
                IQueryable<FileType> fileTypesQuery = _unitOfWork.GetRepository<FileType>().GetByExpression();
                List<FileType> fileTypes = await _unitOfWork.GetRepository<FileType>().ToListAsync(fileTypesQuery);
                return new BaseResponse<List<FileType>>(true, string.Empty, fileTypes);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(GetAllFileTypes), _userAccessor.GetCurrentUserId());
                return new BaseResponse<List<FileType>>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<FileType>>> GetFileTypeByExtention(string extention)
        {
            try
            {
                IQueryable<FileType> fileTypesQuery = _unitOfWork.GetRepository<FileType>().GetByExpression(z => z.Extension == extention);

                List<FileType> fileTypes = await _unitOfWork.GetRepository<FileType>().ToListAsync(fileTypesQuery);

                return new BaseResponse<List<FileType>>(true, string.Empty, fileTypes);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(GetFileTypeByExtention), _userAccessor.GetCurrentUserId(), extention);
                return new BaseResponse<List<FileType>>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<FileType>> GetFileTypeById(int fileTypeId)
        {
            try
            {
                FileType fileType = await _unitOfWork.GetRepository<FileType>().GetById(fileTypeId);
                return new BaseResponse<FileType>(true, string.Empty, fileType);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(GetFileTypeById), _userAccessor.GetCurrentUserId(), fileTypeId);
                return new BaseResponse<FileType>(false, ex.Message);
            }
        }

        public async Task<BaseResponse> UpdateFileType(FileType fileType)
        {
            try
            {
                _unitOfWork.GetRepository<FileType>().Update(fileType);
               
                await _unitOfWork.CommitChanges();
                
                return new BaseResponse(true, "The file type was updated.");
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(UpdateFileType), _userAccessor.GetCurrentUserId(), fileType);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse<FileType>> GetFileTypeByFileExtension(string fileExtension)
        {
            try
            {
                FileType fileType = _unitOfWork.GetRepository<FileType>().GetByExpression(x => fileExtension.Trim().ToLower() == x.Extension).FirstOrDefault();
                return new BaseResponse<FileType>(true, string.Empty, fileType);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(GetFileTypeByFileExtension), _userAccessor.GetCurrentUserId(), fileExtension);
                return new BaseResponse<FileType>(false, ex.Message);
            }
        }

        #endregion
    }
}
