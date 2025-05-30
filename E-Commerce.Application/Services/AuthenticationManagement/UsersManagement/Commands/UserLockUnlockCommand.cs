using Adam.Logging.Interfaces;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;


namespace E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Commands
{
    public class UserLockUnlockCommand
    {
        public class Command : IRequest<BaseResponse>
        {
            public Guid userId;
            public bool isLock;

            public Command(Guid userId, bool isLock)
            {
                this.userId = userId;
                this.isLock = isLock;
            }

            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
            {
                public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        User user = _unitOfWork.GetRepository<User>().GetByExpressionAsNoTracking(p => p.UserId == request.userId).FirstOrDefault();
                        user.IsBlock = request.isLock;

                        await _unitOfWork.CommitChanges();

                        return new BaseResponse(true, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "UserLockUnlockCommand", _userAccessor.GetCurrentUserId());
                        return new BaseResponse(false, ex.Message);
                    }
                }
            }

        }
    }
}
