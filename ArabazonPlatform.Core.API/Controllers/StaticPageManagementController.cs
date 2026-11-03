using ArabazonPlatform.Application.Model.CMS;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.CMSManagement.StaticPageManagement.Commands;
using ArabazonPlatform.Application.Services.CMSManagement.StaticPageManagement.Queries;
using ArabazonPlatform.Application.Services.DashBoardManagement.ItemsManagement.Commands;
using ArabazonPlatform.Application.Services.DashBoardManagement.ItemsManagement.Queries;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaticPageManagementController(IMediator _mediator) : BaseController
    {
        [Route("static-page-by-id/{staticPageId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<StaticPageModel>>> StaticPageById(int staticPageId)
        {
            try
            {
                return await _mediator.Send(new StaticPageQueryById.Query(staticPageId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(StaticPageById), UserAccessor.GetCurrentUserId(), staticPageId);
                return new BaseResponse<StaticPageModel>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("static-page-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateStaticPage([FromBody] StaticPageModel staticPageModel)
        {
            try
            {
                return await _mediator.Send(new StaticPageUpdationCommands.Command(staticPageModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateStaticPage), UserAccessor.GetCurrentUserId(), staticPageModel);
                return new BaseResponse(false, ex.Message);
            }
        }


    }
}
