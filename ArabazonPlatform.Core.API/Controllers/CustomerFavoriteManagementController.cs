using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DashBoardManagement.CartsManagement.Commands;
using ArabazonPlatform.Application.Services.DashBoardManagement.CustomerFavoritesManagement.Commands;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArabazonPlatform.API.Controllers
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
