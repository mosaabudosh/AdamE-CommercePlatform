using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DashBoardManagement.UnitsManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.UnitsManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitManagementController(IMediator _mediator) : BaseController
    {
        [Route("units")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<UnitModel>>>> GetUnits(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new UnitsQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetUnits), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<UnitModel>>(false, ex.Message);
            }
        }

        [Route("units-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetUnitsCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<UnitModel>> unitsResponse = await _mediator.Send(new UnitsQuery.Query(0, int.MaxValue, searchText));
                if (!unitsResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, unitsResponse.Message, unitsResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, unitsResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetUnitsCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("unit-by-id/{unitId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<UnitModel>>> UnitById(int unitId)
        {
            try
            {
                return await _mediator.Send(new UnitQueryById.Query(unitId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UnitById), UserAccessor.GetCurrentUserId(), unitId);
                return new BaseResponse<UnitModel>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("unit-creation")]
        public async Task<ActionResult<BaseResponse>> CreateUnit([FromBody] UnitModel unitModel)
        {
            try
            {
                return await _mediator.Send(new UnitCreationCommands.Command(unitModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateUnit), UserAccessor.GetCurrentUserId(), unitModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("unit-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateUnit([FromBody] UnitModel unitModel)
        {
            try
            {
                return await _mediator.Send(new UnitUpdationCommands.Command(unitModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateUnit), UserAccessor.GetCurrentUserId(), unitModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("unit-deletion/{unitId}")]
        public async Task<ActionResult<BaseResponse>> DeleteUnit(int unitId)
        {
            try
            {
                return await _mediator.Send(new UnitDeletionCommands.Command(unitId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteUnit), UserAccessor.GetCurrentUserId(), unitId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
