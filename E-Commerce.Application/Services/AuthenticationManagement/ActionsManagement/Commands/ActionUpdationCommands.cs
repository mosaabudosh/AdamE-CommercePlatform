using Adam.Logging.Interfaces;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Adam.Generic.Constants.Constants;
using ActionModel = E_Commerce.Application.Model.Authorisation.ActionModel;

namespace E_Commerce.Application.Services.AuthenticationManagement.ActionsManagement.Commands
{
    public class ActionUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public ActionModel ActionModel { get; set; }
            public Command(ActionModel actionModel)
            {
                ActionModel = actionModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Authorisation.Action action = new Domain.Domains.Authorisation.Action()
                    {
                        ActionId = request.ActionModel.ActionId,
                        Name = request.ActionModel.Name.Trim(),
                        NameAr = !string.IsNullOrEmpty(request.ActionModel.NameAr) ? request.ActionModel.NameAr.Trim() : request.ActionModel.Name.Trim(),
                        LastUpdatedDate = DateTime.UtcNow,
                        LastUpdateUserId = _userAccessor.GetCurrentUserId(),
                    };

                    BaseResponse validationBaseResponse = ValidateUpdation(action);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().Update(action);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ActionUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(Domain.Domains.Authorisation.Action action)
            {
                try
                {
                    Domain.Domains.Authorisation.Action existingRecord = _unitOfWork.GetRepository<Domain.Domains.Authorisation.Action>().GetByExpressionAsNoTracking(x => (x.Name == action.Name || x.NameAr == action.NameAr) && x.ActionId != action.ActionId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId(), action);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
