using Adam.Generic.Generic;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DashBoardManagement.TaxsManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.TaxsManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxManagementController(IMediator _mediator) : BaseController
    {
        [Route("taxs")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<TaxModel>>>> GetTaxs(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new TaxsQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetTaxs), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<TaxModel>>(false, ex.Message);
            }
        }

        [Route("taxs-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetTaxsCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<TaxModel>> taxsResponse = await _mediator.Send(new TaxsQuery.Query(0, int.MaxValue, searchText));
                if (!taxsResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, taxsResponse.Message, taxsResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, taxsResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetTaxsCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("tax-by-id/{taxId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<TaxModel>>> TaxById(int taxId)
        {
            try
            {
                return await _mediator.Send(new TaxQueryById.Query(taxId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(TaxById), UserAccessor.GetCurrentUserId(), taxId);
                return new BaseResponse<TaxModel>(false, ex.Message);
            }
        }

        [Route("taxs-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetTaxsDDl()
        {
            try
            {
                return await _mediator.Send(new TaxsDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetTaxsDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("tax-creation")]
        public async Task<ActionResult<BaseResponse>> CreateTax([FromBody] TaxModel taxModel)
        {
            try
            {
                return await _mediator.Send(new TaxCreationCommands.Command(taxModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateTax), UserAccessor.GetCurrentUserId(), taxModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("tax-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateTax([FromBody] TaxModel taxModel)
        {
            try
            {
                return await _mediator.Send(new TaxUpdationCommands.Command(taxModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateTax), UserAccessor.GetCurrentUserId(), taxModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("tax-deletion/{taxId}")]
        public async Task<ActionResult<BaseResponse>> DeleteTax(int taxId)
        {
            try
            {
                return await _mediator.Send(new TaxDeletionCommands.Command(taxId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteTax), UserAccessor.GetCurrentUserId(), taxId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
