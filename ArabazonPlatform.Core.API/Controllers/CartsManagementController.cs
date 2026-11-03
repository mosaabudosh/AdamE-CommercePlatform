using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DashBoardManagement.CartsManagement.Commands;
using ArabazonPlatform.Application.Services.DashBoardManagement.CartsManagement.Queries;
using ArabazonPlatform.Application.Services.DashBoardManagement.CartStatusesManagement.Commands;
using ArabazonPlatform.Application.Services.DashBoardManagement.CitiesManagement.Queries;
using ArabazonPlatform.Domain.Domains.Authorisation;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PdfSharp;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsManagementController(IMediator _mediator) : BaseController
    {
        [HttpPost]
        [Route("cart-creation")]
        public async Task<ActionResult<BaseResponse<CartModel>>> CreateCart([FromBody] CartModel cartModel)
        {
            try
            {
                return await _mediator.Send(new CartCreationCommands.Command(cartModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateCart), UserAccessor.GetCurrentUserId(), cartModel);
                return new BaseResponse<CartModel>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("cart-modification")]
        public async Task<ActionResult<BaseResponse<CartModel>>> UpdateCart([FromBody] CartModel cartModel)
        {
            try
            {
                return await _mediator.Send(new CartUpdationCommands.Command(cartModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateCart), UserAccessor.GetCurrentUserId(), cartModel);
                return new BaseResponse<CartModel>(false, ex.Message);
            }
        }

        [Route("carts")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<CartModel>>>> GetCarts(int pageNumber, int pageSize, string? customerId = null, int? cartStatusId = null,string? shopItemId = null)
        {
            try
            {
                return await _mediator.Send(new CartsQuery.Query(pageNumber, pageSize, string.IsNullOrEmpty(customerId) ? null : new Guid(customerId), cartStatusId, string.IsNullOrEmpty(shopItemId) ? null : new Guid(shopItemId)));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCarts), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, customerId, cartStatusId, shopItemId);
                return new BaseResponse<List<CartModel>>(false, ex.Message);
            }
        }

        [Route("carts-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetCartsCount(string? customerId = null, int? cartStatusId = null, string? shopItemId = null)
        {
            try
            {
                BaseResponse<List<CartModel>> cartsResponse = await _mediator.Send(new CartsQuery.Query(0, int.MaxValue, string.IsNullOrEmpty(customerId) ? null : new Guid(customerId), cartStatusId, string.IsNullOrEmpty(shopItemId) ? null : new Guid(shopItemId)));
                if (!cartsResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, cartsResponse.Message, cartsResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, cartsResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCartsCount), UserAccessor.GetCurrentUserId(), customerId, cartStatusId, shopItemId);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

    }
}
