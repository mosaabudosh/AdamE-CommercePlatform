using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DashBoardManagement.CartsManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.CustomerFavoritesManagement.Commands;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerFavoriteManagementController(IMediator _mediator) : BaseController
    {
        [HttpPost]
        [Route("customer-favorite-creation")]
        public async Task<ActionResult<BaseResponse<CustomerFavoriteModel>>> CreateCustomerFavorite([FromBody] CustomerFavoriteModel customerFavoriteModel)
        {
            try
            {
                return await _mediator.Send(new CustomerFavoriteCreationCommands.Command(customerFavoriteModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateCustomerFavorite), UserAccessor.GetCurrentUserId(), customerFavoriteModel);
                return new BaseResponse<CustomerFavoriteModel>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("customer-favorite-modification")]
        public async Task<ActionResult<BaseResponse<CustomerFavoriteModel>>> UpdateCustomerFavorite([FromBody] CustomerFavoriteModel customerFavoriteModel)
        {
            try
            {
                return await _mediator.Send(new CustomerFavoriteUpdationCommands.Command(customerFavoriteModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateCustomerFavorite), UserAccessor.GetCurrentUserId(), customerFavoriteModel);
                return new BaseResponse<CustomerFavoriteModel>(false, ex.Message);
            }
        }
    }
}
