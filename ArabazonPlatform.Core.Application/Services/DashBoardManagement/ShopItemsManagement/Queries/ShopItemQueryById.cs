using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
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

namespace ArabazonPlatform.Application.Services.DashBoardManagement.ShopItemsManagement.Queries
{
    public class ShopItemQueryById
    {
        public class Query : IRequest<BaseResponse<ShopItemModel>>
        {
            public Guid ShopItemId { get; set; }
            public Query(Guid shopItemId)
            {
                ShopItemId = shopItemId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<ShopItemModel>>
        {
            public async Task<BaseResponse<ShopItemModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    ShopItemModel shopItem = _unitOfWork.GetRepository<ShopItem>().GetByExpressionAsNoTracking(p => p.ShopItemId == request.ShopItemId).Select(s => new ShopItemModel()
                    {
                        ShopItemId = s.ShopItemId,
                        ShopId = s.ShopId,
                        ShopItemStatusId = s.ShopItemStatusId,
                        TaxId = s.TaxId,
                        UnitGroupId = s.UnitGroupId,
                        Cost = s.Cost,
                        Image = s.Image,
                        InStocks = s.InStocks,
                        OutOfStocks = s.OutOfStocks,
                        Price = s.Price,
                        ProfitRate = s.ProfitRate,
                        ColorGroupId = s.ColorGroupId,
                        DefaultUnitId = s.DefaultUnitId,
                        DefaultColorId = s.DefaultColorId,
                        DiscountId = s.DiscountId,
                        IsFeatured = s.IsFeatured,
                        CategoryId = s.CategoryId,
                        IsBlock = s.IsBlock,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Code = s.Code,
                        Descreption = s.Descreption,
                        ShopMenuId = s.ShopMenuId,
                    }).FirstOrDefault();

                    List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSShopItemPath, $"{_cachingPlatformSettings[PlatformSettingsCode.DMSShopItemPath]}\\{shopItem.ShopItemId}"];
                    if (responseGetFileCreationModels.Any())
                    {
                        shopItem.FileCreationModels = responseGetFileCreationModels;
                    }
                   
                    return new BaseResponse<ShopItemModel>(true, string.Empty, string.Empty, shopItem);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopItemQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<ShopItemModel>(false, ex.Message);
                }
            }
        }
    }
}
