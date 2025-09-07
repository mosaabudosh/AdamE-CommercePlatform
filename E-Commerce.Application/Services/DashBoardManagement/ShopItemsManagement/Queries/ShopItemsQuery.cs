using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
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
            public int? CategoryId { get; set; }

            public Query(int pageNumber, int pageSize, string? searchText = null, int? shopId = null, int? categoryId = null)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
                SearchText = searchText;
                ShopId = shopId;
                CategoryId = categoryId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<List<ShopItemModel>>>
        {
            public async Task<BaseResponse<List<ShopItemModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<ShopItemModel> shopItemModels = _unitOfWork.GetRepository<ShopItem>().GetByExpressionAsNoTracking(p => (string.IsNullOrEmpty(request.SearchText) || p.Name.Contains(request.SearchText) || p.NameAr.Contains(request.SearchText)) && (!request.ShopId.HasValue || request.ShopId.Value == p.ShopId) && (!request.CategoryId.HasValue || request.CategoryId.Value == p.CategoryId)).Select(s => new ShopItemModel()
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
                        Name = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Name : s.NameAr,
                        Shop = s.Shop.Name,
                        ShopItemStatus = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.ShopItemStatus.Name : s.ShopItemStatus.NameAr,
                        Tax = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.Tax.Name : s.Tax.NameAr,
                        UnitGroup = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.UnitGroup.Name: s.UnitGroup.NameAr,
                        Images = new List<string>()
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

                    if (shopItemModels.Any(p => !string.IsNullOrEmpty(p.Image)))
                    {
                        string DMSShopItemPath = _cachingPlatformSettings[PlatformSettingsCode.DMSShopItemPath];

                        List<string> filePaths = shopItemModels.Where(p => !string.IsNullOrEmpty(p.Image)).Select(s => 
                        { 
                            return $"{DMSShopItemPath}\\{s.ShopItemId}"; 
                        }).ToList();
                        BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(filePaths));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return new BaseResponse<List<ShopItemModel>>(false, responseGetFileCreationModels.Message);
                        }
                        if (responseGetFileCreationModels.DataObject.Any())
                        {
                            foreach (ShopItemModel shopItemModel in shopItemModels.Where(p => !string.IsNullOrEmpty(p.Image)))
                            {
                                shopItemModel.Images.AddRange(responseGetFileCreationModels.DataObject.Where(p => p.FolderName == shopItemModel.ShopItemId.ToString()).Select(s => s.FileContent));
                            }
                        }
                    }

                    return new BaseResponse<List<ShopItemModel>>(true, string.Empty, string.Empty, shopItemModels);
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
