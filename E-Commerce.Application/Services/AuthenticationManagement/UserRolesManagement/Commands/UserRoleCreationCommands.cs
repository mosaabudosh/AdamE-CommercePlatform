using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.AuthenticationManagement.UserRolesManagement.Commands
{
    public class UserRoleCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public List<UserRoleModel> UserRoles { get; set; }
            public Command(List<UserRoleModel> userRoles)
            {
                UserRoles = userRoles;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!request.UserRoles.Any())
                    {
                        return new BaseResponse(true);
                    }

                    List<Domain.Domains.Authorisation.UserRole> userRoles = request.UserRoles.Select(s =>
                    new Domain.Domains.Authorisation.UserRole
                    {
                        UserRoleId = 0,
                        UserId = s.UserId,
                        RoleId = s.RoleId,
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    }).ToList();
                    _unitOfWork.GetRepository<Domain.Domains.Authorisation.UserRole>().AddRange(userRoles);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "UserRoleCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
