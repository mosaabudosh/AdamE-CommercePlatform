using Adam.Generic.Generic;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DashBoardManagement.UnitGroupsManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.UnitGroupsManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitGroupManagementController(IMediator _mediator) : BaseController
    {
        [Route("unit-groups")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<UnitGroupModel>>>> GetUnitGroups(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new UnitGroupsQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetUnitGroups), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<UnitGroupModel>>(false, ex.Message);
            }
        }

        [Route("unit-groups-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetUnitGroupsCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<UnitGroupModel>> unitGroupsResponse = await _mediator.Send(new UnitGroupsQuery.Query(0, int.MaxValue, searchText));
                if (!unitGroupsResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, unitGroupsResponse.Message, unitGroupsResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, unitGroupsResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetUnitGroupsCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("unit-group-by-id/{unitGroupId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<UnitGroupModel>>> UnitGroupById(int unitGroupId)
        {
            try
            {
                return await _mediator.Send(new UnitGroupQueryById.Query(unitGroupId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UnitGroupById), UserAccessor.GetCurrentUserId(), unitGroupId);
                return new BaseResponse<UnitGroupModel>(false, ex.Message);
            }
        }

        [Route("unit-groups-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetUnitGroupsDDl()
        {
            try
            {
                return await _mediator.Send(new UnitGroupsDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetUnitGroupsDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("unit-group-creation")]
        public async Task<ActionResult<BaseResponse>> CreateUnitGroup([FromBody] UnitGroupModel unitGroupModel)
        {
            try
            {
                return await _mediator.Send(new UnitGroupCreationCommands.Command(unitGroupModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateUnitGroup), UserAccessor.GetCurrentUserId(), unitGroupModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("unit-group-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateUnitGroup([FromBody] UnitGroupModel unitGroupModel)
        {
            try
            {
                return await _mediator.Send(new UnitGroupUpdationCommands.Command(unitGroupModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateUnitGroup), UserAccessor.GetCurrentUserId(), unitGroupModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("unit-group-deletion/{unitGroupId}")]
        public async Task<ActionResult<BaseResponse>> DeleteUnitGroup(int unitGroupId)
        {
            try
            {
                return await _mediator.Send(new UnitGroupDeletionCommands.Command(unitGroupId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteUnitGroup), UserAccessor.GetCurrentUserId(), unitGroupId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
