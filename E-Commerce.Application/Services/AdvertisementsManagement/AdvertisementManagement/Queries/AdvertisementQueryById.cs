using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.Advertisement;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Domain.Domains.Advertisements;
using E_Commerce.Domain.Domains.DashBoard;
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
    public class AdvertisementQueryById
    {
        public class Query : IRequest<BaseResponse<AdvertisementModel>>
        {
            public int AdvertisementId { get; set; }
            public Query(int advertisementId)
            {
                AdvertisementId = advertisementId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<AdvertisementModel>>
        {
            public async Task<BaseResponse<AdvertisementModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    AdvertisementModel advertisement = _unitOfWork.GetRepository<Advertisement>().GetByExpressionAsNoTracking(p => p.AdvertisementId == request.AdvertisementId).Select(s => new AdvertisementModel()
                    {
                        AdvertisementId = s.AdvertisementId,
                        Title = s.Title,
                        AdvertisementStatusId = s.AdvertisementStatusId,
                        EndDate = s.EndDate,
                        StartDate = s.StartDate,
                        ShopAdPackageId = s.ShopAdPackageId,
                        AdPackageId = s.AdPackageId,
                        ImageUrl = s.ImageUrl,
                        TargetUrl = s.TargetUrl,
                    }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(advertisement.ImageUrl))
                    {
                        List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSAdPath, $"{_cachingPlatformSettings[PlatformSettingsCode.DMSAdPath]}\\{advertisement.AdvertisementId}"];
                        if (responseGetFileCreationModels.Any())
                        {
                            advertisement.FileCreationModel = responseGetFileCreationModels.FirstOrDefault();
                        }
                    }

                    return new BaseResponse<AdvertisementModel>(true, string.Empty, string.Empty, advertisement);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "AdvertisementQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<AdvertisementModel>(false, ex.Message);
                }
            }
        }
    }
}
