using Adam.Logging.Interfaces;
using E_Commerce.Application.Authentication.Interfaces;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using static Adam.Generic.Constants.Constants;

namespace E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Commands
{
    public class ChangePasswordCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public Guid UserId;
            public string OldPassword;
            public string NewPassword;

            public Command(Guid userId, string oldPassword, string newPassword)
            {
                UserId = userId;
                OldPassword = oldPassword;
                NewPassword = newPassword;
            }
            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IAuthenticationService _authenticationService, IMediator _mediator) : IRequestHandler<Command, BaseResponse>
            {
                public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        User user = _unitOfWork.GetRepository<User>().GetByExpressionAsNoTracking(x => x.UserId == request.UserId, includedEntities: new List<System.Linq.Expressions.Expression<Func<User, object>>> { z => z.UserLogins.Where(p => !p.LogOutDate.HasValue) }).SingleOrDefault();
                        if (user is null)
                        {
                            return new BaseResponse(false, string.Empty, ErrorCode.InvalidUser);
                        }
                        if (string.IsNullOrEmpty(request.OldPassword))
                        {
                            return new BaseResponse(false, string.Empty, ErrorCode.InvalidUserNameOrPassword);
                        }
                        //if (!_authenticationService.VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt))
                        //{
                        //    return new BaseResponse(false, string.Empty, ErrorCode.InvalidUserNameOrPassword);
                        //}
                        byte[] passwordHash, passwordSalt;
                        _authenticationService.HashPassword(request.NewPassword, out passwordHash, out passwordSalt);
                        user.PasswordHash = passwordHash;
                        user.PasswordSalt = passwordSalt;
                        _unitOfWork.GetRepository<User>().Update(user);
                        foreach (UserLogin exsistUserLogin in user.UserLogins)
                        {
                            await _mediator.Send(new KillRefreshAccessTokenCommands.Command(exsistUserLogin, false));
                        }
                        await _unitOfWork.CommitChanges();
                        return new BaseResponse(true);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "ChangePasswordCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse(false, ex.Message);
                    }
                }

            }
        }
    }
}
