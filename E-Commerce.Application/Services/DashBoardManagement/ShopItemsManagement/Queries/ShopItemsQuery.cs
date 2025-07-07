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
    public class ShopItemsQuery
    {
        public class Query : IRequest<BaseResponse<List<ShopItemModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string? SearchText { get; set; }
            public int? ShopId { get; set; }

            public Query(int pageNumber, int pageSize, string? searchText = null, int? shopId = null)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
                SearchText = searchText;
                ShopId = shopId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<ShopItemModel>>>
        {
            public async Task<BaseResponse<List<ShopItemModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ShopItemModel> shopItemStatuses = _unitOfWork.GetRepository<ShopItem>().GetByExpressionAsNoTracking(p => string.IsNullOrEmpty(request.SearchText) || p.Item.Name.Contains(request.SearchText) || p.Item.NameAr.Contains(request.SearchText) && (!request.ShopId.HasValue || request.ShopId.Value == p.ShopId)).Select(s => new ShopItemModel()
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
                        UnitGroup = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.UnitGroup.Name: s.UnitGroup.NameAr
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<ShopItemModel>>(true, string.Empty, string.Empty, shopItemStatuses);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopItemsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<ShopItemModel>>(false, ex.Message);
                }
            }
        }
    }
}
