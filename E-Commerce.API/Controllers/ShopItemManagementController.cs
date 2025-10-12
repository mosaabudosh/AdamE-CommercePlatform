using Adam.Generic.Generic;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DashBoardManagement.OrdersManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.ShopItemsManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.ShopItemsManagement.Queries;
using E_Commerce.Application.Services.NotificationManagement;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopItemManagementController : BaseController
    {
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly IMediator _mediator;

        public ShopItemManagementController(IHubContext<NotificationHub> notificationHub, IMediator mediator)
        {
            _notificationHub = notificationHub;
            _mediator = mediator;
        }

        [Route("shop-items")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<ShopItemModel>>>> GetShopItems(int pageNumber, int pageSize, string? searchText = null, int? shopId = null, int? categoryId = null)
        {
            try
            {
                return await _mediator.Send(new ShopItemsQuery.Query(pageNumber, pageSize, searchText, shopId, categoryId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetShopItems), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText, shopId, categoryId);
                return new BaseResponse<List<ShopItemModel>>(false, ex.Message);
            }
        }

        [Route("shop-items-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetShopItemsCount(string? searchText = null, int? shopId = null, int? categoryId = null)
        {
            try
            {
                BaseResponse<List<ShopItemModel>> shopItemsResponse = await _mediator.Send(new ShopItemsQuery.Query(0, int.MaxValue, searchText, shopId, categoryId));
                if (!shopItemsResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, shopItemsResponse.Message, shopItemsResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, shopItemsResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetShopItemsCount), UserAccessor.GetCurrentUserId(),searchText, shopId, categoryId);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("shop-item-by-id/{shopItemId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<ShopItemModel>>> ShopItemById(Guid shopItemId)
        {
            try
            {
                return await _mediator.Send(new ShopItemQueryById.Query(shopItemId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(ShopItemById), UserAccessor.GetCurrentUserId(), shopItemId);
                return new BaseResponse<ShopItemModel>(false, ex.Message);
            }
        }

        [Route("shop-items-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetShopItemsDDl(string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new ShopItemsDDLQuery.Query(searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetShopItemsDDl), UserAccessor.GetCurrentUserId(),searchText);
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("shop-item-creation")]
        public async Task<ActionResult<BaseResponse>> CreateShopItem([FromBody] ShopItemModel shopItemModel)
        {
            try
            {
                return await _mediator.Send(new ShopItemCreationCommands.Command(shopItemModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateShopItem), UserAccessor.GetCurrentUserId(), shopItemModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("shop-item-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateShopItem([FromBody] ShopItemModel shopItemModel)
        {
            try
            {
                return await _mediator.Send(new ShopItemUpdationCommands.Command(shopItemModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateShopItem), UserAccessor.GetCurrentUserId(), shopItemModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("shop-item-deletion/{shopItemId}")]
        public async Task<ActionResult<BaseResponse>> DeleteShopItem(Guid shopItemId)
        {
            try
            {
                return await _mediator.Send(new ShopItemDeletionCommands.Command(shopItemId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteShopItem), UserAccessor.GetCurrentUserId(), shopItemId);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("modification-shop-item-status")]
        public async Task<ActionResult<BaseResponse>> ModificationShopItemStatus([FromBody] UpdateShopItemStatusModel updateShopItemStatusModel)
        {
            try
            {
                BaseResponse<List<NotificationModel>> responseUpdateOrderStatus = await _mediator.Send(new UpdateShopItemStatusCommands.Command(updateShopItemStatusModel));
                if (!responseUpdateOrderStatus.IsSuccess)
                {
                    return new BaseResponse(false, responseUpdateOrderStatus.Message, responseUpdateOrderStatus.ErrorCode);
                }
                foreach (NotificationModel notification in responseUpdateOrderStatus.DataObject)
                {
                    if (notification.UserId.HasValue)
                    {
                        await _notificationHub.Clients.Group($"{notification.UserId.Value.ToString()}").SendAsync($"{notification.UserId.Value.ToString()}", notification.Subject);

                    }
                    else if (notification.CustomerId.HasValue)
                    {
                        await _notificationHub.Clients.Group($"{notification.CustomerId.Value.ToString()}").SendAsync($"{notification.CustomerId.Value.ToString()}", notification.Subject);
                    }
                }

                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(ModificationShopItemStatus), UserAccessor.GetCurrentUserId(), updateShopItemStatusModel);
                return new BaseResponse(false, ex.Message);
            }
        }

    }
}
