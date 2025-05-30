using Adam.Logging.Interfaces;
using E_Commerce.Application.Authentication.Interfaces;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Adam.Generic.Constants.Constants;

namespace E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Commands
{
    public class CheckRefreshAccessTokenCommands
    {
        public class Command : IRequest<BaseResponse<UserModel>>
        {
            public string RefreshToken;

            public Command(string refreshToken)
            {
                RefreshToken = refreshToken;
            }
            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, ITokenHandler _tokenHandler) : IRequestHandler<Command, BaseResponse<UserModel>>
            {
                public async Task<BaseResponse<UserModel>> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        IQueryable<UserLogin> userLogins = _unitOfWork.GetRepository<UserLogin>().GetByExpression(x => x.RefreshToken == request.RefreshToken && x.IsActive, includedEntities: new List<System.Linq.Expressions.Expression<Func<UserLogin, object>>>
                        {
                            z => z.User,
                            z => z.User.UserRoles
                        });
                        UserLogin userLogin = await _unitOfWork.GetRepository<UserLogin>().FirstOrDefaultAsync(userLogins);
                        if (userLogin is null)
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
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "CheckRefreshAccessTokenCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse<UserModel>(false, ex.Message);
                    }
                }

            }
        }
    }
}
