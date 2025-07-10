using Adam.Generic.Generic;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DashBoardManagement.ColorsManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.ColorsManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorManagementController(IMediator _mediator) : BaseController
    {
        [Route("colors")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<ColorModel>>>> GetColors(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new ColorsQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetColors), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<ColorModel>>(false, ex.Message);
            }
        }

        [Route("colors-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetColorsCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<ColorModel>> colorsResponse = await _mediator.Send(new ColorsQuery.Query(0, int.MaxValue, searchText));
                if (!colorsResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, colorsResponse.Message, colorsResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, colorsResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetColorsCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("color-by-id/{colorId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<ColorModel>>> ColorById(int colorId)
        {
            try
            {
                return await _mediator.Send(new ColorQueryById.Query(colorId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(ColorById), UserAccessor.GetCurrentUserId(), colorId);
                return new BaseResponse<ColorModel>(false, ex.Message);
            }
        }

        [Route("colors-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetColorsDDl()
        {
            try
            {
                return await _mediator.Send(new ColorsDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetColorsDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("color-creation")]
        public async Task<ActionResult<BaseResponse>> CreateColor([FromBody] ColorModel colorModel)
        {
            try
            {
                return await _mediator.Send(new ColorCreationCommands.Command(colorModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateColor), UserAccessor.GetCurrentUserId(), colorModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("color-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateColor([FromBody] ColorModel colorModel)
        {
            try
            {
                return await _mediator.Send(new ColorUpdationCommands.Command(colorModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateColor), UserAccessor.GetCurrentUserId(), colorModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("color-deletion/{colorId}")]
        public async Task<ActionResult<BaseResponse>> DeleteColor(int colorId)
        {
            try
            {
                return await _mediator.Send(new ColorDeletionCommands.Command(colorId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteColor), UserAccessor.GetCurrentUserId(), colorId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
