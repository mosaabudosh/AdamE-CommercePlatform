

using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using Adam.Logging.Services.MSSQLServer;
using E_Commerce.Application.Authentication.Interfaces;
using E_Commerce.Application.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using System.Collections.Generic;
using System.Text;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly IExceptionService _exceptionService;
        private readonly IUserAccessor _userAccessor;
        private readonly ITokenHandler _tokenHandler;

        public UserManagementService(IUnitOfWork unitOfWork, IAuthenticationService authenticationService, IExceptionService exceptionService, IUserAccessor userAccessor, ITokenHandler tokenHandler)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _exceptionService = exceptionService;
            _userAccessor = userAccessor;
            _tokenHandler = tokenHandler;
        }

        public async Task<BaseResponse> Registration(UserModel userModel)
        {
            try
            {
                IQueryable<User> usersQueryable = _unitOfWork.GetRepository<User>().GetByExpression(x => x.UserName == userModel.UserName, x => x.OrderBy(z => z.UserName));
                if (usersQueryable.Any())
                {
                    return new BaseResponse(false, string.Empty, ErrorCode.AlreadyExsist);
                }

                User user = new User()
                {
                    UserName = userModel.UserName,
                    FirstName = userModel.FirstName,
                    FirstNameAr = userModel.FirstNameAr,
                    SecondName = userModel.SecondName,
                    SecondNameAr = userModel.SecondNameAr,
                    LastName = userModel.LastName,
                    LastNameAr = userModel.LastNameAr,
                    Email = userModel.Email,
                    MobileNumber = userModel.MobileNumber,
                    LanguageId = userModel.LanguageId,
                    GenderId = userModel.GenderId,
                    Adresse = userModel.Adresse,
                    CityId = userModel.CityId,
                    DateOfBirth = userModel.DateOfBirth,
                    IsBlock = userModel.IsBlock.HasValue ? userModel.IsBlock.Value : false,
                    UserTypeId = userModel.UserTypeId,
                    UserRoles = new List<UserRole>(),
                    CreationDate = DateTime.UtcNow,
                    IsActive = true
                };
                byte[] passwordHash, passwordSalt;
                _authenticationService.HashPassword(userModel.Password, out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                user.UserRoles.Add(new UserRole
                {
                    RoleId = (int)Roles.User,
                    CreationDate = DateTime.UtcNow,
                    IsActive = true
                });
                _unitOfWork.GetRepository<User>().Insert(user);
                await _unitOfWork.CommitChanges();
                return new BaseResponse(true, string.Empty);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(Registration), _userAccessor.GetCurrentUserId(), userModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse<UserModel>> Login(string userName, string password)
        {
            try
            {
                IQueryable<User> users = _unitOfWork.GetRepository<User>().GetByExpression(x => x.UserName == userName && x.IsActive == true, includedEntities: new System.Collections.Generic.List<System.Linq.Expressions.Expression<Func<User, object>>>
                {
                    z => z.UserLogins.Where(p => !string.IsNullOrEmpty(p.AccessToken)),
                    z => z.UserRoles
                });
                User user = users.FirstOrDefault();
                if (user == null)
                {
                    return new BaseResponse<UserModel>(false, string.Empty, ErrorCode.InvalidUserNameOrPassword);
                }
                if (!_authenticationService.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                {
                    return new BaseResponse<UserModel>(false, string.Empty, ErrorCode.InvalidUserNameOrPassword);
                }
                if (user.UserLogins.Any())
                {
                    foreach (UserLogin exsistUserLogin in user.UserLogins)
                    {
                        await KillRefreshAccessToken(exsistUserLogin, false);
                    }
                }
                user.UserLogins = new List<UserLogin>();

                UserLogin userLogin = new UserLogin()
                {
                    UserId = user.UserId,
                    User = user,
                    LoginDate = DateTime.Now,
                    CreationUserId = user.UserId,
                    CreationDate = DateTime.Now,
                    IsActive = true
                };
                userLogin = _tokenHandler.BuildTokens(userLogin);

                _unitOfWork.GetRepository<UserLogin>().Insert(userLogin);

                await _unitOfWork.CommitChanges();

                List<int> roleIds = user.UserRoles.Select(s => s.RoleId).ToList();
                List<UserMenu> userMenus = GetUserMenu(roleIds);

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
                    LanguageId = user.LanguageId,
                    ProfileImage = user.ProfileImage,
                    RoleIds = roleIds,
                    UserMenus = userMenus
                };
                return new BaseResponse<UserModel>(true, string.Empty, string.Empty, userModel);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(Login), _userAccessor.GetCurrentUserId(), userName, password);
                return new BaseResponse<UserModel>(false, ex.Message);
            }
        }

        public List<UserMenu> GetUserMenu(List<int> roleIds)
        {
            List<UserMenu> userMenus = new List<UserMenu>();
            try
            {
                userMenus = _unitOfWork.GetRepository<RoleScreen>().GetByExpression(p => roleIds.Contains(p.RoleId)).GroupBy(g => g.Screen.ModuleId).Select(s => new UserMenu
                {
                    Id = s.Key,
                    Label = s.FirstOrDefault().Screen.Module.Name,
                    LabelAr = s.FirstOrDefault().Screen.Module.NameAr,
                    Icon = s.FirstOrDefault().Screen.Module.Icon,
                    Expanded = false,
                    IsActive = false,
                    UserMenuItems = s.Select(m => new UserMenuItem()
                    {
                        Id = m.ScreenId,
                        Label = m.Screen.Name,
                        LabelAr = m.Screen.NameAr,
                        Icon = m.Screen.Icon,
                        Expanded = false,
                        IsActive = false,
                        ParentId = s.Key,
                        URL = m.Screen.Url
                    }).ToList()
                }).ToList();
                return userMenus;
            }
            catch (Exception ex) 
            {
                _exceptionService.LogErrorAsync(ex, nameof(GetUserMenu), _userAccessor.GetCurrentUserId(), roleIds);
                return userMenus;
            }
        }

        public async Task<BaseResponse> LogOut(string refreshToken)
        {
            try
            {
                IQueryable<UserLogin> userLogins = _unitOfWork.GetRepository<UserLogin>().GetByExpression(x => x.RefreshToken == refreshToken);
                UserLogin userLogin = userLogins.FirstOrDefault();
                if (userLogin == null)
                {
                    return new BaseResponse<string>(false, string.Empty, ErrorCode.InvalidRefreshToken, string.Empty);
                }
                BaseResponse killRefreshAccessTokenResponse = await KillRefreshAccessToken(userLogin);
                if (!killRefreshAccessTokenResponse.IsSuccess)
                {
                    return new BaseResponse<string>(false, killRefreshAccessTokenResponse.Message, killRefreshAccessTokenResponse.ErrorCode);
                }
                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(LogOut), _userAccessor.GetCurrentUserId(), refreshToken);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse<UserModel>> CheckRefreshAccessToken(string refreshToken)
        {
            try
            {
                IQueryable<UserLogin> userLogins = _unitOfWork.GetRepository<UserLogin>().GetByExpression(x => x.RefreshToken == refreshToken && x.IsActive, includedEntities: new List<System.Linq.Expressions.Expression<Func<UserLogin, object>>>
                {
                    z => z.User,
                    z => z.User.UserRoles
                });
                UserLogin userLogin = userLogins.FirstOrDefault();
                if (userLogin == null)
                {
                    return new BaseResponse<UserModel>(false, string.Empty, ErrorCode.InvalidRefreshToken);
                }
                //if (DateTime.Now.Ticks > userLogin.RefreshTokenExpirationDate)
                //{
                //    BaseResponse killRefreshAccessTokenResponse = await KillRefreshAccessToken(userLogin);
                //    if (!killRefreshAccessTokenResponse.IsSuccess)
                //    {
                //        return new BaseResponse<UserModel>(false, killRefreshAccessTokenResponse.Message, killRefreshAccessTokenResponse.ErrorCode);
                //    }
                //    return new BaseResponse<UserModel>(false, string.Empty, ErrorCode.InvalidRefreshToken);
                //}
                userLogin = _tokenHandler.BuildTokens(userLogin);
                await _unitOfWork.CommitChanges();
                UserModel userModel = new UserModel()
                {
                    UserId = userLogin.UserId.ToString(),
                    UserName = userLogin.User.UserName,
                    FirstName = userLogin.User.FirstName,
                    FirstNameAr = userLogin.User.FirstNameAr,
                    LastName = userLogin.User.LastName,
                    LastNameAr = userLogin.User.LastNameAr,
                    AccessToken = userLogin.AccessToken,
                    RefreshToken = userLogin.RefreshToken
                };
                return new BaseResponse<UserModel>(true, string.Empty, string.Empty, userModel);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(CheckRefreshAccessToken), _userAccessor.GetCurrentUserId(), refreshToken);
                return new BaseResponse<UserModel>(false, ex.Message);
            }
        }

        private async Task<BaseResponse> KillRefreshAccessToken(UserLogin userLogin, bool isCommit = true)
        {
            try
            {
                userLogin.LogOutDate = DateTime.Now;
                userLogin.RefreshTokenExpirationDate = 0;
                userLogin.RefreshToken = string.Empty;
                userLogin.AccessToken = string.Empty;
                userLogin.IsActive = false;
                if (isCommit)
                {
                    await _unitOfWork.CommitChanges();
                }
                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(KillRefreshAccessToken), _userAccessor.GetCurrentUserId(), userLogin, isCommit);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse> ChangePassword(Guid userId, string oldPassword, string newPassword)
        {
            try
            {
                User user = _unitOfWork.GetRepository<User>().GetByExpression(x => x.UserId == userId, includedEntities: new List<System.Linq.Expressions.Expression<Func<User, object>>> { z => z.UserLogins.Where(p => !p.LogOutDate.HasValue) }).SingleOrDefault();
                if (user == null)
                {
                    return new BaseResponse(false, string.Empty, ErrorCode.InvalidUser);
                }
                if (string.IsNullOrEmpty(oldPassword))
                {
                    return new BaseResponse(false, string.Empty, ErrorCode.InvalidUserNameOrPassword);
                }
                //if (!_authenticationService.VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt))
                //{
                //    return new BaseResponse(false, string.Empty, ErrorCode.InvalidUserNameOrPassword);
                //}
                byte[] passwordHash, passwordSalt;
                _authenticationService.HashPassword(newPassword, out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                _unitOfWork.GetRepository<User>().Update(user);
                foreach (UserLogin exsistUserLogin in user.UserLogins)
                {
                    await KillRefreshAccessToken(exsistUserLogin, false);
                }
                await _unitOfWork.CommitChanges();
                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(ChangePassword), _userAccessor.GetCurrentUserId(), userId, oldPassword, newPassword);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<UserLogin> GetUserLoginByUserId(Guid userId)
        {
            try
            {
                return _unitOfWork.GetRepository<UserLogin>().GetByExpression(x => x.UserId == userId && !string.IsNullOrEmpty(x.AccessToken)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(GetUserLoginByUserId), _userAccessor.GetCurrentUserId(), userId);
                return null;
            }
        }

        public async Task<BaseResponse<List<UserModel>>> GetUsers(int pageNumber, int pageSize)
        {
            try
            {
                List<UserModel> users = _unitOfWork.GetRepository<User>().GetByExpression().Select(s => new UserModel
                {
                    FirstName = s.FirstName,
                    SecondName = s.SecondName,
                    LastName = s.LastName,
                    FirstNameAr = s.FirstNameAr,
                    SecondNameAr = s.SecondNameAr,
                    LastNameAr = s.LastNameAr,
                    Email = s.Email,
                    LanguageId = s.LanguageId,
                    MobileNumber = s.MobileNumber,
                    UserId = s.UserId.ToString(),
                    GenderId = s.GenderId,
                    IsBlock = s.IsBlock,
                    IsActive = s.IsActive,
                }).Skip(pageNumber * pageSize).Take(pageSize).ToList();
                return new BaseResponse<List<UserModel>>(true, string.Empty, string.Empty, users);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(GetUsers), _userAccessor.GetCurrentUserId());
                return new BaseResponse<List<UserModel>>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<int>> GetUsersCount()
        {
            try
            {
                int usersCount = _unitOfWork.GetRepository<User>().GetByExpression().Count();
                return new BaseResponse<int>(true, string.Empty, string.Empty, usersCount);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(GetUsersCount), _userAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<UserModel>> GetUserById(Guid userId)
        {
            try
            {
                UserModel user = _unitOfWork.GetRepository<User>().GetByExpression(p => p.UserId == userId).Select(s => new UserModel
                {
                    FirstName = s.FirstName,
                    SecondName = s.SecondName,
                    LastName = s.LastName,
                    FirstNameAr = s.FirstNameAr,
                    SecondNameAr = s.SecondNameAr,
                    LastNameAr = s.LastNameAr,
                    Email = s.Email,
                    LanguageId = s.LanguageId,
                    MobileNumber = s.MobileNumber,
                    UserId = s.UserId.ToString(),
                    GenderId = s.GenderId,
                    UserName = s.UserName,
                    DateOfBirth = s.DateOfBirth,
                    Adresse = s.Adresse,
                    CityId =s.CityId,
                    UserTypeId = s.UserTypeId
                }).SingleOrDefault();
                return new BaseResponse<UserModel>(true, string.Empty, string.Empty, user);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(GetUserById), _userAccessor.GetCurrentUserId(),userId);
                return new BaseResponse<UserModel>(false, ex.Message);
            }
        }

        public async Task<BaseResponse> UpdateUser(UserModel userModel)
        {
            try
            {
                Guid userId = new Guid(userModel.UserId);

                User user = _unitOfWork.GetRepository<User>().GetByExpression(x => x.UserId == userId).SingleOrDefault();
                user.FirstName = userModel.FirstName;
                user.FirstNameAr = userModel.FirstNameAr;
                user.SecondName = userModel.SecondName;
                user.SecondNameAr = userModel.SecondNameAr;
                user.LastName = userModel.LastName;
                user.LastNameAr = userModel.LastNameAr;
                user.Email = userModel.Email;
                user.MobileNumber = userModel.MobileNumber;
                user.CityId = userModel.CityId;
                user.LanguageId = userModel.LanguageId;
                user.GenderId = userModel.GenderId;
                user.LastUpdatedDate = DateTime.UtcNow;

                await _unitOfWork.CommitChanges();
                return new BaseResponse(true, string.Empty);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(UpdateUser), _userAccessor.GetCurrentUserId(), userModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse> DeleteUser(string userId)
        {
            try
            {
                User user = _unitOfWork.GetRepository<User>().GetByExpression(p => p.UserId == new Guid(userId)).SingleOrDefault();
                user.IsActive = false;
                await _unitOfWork.CommitChanges();
                return new BaseResponse<UserModel>(true,string.Empty);
            }
            catch (Exception ex)
            {
                await _exceptionService.LogErrorAsync(ex, nameof(DeleteUser), _userAccessor.GetCurrentUserId(), userId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
