

using Adam.Logging.Interfaces;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;

namespace E_Commerce.Application.Services.AuthenticationManagement.UserRolesManagement.Commands
{
    public class UserRoleDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int UserRoleId { get; set; }
            public Command(int userRoleId)
            {
                UserRoleId = userRoleId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Authorisation.UserRole screenAction = await _unitOfWork.GetRepository<Domain.Domains.Authorisation.UserRole>().GetById(request.UserRoleId);
                    _unitOfWork.GetRepository<Domain.Domains.Authorisation.UserRole>().Delete(screenAction);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UserRoleDeletionCommands", _userAccessor.GetCurrentUserId(), request.UserRoleId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
