using ArabazonPlatform.Generic.Generic;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DashBoardManagement.ShopItemStatusesManagement.Commands;
using ArabazonPlatform.Application.Services.DashBoardManagement.ShopItemStatusesManagement.Queries;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopItemStatusManagementController(IMediator _mediator) : BaseController
    {
        [Route("shop-item-statuses")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<ShopItemStatusModel>>>> GetShopItemStatuses(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new ShopItemStatusesQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetShopItemStatuses), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<ShopItemStatusModel>>(false, ex.Message);
            }
        }

        [Route("shop-item-statuses-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetShopItemStatusesCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<ShopItemStatusModel>> ShopItemStatusesResponse = await _mediator.Send(new ShopItemStatusesQuery.Query(0, int.MaxValue, searchText));
                if (!ShopItemStatusesResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, ShopItemStatusesResponse.Message, ShopItemStatusesResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, ShopItemStatusesResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetShopItemStatusesCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("shop-item-status-by-id/{shopItemStatusId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<ShopItemStatusModel>>> ShopItemStatusById(int shopItemStatusId)
        {
            try
            {
                return await _mediator.Send(new ShopItemStatusQueryById.Query(shopItemStatusId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(ShopItemStatusById), UserAccessor.GetCurrentUserId(), shopItemStatusId);
                return new BaseResponse<ShopItemStatusModel>(false, ex.Message);
            }
        }

        [Route("shop-item-statuses-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetShopItemStatusesDDl(string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new ShopItemStatusesDDLQuery.Query(searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetShopItemStatusesDDl), UserAccessor.GetCurrentUserId(),searchText);
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("shop-item-status-creation")]
        public async Task<ActionResult<BaseResponse>> CreateShopItemStatus([FromBody] ShopItemStatusModel shopItemStatusModel)
        {
            try
            {
                return await _mediator.Send(new ShopItemStatusCreationCommands.Command(shopItemStatusModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateShopItemStatus), UserAccessor.GetCurrentUserId(), shopItemStatusModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("shop-item-status-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateShopItemStatus([FromBody] ShopItemStatusModel shopItemStatusModel)
        {
            try
            {
                return await _mediator.Send(new ShopItemStatusUpdationCommands.Command(shopItemStatusModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateShopItemStatus), UserAccessor.GetCurrentUserId(), shopItemStatusModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("shop-item-status-deletion/{shopItemStatusId}")]
        public async Task<ActionResult<BaseResponse>> DeleteShopItemStatus(int shopItemStatusId)
        {
            try
            {
                return await _mediator.Send(new ShopItemStatusDeletionCommands.Command(shopItemStatusId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteShopItemStatus), UserAccessor.GetCurrentUserId(), shopItemStatusId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
