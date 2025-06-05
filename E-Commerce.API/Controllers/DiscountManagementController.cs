using Adam.Generic.Generic;
using E_Commerce.Application.Model;
using E_Commerce.Application.Services.DashBoardManagement.DiscountManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.DiscountManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountManagementController(IMediator _mediator) : BaseController
    {
        [Route("discounts")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DiscountModel>>>> GetDiscounts(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new DiscountsQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetDiscounts), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<DiscountModel>>(false, ex.Message);
            }
        }

        [Route("discounts-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetDiscountsCount(string? searchText = null, int? countryId = null)
        {
            try
            {
                BaseResponse<List<DiscountModel>> discountsResponse = await _mediator.Send(new DiscountsQuery.Query(0, int.MaxValue, searchText));
                if (!discountsResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, discountsResponse.Message, discountsResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, discountsResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetDiscountsCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("discount-by-id/{discountId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<DiscountModel>>> DiscountById(int discountId)
        {
            try
            {
                return await _mediator.Send(new DiscountQueryById.Query(discountId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DiscountById), UserAccessor.GetCurrentUserId(), discountId);
                return new BaseResponse<DiscountModel>(false, ex.Message);
            }
        }

        [Route("discounts-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetDiscountsDDl()
        {
            try
            {
                return await _mediator.Send(new DiscountsDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetDiscountsDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("discount-creation")]
        public async Task<ActionResult<BaseResponse>> CreateDiscount([FromBody] DiscountModel discountModel)
        {
            try
            {
                return await _mediator.Send(new DiscountCreationCommands.Command(discountModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateDiscount), UserAccessor.GetCurrentUserId(), discountModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("discount-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateDiscount([FromBody] DiscountModel discountModel)
        {
            try
            {
                return await _mediator.Send(new DiscountUpdationCommands.Command(discountModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateDiscount), UserAccessor.GetCurrentUserId(), discountModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("discount-deletion/{discountId}")]
        public async Task<ActionResult<BaseResponse>> DeleteDiscount(int discountId)
        {
            try
            {
                return await _mediator.Send(new DiscountDeletionCommands.Command(discountId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteDiscount), UserAccessor.GetCurrentUserId(), discountId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
