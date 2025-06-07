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
    public class UserLogOutCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public string RefreshToken;

            public Command(string refreshToken)
            {
                RefreshToken = refreshToken;
            }
            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator) : IRequestHandler<Command, BaseResponse>
            {
                public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        IQueryable<UserLogin> userLogins = _unitOfWork.GetRepository<UserLogin>().GetByExpressionAsNoTracking(x => x.RefreshToken == request.RefreshToken);
                        UserLogin userLogin = await _unitOfWork.GetRepository<UserLogin>().FirstOrDefaultAsync(userLogins);
                        if (userLogin is null)
                        {
                            return new BaseResponse<string>(false, string.Empty, ErrorCode.InvalidRefreshToken, string.Empty);
                        }
                        BaseResponse killRefreshAccessTokenResponse =  await _mediator.Send(new KillUserRefreshAccessTokenCommands.Command(userLogin));
                        if (!killRefreshAccessTokenResponse.IsSuccess)
                        {
                            return new BaseResponse<string>(false, killRefreshAccessTokenResponse.Message, killRefreshAccessTokenResponse.ErrorCode);
                        }
                        return new BaseResponse(true);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "UserLogOutCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse(false, ex.Message);
                    }
                }
            }
        }
    }
}
