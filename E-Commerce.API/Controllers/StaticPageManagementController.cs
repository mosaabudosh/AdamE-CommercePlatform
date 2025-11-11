using E_Commerce.Application.Model.CMS;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.CMSManagement.StaticPageManagement.Commands;
using E_Commerce.Application.Services.CMSManagement.StaticPageManagement.Queries;
using E_Commerce.Application.Services.DashBoardManagement.ItemsManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.ItemsManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
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
