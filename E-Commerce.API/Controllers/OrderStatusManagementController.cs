using Adam.Generic.Generic;
using E_Commerce.Application.Model;
using E_Commerce.Application.Services.AuthenticationManagement.ActionsManagement.Queries;
using E_Commerce.Application.Services.DashBoardManagement.OrderStatusesManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.OrderStatusesManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusManagementController(IMediator _mediator) : BaseController
    {
        [Route("order-statuses")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<OrderStatusModel>>>> GetOrderStatuses(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new OrderStatusesQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetOrderStatuses), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<OrderStatusModel>>(false, ex.Message);
            }
        }

        [Route("order-statuses-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetOrderStatusesCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<OrderStatusModel>> orderStatusesResponse = await _mediator.Send(new OrderStatusesQuery.Query(0, int.MaxValue, searchText));
                if (!orderStatusesResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, orderStatusesResponse.Message, orderStatusesResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, orderStatusesResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetOrderStatusesCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("order-status-by-id/{orderStatusId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<OrderStatusModel>>> OrderStatusById(int orderStatusId)
        {
            try
            {
                return await _mediator.Send(new OrderStatusQueryById.Query(orderStatusId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(OrderStatusById), UserAccessor.GetCurrentUserId(), orderStatusId);
                return new BaseResponse<OrderStatusModel>(false, ex.Message);
            }
        }

        [Route("order-statuses-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetOrderStatusesDDL()
        {
            try
            {
                return await _mediator.Send(new OrderStatusesDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetOrderStatusesDDL), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }


        [HttpPost]
        [Route("order-status-creation")]
        public async Task<ActionResult<BaseResponse>> CreateOrderStatus([FromBody] OrderStatusModel orderStatusModel)
        {
            try
            {
                return await _mediator.Send(new OrderStatusCreationCommands.Command(orderStatusModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateOrderStatus), UserAccessor.GetCurrentUserId(), orderStatusModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("order-status-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateOrderStatus([FromBody] OrderStatusModel orderStatusModel)
        {
            try
            {
                return await _mediator.Send(new OrderStatusUpdationCommands.Command(orderStatusModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateOrderStatus), UserAccessor.GetCurrentUserId(), orderStatusModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("order-status-deletion/{orderStatusId}")]
        public async Task<ActionResult<BaseResponse>> DeleteOrderStatus(int orderStatusId)
        {
            try
            {
                return await _mediator.Send(new OrderStatusDeletionCommands.Command(orderStatusId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteOrderStatus), UserAccessor.GetCurrentUserId(), orderStatusId);
                return new BaseResponse(false, ex.Message);
            }
        }

    }
}
