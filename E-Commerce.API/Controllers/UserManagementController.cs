using E_Commerce.API.Model;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Model.Settings;
using E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Commands;
using E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController(IMediator _mediator) : BaseController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("registration")]
        public async Task<ActionResult<BaseResponse>> Registration([FromBody] UserModel userModel)
        {
            try
            {
                return await _mediator.Send(new RegistrationCommands.Command(userModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(Registration), UserAccessor.GetCurrentUserId(), userModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<BaseResponse<UserModel>>> Login(LoginModel loginModel)
        {
            try
            {
                return await _mediator.Send(new LoginCommands.Command(loginModel.UserName, loginModel.Password));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(Login), UserAccessor.GetCurrentUserId(), loginModel);
                return new BaseResponse<UserModel>(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("logout")]
        [HttpPost]
        public async Task<ActionResult<BaseResponse>> LogOut([FromBody] RefreshTokenModel refreshTokenModel)
        {
            try
            {
                return await _mediator.Send(new LogOutCommands.Command(refreshTokenModel.RefreshToken));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(LogOut), UserAccessor.GetCurrentUserId(), refreshTokenModel);
                return new BaseResponse<string>(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("check-refresh-token")]
        [HttpPost]
        public async Task<BaseResponse<UserModel>> CheckRefreshAccessToken([FromBody] RefreshTokenModel refreshTokenModel)
        {
            try
            {
                return await _mediator.Send(new CheckRefreshAccessTokenCommands.Command(refreshTokenModel.RefreshToken));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CheckRefreshAccessToken), UserAccessor.GetCurrentUserId(), refreshTokenModel);
                return new BaseResponse<UserModel>(false, ex.Message);
            }
        }

        [Route("change-password")]
        [HttpPost]
        public async Task<ActionResult<BaseResponse>> ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
        {
            try
            {
                return await _mediator.Send(new ChangePasswordCommands.Command(changePasswordModel.UserId, changePasswordModel.OldPassword, changePasswordModel.NewPassword));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(ChangePassword), UserAccessor.GetCurrentUserId(), changePasswordModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [Route("users")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<UserModel>>>> GetUsers(int pageNumber, int pageSize, string? searchText, int? cityId, int? shopId, int? languageId)
        {
            try
            {
                return await _mediator.Send(new UsersQuery.Query(pageNumber, pageSize, searchText, cityId, shopId, languageId,true));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetUsers), UserAccessor.GetCurrentUserId(), pageNumber, pageSize,searchText,cityId,shopId,languageId);
                return new BaseResponse<List<UserModel>>(false, ex.Message);
            }
        }

        [Route("users-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetUsersCount(string? searchText, int? cityId, int? shopId, int? languageId)
        {
            try
            {
                BaseResponse<List<UserModel>> responseUserModels = await _mediator.Send(new UsersQuery.Query(0, int.MaxValue, searchText, cityId, shopId, languageId));
                if (!responseUserModels.IsSuccess)
                {
                    return new BaseResponse<int>(false, responseUserModels.Message, responseUserModels.ErrorCode);
                }
                return new BaseResponse<int>(true,string.Empty,string.Empty, responseUserModels.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetUsersCount), UserAccessor.GetCurrentUserId(), searchText, cityId, shopId, languageId);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("user-by-id/{userId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<UserModel>>> GetUserById(Guid userId)
        {
            try
            {
                return await _mediator.Send(new GetUserByIdQuery.Query(userId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex , nameof(GetUserById),UserAccessor.GetCurrentUserId(),userId );
                return new BaseResponse<UserModel>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("user-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateUser([FromBody] UserModel userModel)
        {
            try
            {
                return await _mediator.Send(new UserUpdationCommands.Command(userModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateUser), UserAccessor.GetCurrentUserId(), userModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("user-deletion/{userId}")]
        public async Task<ActionResult<BaseResponse>> DeleteUser(string userId)
        {
            try
            {
                return await _mediator.Send(new UserDeletionCommands.Command(new Guid(userId)));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteUser), UserAccessor.GetCurrentUserId(), userId);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("user-lock-un-lock")]
        public async Task<ActionResult<BaseResponse>> UpdateUserLockUnlock([FromBody] UserLockUnlockModel userLockUnlockModel)
        {
            try
            {
                return await _mediator.Send(new UserLockUnlockCommand.Command(new Guid(userLockUnlockModel.UserId), userLockUnlockModel.IsLock));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateUserLockUnlock), UserAccessor.GetCurrentUserId(), userLockUnlockModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("users-printing")]
        public async Task<IActionResult> UsersPrinting(string? searchText, int? cityId, int? shopId, int? languageId)
        {
            try
            {
                BaseResponse<List<UserModel>> usersResponse = await _mediator.Send(new UsersQuery.Query(0, int.MaxValue, searchText, cityId, shopId, languageId));
                if (!usersResponse.IsSuccess)
                {
                    throw new Exception(usersResponse.Message);
                }

                Dictionary<string, object> dataSources = new Dictionary<string, object>();
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                var userModels = usersResponse.DataObject.Select(x => new
                {
                    UserFullName = HttpAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? $"{x.FirstName} {x.SecondName} {x.LastName}" : $"{x.FirstNameAr} {x.SecondNameAr} {x.LastNameAr}",
                    MobileNumber = x.MobileNumber,
                    Gender = x.Gender,
                    Age = x.Age
                }).ToList();

                PlatformInfoModel platformInfoModel = CachingPlatformSettings.GetPlatformInfo();
                platformInfoModel.PrintedBy = UserAccessor.GetCurrentUserName();
                dataSources.Add("PlatformInfoModel", new[] { platformInfoModel });
                dataSources.Add("UserModel", userModels);

                return ToPDF("\\Reports\\Users.rdlc", dataSources, parameters);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await ExceptionService.LogErrorAsync(ex, nameof(UsersPrinting), UserAccessor.GetCurrentUserId(),searchText, cityId, shopId, languageId);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("user-settings-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateUserSettings([FromBody] List<UserSettingModel> userSettingModels)
        {
            try
            {
                return await _mediator.Send(new UserSettingsUpdationCommands.Command(userSettingModels));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateUserSettings), UserAccessor.GetCurrentUserId(), userSettingModels);
                return new BaseResponse(false, ex.Message);
            }
        }

    }
}
