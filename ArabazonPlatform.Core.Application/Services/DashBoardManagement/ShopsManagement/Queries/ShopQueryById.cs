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
    public class ShopQueryById
    {
        public class Query : IRequest<BaseResponse<ShopModel>>
        {
            public int ShopId { get; set; }
            public Query(int shopId)
            {
                ShopId = shopId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<ShopModel>>
        {
            public async Task<BaseResponse<ShopModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    ShopModel shop = _unitOfWork.GetRepository<Shop>().GetByExpressionAsNoTracking(p => p.ShopId == request.ShopId).Select(s => new ShopModel()
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
                        PhoneNumber = s.PhoneNumber,
                        IsViewInHome = s.IsViewInHome,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude
                    }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(shop.Logo))
                    {
                        List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSShopPath, $"{_cachingPlatformSettings[PlatformSettingsCode.DMSShopPath]}\\{shop.ShopId}"];
                        if (responseGetFileCreationModels.Any())
                        {
                            shop.FileCreationModel = responseGetFileCreationModels.FirstOrDefault();
                        }
                    }

                    return new BaseResponse<ShopModel>(true, string.Empty, string.Empty, shop);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<ShopModel>(false, ex.Message);
                }
            }
        }
    }
}
