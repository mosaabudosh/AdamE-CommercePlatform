using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Application.Model.Settings;
using ArabazonPlatform.Application.Services.AuthenticationManagement.ScreensManagement.Queries;
using ArabazonPlatform.Application.Services.AuthenticationManagement.UsersManagement.Commands;
using ArabazonPlatform.Application.Services.SettingsManagement.Commands;
using ArabazonPlatform.Application.Services.SettingsManagement.Queries;
using ArabazonPlatform.Infrastructure.HttpServices;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsManagementController(IMediator _mediator) : BaseController
    {
        [Route("settings")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<SettingsModel>>>> GetSettings(int pageNumber, int pageSize)
        {
            try
            {
                return await _mediator.Send(new SettingsQuery.Query(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetSettings), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<SettingsModel>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("setting-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateSetting([FromBody] SettingsModel settingsModel)
        {
            try
            {
                return await _mediator.Send(new SettingsUpdationCommands.Command(settingsModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateSetting), UserAccessor.GetCurrentUserId(), settingsModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [Route("settings-by-group")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<SettingsModel>>>> GetSettingsByGroup(string groupName)
        {
            try
            {
                return await _mediator.Send(new SettingsByGroupQuery.Query(groupName));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetSettingsByGroup), UserAccessor.GetCurrentUserId(), groupName);
                return new BaseResponse<List<SettingsModel>>(false, ex.Message);
            }
        }

    }
}
