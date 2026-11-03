using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.Advertisement;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
using ArabazonPlatform.Domain.Domains.Advertisements;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.AdvertisementsManagement.AdvertisementManagement.Queries
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
