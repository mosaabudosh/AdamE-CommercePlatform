using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DashBoardManagement.OrdersManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.OrdersManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderManagementController(IMediator _mediator) : BaseController
    {
        [HttpPost]
        [Route("order-creation")]
        public async Task<ActionResult<BaseResponse>> CreateOrder([FromBody] OrderModel orderModel)
        {
            try
            {
                return await _mediator.Send(new OrderCreationCommands.Command(orderModel));
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
