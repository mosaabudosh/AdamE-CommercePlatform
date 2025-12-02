using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.Advertisement;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Domain.Domains.Advertisements;
using E_Commerce.Domain.Enum;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.AdvertisementsManagement.AdvertisementManagement.Queries
{
    public class ActiveStoreAdvertisementsQuery
    {
        public class Query : IRequest<BaseResponse<List<ActiveStoreAdvertisementModel>>>
        {    
            public Query() { }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<List<ActiveStoreAdvertisementModel>>>
        {
            public async Task<BaseResponse<List<ActiveStoreAdvertisementModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    DateTime today = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0, 0);
                    List<ActiveStoreAdvertisementModel> advertisementModels = _unitOfWork.GetRepository<Advertisement>().GetByExpressionAsNoTracking(p => p.AdvertisementStatusId != (int)Enums.AdvertisementStatuses.Expired && p.StartDate <= today && p.EndDate >= today ).Select(s => new ActiveStoreAdvertisementModel()
                    {
                        AdvertisementId = s.AdvertisementId,
                        Title = s.Title,
                        ImageUrl = s.ImageUrl,
                        TargetUrl = s.TargetUrl,
                    }).ToList();

                    if (advertisementModels.Any(p => !string.IsNullOrEmpty(p.ImageUrl)))
                    {
                        List<string> filePaths = advertisementModels.Where(p => !string.IsNullOrEmpty(p.ImageUrl)).Select(s => { return $"{_cachingPlatformSettings[PlatformSettingsCode.DMSAdPath]}\\{s.AdvertisementId}"; }).ToList();
                      
                        List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSAdPath, filePaths];
                        if (responseGetFileCreationModels.Any())
                        {
                            foreach (ActiveStoreAdvertisementModel advertisementModel in advertisementModels.Where(p => !string.IsNullOrEmpty(p.ImageUrl)))
                            {
                                FileCreationModel file = responseGetFileCreationModels.FirstOrDefault(p => p.FolderName == advertisementModel.ImageUrl);
                                advertisementModel.ImageUrl = file.FileContent;
                                advertisementModel.FileType = file?.FileType;
                            }
                        }
                    }

                    return new BaseResponse<List<ActiveStoreAdvertisementModel>>(true, string.Empty, string.Empty, advertisementModels);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ActiveStoreAdvertisementsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<ActiveStoreAdvertisementModel>>(false, ex.Message);
                }
            }
        }
    }
}
