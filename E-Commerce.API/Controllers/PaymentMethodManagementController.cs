using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DashBoardManagement.PaymentMethodsManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.PaymentMethodsManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodManagementController(IMediator _mediator) : BaseController
    {
        [Route("payment-methods")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<PaymentMethodModel>>>> GetPaymentMethods(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new PaymentMethodsQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetPaymentMethods), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<PaymentMethodModel>>(false, ex.Message);
            }
        }

        [Route("payment-methods-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetPaymentMethodsCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<PaymentMethodModel>> paymentMethodsResponse = await _mediator.Send(new PaymentMethodsQuery.Query(0, int.MaxValue, searchText));
                if (!paymentMethodsResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, paymentMethodsResponse.Message, paymentMethodsResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, paymentMethodsResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetPaymentMethodsCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("payment-method-by-id/{paymentMethodId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<PaymentMethodModel>>> PaymentMethodById(int paymentMethodId)
        {
            try
            {
                return await _mediator.Send(new PaymentMethodQueryById.Query(paymentMethodId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(PaymentMethodById), UserAccessor.GetCurrentUserId(), paymentMethodId);
                return new BaseResponse<PaymentMethodModel>(false, ex.Message);
            }
        }


        [HttpPost]
        [Route("payment-method-creation")]
        public async Task<ActionResult<BaseResponse>> CreatePaymentMethod([FromBody] PaymentMethodModel paymentMethodModel)
        {
            try
            {
                return await _mediator.Send(new PaymentMethodCreationCommands.Command(paymentMethodModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreatePaymentMethod), UserAccessor.GetCurrentUserId(), paymentMethodModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("payment-method-modification")]
        public async Task<ActionResult<BaseResponse>> UpdatePaymentMethod([FromBody] PaymentMethodModel paymentMethodModel)
        {
            try
            {
                return await _mediator.Send(new PaymentMethodUpdationCommands.Command(paymentMethodModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdatePaymentMethod), UserAccessor.GetCurrentUserId(), paymentMethodModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("payment-method-deletion/{paymentMethodId}")]
        public async Task<ActionResult<BaseResponse>> DeletePaymentMethod(int paymentMethodId)
        {
            try
            {
                return await _mediator.Send(new PaymentMethodDeletionCommands.Command(paymentMethodId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeletePaymentMethod), UserAccessor.GetCurrentUserId(), paymentMethodId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
