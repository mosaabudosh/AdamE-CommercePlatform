using Azure.Core;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DashBoardManagement.OrdersManagement.Commands;
using ArabazonPlatform.Application.Services.DashBoardManagement.OrdersManagement.Queries;
using ArabazonPlatform.Application.Services.NotificationManagement;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderManagementController : BaseController
    {
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly IMediator _mediator;

        public OrderManagementController(IHubContext<NotificationHub> notificationHub, IMediator mediator)
        {
            _notificationHub = notificationHub;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("order-creation")]
        public async Task<ActionResult<BaseResponse<string>>> CreateOrder([FromBody] OrderModel orderModel)
        {
            try
            {
                BaseResponse<OrderModel> responseOrderModel = await _mediator.Send(new OrderCreationCommands.Command(orderModel));
                if (!responseOrderModel.IsSuccess) 
                {
                    return new BaseResponse<string>(false, responseOrderModel.Message, responseOrderModel.ErrorCode, string.Empty);
                }
                foreach (NotificationModel notification in responseOrderModel.DataObject.Notifications)
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

                return new BaseResponse<string>(true, string.Empty, responseOrderModel.DataObject.ReferenceNumber);
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateOrder), UserAccessor.GetCurrentUserId(), orderModel);
                return new BaseResponse<string>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("modification-order-status")]
        public async Task<ActionResult<BaseResponse>> ModificationOrderStatus([FromBody] UpdateOrderStatusModel updateOrderStatusModel)
        {
            try
            {
                BaseResponse<List<NotificationModel>> responseUpdateOrderStatus = await _mediator.Send(new OrderUpdateOrderStatusCommands.Command(updateOrderStatusModel));
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
                await ExceptionService.LogErrorAsync(ex, nameof(ModificationOrderStatus), UserAccessor.GetCurrentUserId(), updateOrderStatusModel);
                return new BaseResponse(false, ex.Message);
            }
        }


        [HttpGet]
        [Route("orders-by-customer-id/{customerId}")]
        public async Task<ActionResult<BaseResponse<List<OrderModel>>>> GetOrdersByCustomerId(Guid customerId)
        {
            try
            {
                return await _mediator.Send(new OrdersByCustomerIdQuery.Query(customerId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetOrdersByCustomerId), UserAccessor.GetCurrentUserId(), customerId);
                return new BaseResponse<List<OrderModel>>(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("orders")]
        public async Task<ActionResult<BaseResponse<List<OrderModel>>>> GetOrders(Guid? customerId = null, Guid? itemId = null, Guid? userId = null, int? categoryId = null, int? orderStatusId = null)
        {
            try
            {
                return await _mediator.Send(new OrdersQuery.Query(customerId, itemId, userId, categoryId, orderStatusId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetOrders), UserAccessor.GetCurrentUserId(), customerId, itemId, userId, categoryId, orderStatusId);
                return new BaseResponse<List<OrderModel>>(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("orders-count")]
        public async Task<ActionResult<BaseResponse<int>>> GetOrdersCount(Guid? customerId = null, Guid? itemId = null, Guid? userId = null, int? categoryId = null, int? orderStatusId = null)
        {
            try
            {
                BaseResponse<List<OrderModel>> responseOrdersQuery = await _mediator.Send(new OrdersQuery.Query(customerId, itemId, userId, categoryId, orderStatusId));
                if (!responseOrdersQuery.IsSuccess)
                {
                    return new BaseResponse<int>(false, responseOrdersQuery.Message, responseOrdersQuery.ErrorCode, 0);
                }
                return new BaseResponse<int>(false, string.Empty,responseOrdersQuery.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetOrdersCount), UserAccessor.GetCurrentUserId(), customerId, itemId, userId, categoryId, orderStatusId);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

    }
}
