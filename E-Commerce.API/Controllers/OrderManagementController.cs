using Azure.Core;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DashBoardManagement.OrdersManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.OrdersManagement.Queries;
using E_Commerce.Application.Services.NotificationManagement;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Commerce.API.Controllers
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
        public async Task<ActionResult<BaseResponse>> CreateOrder([FromBody] OrderModel orderModel)
        {
            try
            {
                BaseResponse<OrderModel> responseOrderModel = await _mediator.Send(new OrderCreationCommands.Command(orderModel));
                if (!responseOrderModel.IsSuccess) 
                {
                    return responseOrderModel;
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

                return responseOrderModel;
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateOrder), UserAccessor.GetCurrentUserId(), orderModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [Route("orders-by-customer-id/{customerId}")]
        [HttpGet]
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

    }
}
