using Adam.Generic.Generic;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.AuthenticationManagement.ActionsManagement.Queries;
using E_Commerce.Application.Services.AuthenticationManagement.ScreensManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenManagementController(IMediator _mediator) : BaseController
    {
        [Route("secreens")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<ScreenModel>>>> GetSecreens(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new ScreensQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetSecreens), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<ScreenModel>>(false, ex.Message);
            }
        }

        [Route("secreens-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetSecreensCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<ScreenModel>> secreensResponse = await _mediator.Send(new ScreensQuery.Query(0, int.MaxValue, searchText));
                if (!secreensResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, secreensResponse.Message, secreensResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, secreensResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetSecreensCount), UserAccessor.GetCurrentUserId(), searchText);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("secreens-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetSecreensDDl()
        {
            try
            {
                return await _mediator.Send(new ScreensDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetSecreensDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }
    }
}
