using ArabazonPlatform.Generic.Generic;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DashBoardManagement.CartStatusesManagement.Commands;
using ArabazonPlatform.Application.Services.DashBoardManagement.CartStatusesManagement.Queries;
using ArabazonPlatform.Domain.Domains;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartStatusManagementController(IMediator _mediator) : BaseController
    {
        [Route("cart-statuses")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<CartStatusModel>>>> GetCartStatuses(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new CartStatusQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCartStatuses), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<CartStatusModel>>(false, ex.Message);
            }
        }

        [Route("cart-statuses-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetCartStatusesCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<CartStatusModel>> cartStatusesResponse = await _mediator.Send(new CartStatusQuery.Query(0, int.MaxValue, searchText));
                if (!cartStatusesResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, cartStatusesResponse.Message, cartStatusesResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, cartStatusesResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCartStatusesCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("cart-status-by-id/{cartStatusId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<CartStatusModel>>> CartStatusById(int cartStatusId)
        {
            try
            {
                return await _mediator.Send(new CartStatusQueryById.Query(cartStatusId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CartStatusById), UserAccessor.GetCurrentUserId(), cartStatusId);
                return new BaseResponse<CartStatusModel>(false, ex.Message);
            }
        }

        [Route("cart-statuses-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetCartStatusesDDl()
        {
            try
            {
                return await _mediator.Send(new CartStatusDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCartStatusesDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("cart-status-creation")]
        public async Task<ActionResult<BaseResponse>> CreateCartStatus([FromBody] CartStatusModel cartStatusModel)
        {
            try
            {
                return await _mediator.Send(new CartStatusCreationCommands.Command(cartStatusModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateCartStatus), UserAccessor.GetCurrentUserId(), cartStatusModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("cart-status-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateCartStatus([FromBody] CartStatusModel cartStatusModel)
        {
            try
            {
                return await _mediator.Send(new CartStatusUpdationCommands.Command(cartStatusModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateCartStatus), UserAccessor.GetCurrentUserId(), cartStatusModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("cart-status-deletion/{cartStatusId}")]
        public async Task<ActionResult<BaseResponse>> DeleteCartStatus(int cartStatusId)
        {
            try
            {
                return await _mediator.Send(new CartStatusDeletionCommands.Command(cartStatusId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteCartStatus), UserAccessor.GetCurrentUserId(), cartStatusId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
