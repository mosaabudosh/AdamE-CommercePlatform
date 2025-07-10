using Adam.Generic.Generic;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DashBoardManagement.ColorGroupsManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.ColorGroupsManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorGroupManagementController(IMediator _mediator) : BaseController
    {
        [Route("color-groups")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<ColorGroupModel>>>> GetColorGroups(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new ColorGroupsQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetColorGroups), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<ColorGroupModel>>(false, ex.Message);
            }
        }

        [Route("color-groups-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetColorGroupsCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<ColorGroupModel>> colorGroupsResponse = await _mediator.Send(new ColorGroupsQuery.Query(0, int.MaxValue, searchText));
                if (!colorGroupsResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, colorGroupsResponse.Message, colorGroupsResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, colorGroupsResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetColorGroupsCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("color-group-by-id/{colorGroupId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<ColorGroupModel>>> ColorGroupById(int colorGroupId)
        {
            try
            {
                return await _mediator.Send(new ColorGroupQueryById.Query(colorGroupId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(ColorGroupById), UserAccessor.GetCurrentUserId(), colorGroupId);
                return new BaseResponse<ColorGroupModel>(false, ex.Message);
            }
        }

        [Route("color-groups-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetColorGroupsDDl()
        {
            try
            {
                return await _mediator.Send(new ColorGroupsDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetColorGroupsDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("color-group-creation")]
        public async Task<ActionResult<BaseResponse>> CreateColorGroup([FromBody] ColorGroupModel colorGroupModel)
        {
            try
            {
                return await _mediator.Send(new ColorGroupCreationCommands.Command(colorGroupModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateColorGroup), UserAccessor.GetCurrentUserId(), colorGroupModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("color-group-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateColorGroup([FromBody] ColorGroupModel colorGroupModel)
        {
            try
            {
                return await _mediator.Send(new ColorGroupUpdationCommands.Command(colorGroupModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateColorGroup), UserAccessor.GetCurrentUserId(), colorGroupModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("color-group-deletion/{colorGroupId}")]
        public async Task<ActionResult<BaseResponse>> DeleteColorGroup(int colorGroupId)
        {
            try
            {
                return await _mediator.Send(new ColorGroupDeletionCommands.Command(colorGroupId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteColorGroup), UserAccessor.GetCurrentUserId(), colorGroupId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
