using Adam.Generic.Generic;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DashBoardManagement.ShopItemStatusesManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.ShopItemStatusesManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopItemStatusManagementController(IMediator _mediator) : BaseController
    {
        [Route("shop-item-statuses")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<ShopItemStatusModel>>>> GetShopItemStatuss(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new ShopItemStatusesQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetShopItemStatuss), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<ShopItemStatusModel>>(false, ex.Message);
            }
        }

        [Route("shop-item-statuses-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetShopItemStatussCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<ShopItemStatusModel>> taxsResponse = await _mediator.Send(new ShopItemStatusesQuery.Query(0, int.MaxValue, searchText));
                if (!taxsResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, taxsResponse.Message, taxsResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, taxsResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetShopItemStatussCount), UserAccessor.GetCurrentUserId());
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
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetShopItemStatussDDl()
        {
            try
            {
                return await _mediator.Send(new ShopItemStatusesDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetShopItemStatussDDl), UserAccessor.GetCurrentUserId());
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
