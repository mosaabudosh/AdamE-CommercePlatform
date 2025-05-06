using E_Commerce.API.Model;
using E_Commerce.Application.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Application.Services.Users.Commands;
using E_Commerce.Application.Services.UserType.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : BaseController
    {
        #region Fields
        private readonly IUserManagementService _userManagementService;
        private readonly IMediator _mediator;
        #endregion

        #region Constructor
        public UserManagementController(IUserManagementService userManagementService, IMediator mediator)
        {
            _userManagementService = userManagementService;
            _mediator = mediator;
        }
        #endregion

        [AllowAnonymous]
        [HttpPost]
        [Route("registration")]
        public async Task<ActionResult<BaseResponse>> Registration([FromBody] UserModel userModel)
        {
            try
            {
                return await _userManagementService.Registration(userModel);
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
                return await _userManagementService.Login(loginModel.UserName, loginModel.Password);
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
                return await _userManagementService.LogOut(refreshTokenModel.RefreshToken);
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
        public async Task<BaseResponse<UserModel>> CheckRefreshAccessToken([FromBody] RefreshTokenModel refreshToken)
        {
            try
            {
                return await _userManagementService.CheckRefreshAccessToken(refreshToken.RefreshToken);
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CheckRefreshAccessToken), UserAccessor.GetCurrentUserId(), refreshToken);
                return new BaseResponse<UserModel>(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("change-password")]
        [HttpPost]
        public async Task<ActionResult<BaseResponse>> ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
        {
            try
            {
                return await _userManagementService.ChangePassword(changePasswordModel.UserId, changePasswordModel.OldPassword, changePasswordModel.NewPassword);
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(ChangePassword), UserAccessor.GetCurrentUserId(), changePasswordModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [Route("users")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<UserModel>>>> GetUsers(int pageNumber, int pageSize)
        {
            try
            {
                return await _userManagementService.GetUsers(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetUsers), UserAccessor.GetCurrentUserId(), pageNumber, pageSize);
                return new BaseResponse<List<UserModel>>(false, ex.Message);
            }
        }

        [Route("users-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetUsersCount()
        {
            try
            {
                return await _userManagementService.GetUsersCount();
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetUsersCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("user-by-id/{userId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<UserModel>>> GetUserById(Guid userId)
        {
            try
            {
                return await _userManagementService.GetUserById(userId);
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
                return await _userManagementService.UpdateUser(userModel);
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
                return await _userManagementService.DeleteUser(userId);
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
    }
}
