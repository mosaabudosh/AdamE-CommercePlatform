using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<ShopItemModel>>
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

                    string DMSShopItemPath = _cachingPlatformSettings[PlatformSettingsCode.DMSShopItemPath];
                    string folderPath = $"{DMSShopItemPath}\\{shopItem.ShopItemId}";
                    BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFolderPathQuery.Query(folderPath));
                    if (!responseGetFileCreationModels.IsSuccess)
                    {
                        return new BaseResponse<ShopItemModel>(false, responseGetFileCreationModels.Message);
                    }
                    if (responseGetFileCreationModels.DataObject.Any())
                    {
                        shopItem.FileCreationModels = responseGetFileCreationModels.DataObject;
                    }

                    //List<ShopItem> shopItems = _unitOfWork.GetRepository<ShopItem>().GetByExpressionAsNoTracking(p => p.ShopItemId != shopItem.ShopItemId && string.IsNullOrEmpty(p.Image )).ToList();
                    //foreach (ShopItem item in shopItems)
                    //{
                    //    item.Image = item.ShopItemId.ToString();
                    //    await _unitOfWork.CommitChanges();
                    //    FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                    //    {
                    //        FolderName = $"{item.ShopItemId}",
                    //        FolderPath = $"{DMSShopItemPath}",
                    //        FileCreationModels = responseGetFileCreationModels.DataObject.Select(s => new FileCreationModel()
                    //        {
                    //            FileContent = s.FileContent,
                    //            FileName = s.FileName,
                    //            FileType = s.FileType                                
                    //        }).ToList()
                    //    };
                    //    BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                    //    if (!createFolderAndOneFileResponse.IsSuccess)
                    //    {
                    //        await _exceptionService.LogErrorAsync(createFolderAndOneFileResponse.Message, "CreateFolderAndOneFileCommands", _userAccessor.GetCurrentUserId());
                    //    }
                    //}
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
