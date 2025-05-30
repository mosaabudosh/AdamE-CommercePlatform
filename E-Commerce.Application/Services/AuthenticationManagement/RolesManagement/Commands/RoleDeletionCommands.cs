using Adam.Logging.Interfaces;
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

namespace E_Commerce.Application.Services.AuthenticationManagement.RolesManagement.Commands
{
    public class RoleDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public int RoleId { get; set; }
            public Command(int roleId)
            {
                RoleId = roleId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.RoleId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    Domain.Domains.Authorisation.Role role = await _unitOfWork.GetRepository<Domain.Domains.Authorisation.Role>().GetById(request.RoleId);
                    _unitOfWork.GetRepository<Domain.Domains.Authorisation.Role>().Delete(role);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "RoleDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(int roleId)
            {
                try
                {
                    bool haveRelatedData = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Role>().GetByExpressionAsNoTracking(i => i.RoleId == roleId && (i.RoleScreens.Any() || i.UserRoles.Any())).Any();
                    if (haveRelatedData)
                    {
                        return new BaseResponse(false, string.Empty, ErrorCode.RelatedData);
                    }
                    return new BaseResponse(true, string.Empty);

                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), roleId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
