using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Authentication.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.Enum;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using static Adam.Generic.Constants.Constants;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Commands
{
    public class UserLoginCommands
    {
        public class Command : IRequest<BaseResponse<UserModel>>
        {
            public string UserName;
            public string Password;
            public Command(string userName, string password)
            {
                UserName = userName;
                Password = password;
            }
            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IAuthenticationService _authenticationService, ITokenHandler _tokenHandler, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Command, BaseResponse<UserModel>>
            {
                public async Task<BaseResponse<UserModel>> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        IQueryable<User> users = _unitOfWork.GetRepository<User>().GetByExpression(x => x.UserName == request.UserName && x.IsActive == true, includedEntities: new List<System.Linq.Expressions.Expression<Func<User, object>>>
                        {
                            z => z.UserLogins.Where(p => !string.IsNullOrEmpty(p.AccessToken)),
                            z => z.UserRoles,
                            z => z.UserSettings
                        });
                        User user = await _unitOfWork.GetRepository<User>().FirstOrDefaultAsync(users);
                        if (user is null)
                        {
                            return new BaseResponse<UserModel>(false, string.Empty, ErrorCode.InvalidUserNameOrPassword);
                        }
                        if (!_authenticationService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                        {
                            return new BaseResponse<UserModel>(false, string.Empty, ErrorCode.InvalidUserNameOrPassword);
                        }
                        if (user.UserLogins.Any())
                        {
                            foreach (UserLogin exsistUserLogin in user.UserLogins)
                            {
                                await _mediator.Send(new KillUserRefreshAccessTokenCommands.Command(exsistUserLogin, false));
                            }
                        }
                        user.UserLogins = new List<UserLogin>();
                        UserLogin userLogin = new UserLogin()
                        {
                            UserId = user.UserId,
                            User = user,
                            LoginDate = DateTime.UtcNow,
                            CreationUserId = user.UserId,
                            CreationDate = DateTime.UtcNow,
                            IsActive = true
                        };
                        userLogin = _tokenHandler.BuildUserTokens(userLogin);

                        _unitOfWork.GetRepository<UserLogin>().Insert(userLogin);

                        await _unitOfWork.CommitChanges();

                        string? screenId = user.UserSettings.FirstOrDefault(p => p.PlatformSettingId == (int)Enums.PlatformSettings.DefaultPage)?.Value;
                        List<int> roleIds = user.UserRoles.Select(s => s.RoleId).ToList();
                        List<UserMenu> userMenus = GetUserMenu(roleIds,!string.IsNullOrEmpty(screenId) ? Convert.ToInt32(screenId) : null);

                        UserModel userModel = new UserModel()
                        {
                            UserId = user.UserId.ToString(),
                            UserName = user.UserName,
                            FirstName = user.FirstName,
                            FirstNameAr = user.FirstNameAr,
                            LastName = user.LastName,
                            LastNameAr = user.LastNameAr,
                            AccessToken = userLogin.AccessToken,
                            RefreshToken = userLogin.RefreshToken,
                            GenderId = user.GenderId,
                            UserTypeId = user.UserTypeId,
                            ProfileImage = user.ProfileImage,
                            ShopId = user.ShopId,
                            RoleIds = roleIds,
                            Email = user.Email,
                            UserMenus = userMenus,
                            UserSettingModels = user.UserSettings.Select(s => new UserSettingModel()
                            {
                                UserSettingId = s.UserSettingId,
                                PlatformSettingId = s.PlatformSettingId,
                                UserId = s.UserId,
                                Value = s.Value
                            }).ToList()
                        };
                        if (!string.IsNullOrEmpty(userModel.ProfileImage))
                        {
                            List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSUserPath, $"{_cachingPlatformSettings[PlatformSettingsCode.DMSUserPath]}\\{userModel.UserId.ToString().ToLower()}"];
                          
                            if (responseGetFileCreationModels.Any())
                            {
                                userModel.FileCreationModel = responseGetFileCreationModels.FirstOrDefault();
                            }
                        }
                        return new BaseResponse<UserModel>(true, string.Empty, string.Empty, userModel);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "UserLoginCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse<UserModel>(false, ex.Message);
                    }
                }

                private List<UserMenu> GetUserMenu(List<int> roleIds,int? screenId = null)
                {
                    List<UserMenu> userMenus = new List<UserMenu>();
                    try
                    {
                        userMenus = _unitOfWork.GetRepository<RoleScreen>().GetByExpressionAsNoTracking(p => roleIds.Contains(p.RoleId) && p.Screen.IsInMenue).GroupBy(g => g.Screen.ModuleId).Select(s => new UserMenu
                        {
                            Id = s.Key,
                            Label = s.FirstOrDefault().Screen.Module.Name,
                            LabelAr = s.FirstOrDefault().Screen.Module.NameAr,
                            Icon = s.FirstOrDefault().Screen.Module.Icon,
                            Expanded = screenId.HasValue ? s.Any(x => x.Screen.ModuleId == s.Key && x.Screen.Module.Screens.Any(a => a.ScreenId == screenId.Value)) : false,
                            IsActive = screenId.HasValue ? s.Any(x => x.Screen.ModuleId == s.Key && x.Screen.Module.Screens.Any(a => a.ScreenId == screenId.Value)) : false,
                            Order = s.FirstOrDefault().Screen.Module.Order,
                            UserMenuItems = s.Select(m => new UserMenuItem()
                            {
                                Id = m.ScreenId,
                                Label = m.Screen.Name,
                                LabelAr = m.Screen.NameAr,
                                Icon = m.Screen.Icon,
                                Expanded = false,
                                IsActive = screenId.HasValue ? screenId.Value == m.ScreenId : false,
                                ParentId = s.Key,
                                URL = m.Screen.Url,
                            }).ToList()
                        }).OrderBy(x => x.Order).ToList();
                        userMenus = userMenus.Select(sm => { sm.UserMenuItems = sm.UserMenuItems.DistinctBy(p => p.Id).ToList(); return sm; }).Distinct().ToList();
                        return userMenus;
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        _exceptionService.LogErrorAsync(ex, nameof(GetUserMenu), _userAccessor.GetCurrentUserId(), roleIds);
                        return userMenus;
                    }
                }
            }
        }
    }
}
