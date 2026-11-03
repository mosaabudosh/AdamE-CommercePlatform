using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Generic.Constants.Constants;

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.RolesManagement.Commands
{
    public class RoleUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public RoleModel RoleModel { get; set; }
            public Command(RoleModel roleModel)
            {
                RoleModel = roleModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Authorisation.Role exisitRole = await _unitOfWork.GetRepository<Domain.Domains.Authorisation.Role>().GetById(request.RoleModel.RoleId);

                    exisitRole.Name = request.RoleModel.Name.Trim();
                    exisitRole.NameAr = request.RoleModel.NameAr.Trim();
                    exisitRole.LastUpdatedDate = DateTime.UtcNow;
                    exisitRole.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    BaseResponse validationBaseResponse = ValidateUpdation(exisitRole);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
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
                    await _exceptionService.LogErrorAsync(ex, "RoleUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(Domain.Domains.Authorisation.Role role)
            {
                try
                {
                    Domain.Domains.Authorisation.Role existingRecord = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Role>().GetByExpressionAsNoTracking(x => (x.Name == role.Name || x.NameAr == role.NameAr) && x.RoleId != role.RoleId).FirstOrDefault();

                    if (existingRecord is not null)
                    {
                        return new BaseResponse(false, string.Empty, ErrorCode.AlreadyExsist);
                    }
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId(), role);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
