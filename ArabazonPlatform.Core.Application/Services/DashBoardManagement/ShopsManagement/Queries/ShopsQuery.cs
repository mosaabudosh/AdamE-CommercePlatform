using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.ShopsManagement.Queries
{
    public class ShopsQuery
    {
        public class Query : IRequest<BaseResponse<List<ShopModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string? SearchText { get; set; }
            public int? CityId { get; set; }
            public int? ParentShopId { get; set; }
            public bool IsForDashboard { get; set; }
            public List<int>? CategoryIds { get; set; }

            public Query(int pageNumber, int pageSize, string? searchText = null, int? cityId = null, int? parentShopId = null, bool isForDashboard = false, List<int>? categoryIds = null)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
                SearchText = searchText;
                CityId = cityId;
                ParentShopId = parentShopId;
                IsForDashboard = isForDashboard;
                CategoryIds = categoryIds;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<List<ShopModel>>>
        {
            public async Task<BaseResponse<List<ShopModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ShopModel> shops = _unitOfWork.GetRepository<Shop>().GetByExpressionAsNoTracking(p => 
                    (string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.CommercialName.Contains(request.SearchText) || p.Descreption.Contains(request.SearchText) || p.About.Contains(request.SearchText)) && 
                    (!request.CityId.HasValue || p.CityId == request.CityId.Value) && 
                    (!request.ParentShopId.HasValue || p.ParentShopId == request.ParentShopId.Value) && 
                    (request.IsForDashboard ? p.IsViewInHome : true) && (request.CategoryIds == null || !request.CategoryIds.Any() || request.CategoryIds.Any(c =>  p.ShopCategories.Any(s => s.CategoryId == c)))).Select(s => new ShopModel()
                    {
                        ShopId = s.ShopId,
                        ParentShopId = s.ParentShopId,
                        Logo = s.Logo,
                        Name = s.Name,
                        Code = s.Code,
                        Descreption = s.Descreption,
                        IsBlock = s.IsBlock,
                        About = s.About,
                        Adresse = s.Adresse,
                        CityId = s.CityId,
                        City = !s.CityId.HasValue ? null : _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.City.Name : s.City.NameAr,
                        CommercialName = s.CommercialName,
                        HasBranch = s.HasBranch,
                        NoOfBranch = s.NoOfBranch,
                        NoOfVendor = s.NoOfVendor,
                        PersonConcateName = s.PersonConcateName,
                        PhoneNumber = s.PhoneNumber
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

                    if (shops.Any(p => !string.IsNullOrEmpty(p.Logo)))
                    {
                        List<string> filePaths = shops.Where(p => !string.IsNullOrEmpty(p.Logo)).Select(s => { return $"{_cachingPlatformSettings[PlatformSettingsCode.DMSShopPath]}\\{s.ShopId}"; }).ToList();
                        List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSShopPath, filePaths];
                        if (responseGetFileCreationModels.Any())
                        {
                            foreach (ShopModel shop in shops.Where(p => !string.IsNullOrEmpty(p.Logo)))
                            {
                                shop.Logo = responseGetFileCreationModels.FirstOrDefault(p => p.FolderName == shop.ShopId.ToString())?.FileContent;
                            }
                        }
                    }

                    return new BaseResponse<List<ShopModel>>(true, string.Empty, string.Empty, shops);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<ShopModel>>(false, ex.Message);
                }
            }
        }
    }
}
