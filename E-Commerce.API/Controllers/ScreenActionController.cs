using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.AuthenticationManagement.ScreenActionsManagement.Commands;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenActionController(IMediator _mediator) : BaseController
    {
        [HttpPost]
        [Route("screen-actions-creation")]
        public async Task<ActionResult<BaseResponse>> CreateScreenActions([FromBody] List<ScreenActionModel> screenActions)
        {
            try
            {
                return await _mediator.Send(new ScreenActionCreationCommands.Command(screenActions));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateScreenActions), UserAccessor.GetCurrentUserId(), screenActions);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("screen-action-deletion/{screenActionId}")]
        public async Task<ActionResult<BaseResponse>> DeleteScreenAction(int screenActionId)
        {
            try
            {
                return await _mediator.Send(new ScreenActionDeletionCommands.Command(screenActionId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteScreenAction), UserAccessor.GetCurrentUserId(), screenActionId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
