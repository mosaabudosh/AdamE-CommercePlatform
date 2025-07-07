using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DashBoard;
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

namespace E_Commerce.Application.Services.DashBoardManagement.ShopItemsManagement.Queries
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<ShopItemModel>>
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
                        ItemId = s.ItemId,
                        OutOfStocks = s.OutOfStocks,
                        Price = s.Price,
                        ProfitRate = s.ProfitRate,
                        ItemName = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Item.Name : s.Item.NameAr,
                        Shop = s.Shop.Name,
                        ShopItemStatus = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.ShopItemStatus.Name : s.ShopItemStatus.NameAr,
                        Tax = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Tax.Name : s.Tax.NameAr,
                        UnitGroup = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.UnitGroup.Name : s.UnitGroup.NameAr
                    }).FirstOrDefault();
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
