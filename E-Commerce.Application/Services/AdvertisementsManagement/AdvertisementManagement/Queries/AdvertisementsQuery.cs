using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.Advertisement;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Domain.Domains.Advertisements;
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<List<AdvertisementModel>>>
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
                        string DMSAdPath = _cachingPlatformSettings[PlatformSettingsCode.DMSAdPath];

                        List<string> filePaths = advertisementModels.Where(p => !string.IsNullOrEmpty(p.ImageUrl)).Select(s => { return $"{DMSAdPath}\\{s.AdvertisementId}"; }).ToList();
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(filePaths));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<List<AdvertisementModel>>(false, responseGetFileCreationModels.Message);
                        }
                        if (responseGetFileCreationModels.DataObject.Any())
                        {
                            foreach (AdvertisementModel advertisementModel in advertisementModels.Where(p => !string.IsNullOrEmpty(p.ImageUrl)))
                            {
                                advertisementModel.ImageUrl = responseGetFileCreationModels.DataObject.FirstOrDefault(p => p.FolderName == advertisementModel.ImageUrl).FileContent;
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
