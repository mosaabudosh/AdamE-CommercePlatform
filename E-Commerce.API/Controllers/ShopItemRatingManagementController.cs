using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DashBoardManagement.ShopItemRatingsManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.ShopItemRatingsManagement.Queries;
using E_Commerce.Application.Services.DashBoardManagement.ShopItemsManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.ShopItemsManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopItemRatingManagementController(IMediator _mediator) : BaseController
    {

        [HttpPost]
        [Route("shop-item-rating-creation")]
        public async Task<ActionResult<BaseResponse>> CreateShopItemRating([FromBody] ShopItemRatingModel shopItemRatingModel)
        {
            try
            {
                return await _mediator.Send(new ShopItemRatingCreationCommands.Command(shopItemRatingModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateShopItemRating), UserAccessor.GetCurrentUserId(), shopItemRatingModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [Route("shop-item-rating-by-shop-item-id/{shopItemId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<ShopItemRatingModel>>>> ShopItemRatingByShopItemId(Guid shopItemId)
        {
            try
            {
                return await _mediator.Send(new ShopItemRatingQueryByShopItemId.Query(shopItemId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(ShopItemRatingByShopItemId), UserAccessor.GetCurrentUserId(), shopItemId);
                return new BaseResponse<List<ShopItemRatingModel>>(false, ex.Message);
            }
        }


    }
}
