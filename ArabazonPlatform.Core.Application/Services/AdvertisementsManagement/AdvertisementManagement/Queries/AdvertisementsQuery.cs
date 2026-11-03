using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.Advertisement;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
using ArabazonPlatform.Domain.Domains.Advertisements;
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
    public class AdvertisementsQuery
    {
        public class Query : IRequest<BaseResponse<List<AdvertisementModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string? SearchText { get; set; }

            public Query(int pageNumber, int pageSize, string? searchText = null)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
                SearchText = searchText;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<List<AdvertisementModel>>>
        {
            public async Task<BaseResponse<List<AdvertisementModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<AdvertisementModel> advertisementModels = _unitOfWork.GetRepository<Advertisement>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Title.Contains(request.SearchText)).Select(s => new AdvertisementModel()
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
                        AdPackageName = s.AdPackageId.HasValue ? s.AdPackage.Name : null,
                        AdvertisementStatus = s.AdvertisementStatus.Name,
                        ShopAdPackageName = s.ShopAdPackageId.HasValue ? s.ShopAdPackage.Shop.Name : null,
                        AdClicks = s.AdClicks.Any() ? s.AdClicks.Count().ToString() : "0",
                        AdImpressions = s.AdImpressions.Any() ? s.AdImpressions.Count().ToString() : "0"
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

                    if (advertisementModels.Any(p => !string.IsNullOrEmpty(p.ImageUrl)))
                    {
                        List<string> filePaths = advertisementModels.Where(p => !string.IsNullOrEmpty(p.ImageUrl)).Select(s => { return $"{_cachingPlatformSettings[PlatformSettingsCode.DMSAdPath]}\\{s.AdvertisementId}"; }).ToList();

                        List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSAdPath, filePaths];
                        if (responseGetFileCreationModels.Any())
                        {
                            foreach (AdvertisementModel advertisementModel in advertisementModels.Where(p => !string.IsNullOrEmpty(p.ImageUrl)))
                            {
                                advertisementModel.ImageUrl = responseGetFileCreationModels.FirstOrDefault(p => p.FolderName == advertisementModel.ImageUrl).FileContent;
                            }
                        }
                    }

                    return new BaseResponse<List<AdvertisementModel>>(true, string.Empty, string.Empty, advertisementModels);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "AdvertisementsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<AdvertisementModel>>(false, ex.Message);
                }
            }
        }
    }
}
